using LibPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibPro.Controllers
{
    [Authorize(Roles = "Staff")]
    public class LoansController : Controller
    {
        private readonly LibproContext _context;

        public LoansController(LibproContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            var libproContext = _context.Loans.Include(l => l.BookItem).Include(l => l.Patron);
            return View(await libproContext.ToListAsync());
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loans = await _context.Loans
                .Include(l => l.BookItem)
                .Include(l => l.Patron)
                .FirstOrDefaultAsync(m => m.LoanID == id);
            if (loans == null)
            {
                return NotFound();
            }

            return View(loans);
        }


        public IActionResult CheckOut()
        {
            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(Loans loans)
        {
            ModelState.Remove("LoanID");
            ModelState.Remove("LoanDate");
            ModelState.Remove("DueDate");

            if (ModelState.IsValid)
            {



                var patron = await _context.Patrons.FindAsync(loans.PatronID);
                if (patron == null)
                {

                    return Json(new { success = false, message = "找不到此借閱人，請確認證號是否正確。" });
                }

                int maxBorrowLimit = 10;

                int currentLoanCount = await _context.Loans.CountAsync(l => l.PatronID == loans.PatronID && l.ReturnDate == null);

                if (currentLoanCount >= maxBorrowLimit)
                {
                    return Json(new { success = false, message = $"該讀者目前已借閱 {currentLoanCount} 本書，已達借閱上限 ({maxBorrowLimit} 本)，請先歸還部分書籍。" });
                }


                var bookItem = await _context.BookItems.FindAsync(loans.ItemID);
                Reserves targetReserve = null; 

               
                if (bookItem == null || (bookItem.ItmStatus != 1 && bookItem.ItmStatus != 3))
                {
                    return Json(new { success = false, message = "此書籍不在架上，或已被借出。" });
                }

               
                if (bookItem.ItmStatus == 3)
                {
                    targetReserve = await _context.Reserves.FirstOrDefaultAsync(r =>
                        r.ItemID == bookItem.ItemID && r.PatronID == loans.PatronID && r.ResStatus == 2);

                   
                    if (targetReserve == null)
                    {
                        return Json(new { success = false, message = "此書籍為其他讀者預約保留中，不可借閱。" });
                    }
                }

                // 程式能順利走到這裡，代表「書在架上(1)」或是「這本書剛好是這位讀者預約的(3)」，可以放心繼續往下執行借書邏輯！


                bool hasBorrowedSameBook = await _context.Loans
                 .Include(l => l.BookItem) 
                 .AnyAsync(l => l.PatronID == loans.PatronID && l.ReturnDate == null && l.BookItem.BibID == bookItem.BibID);

                if (hasBorrowedSameBook)
                {
                    return Json(new { success = false, message = "以借閱過同書目書籍，不可重複借閱。" });
                }


                var loanIDResult = await _context.Database.SqlQuery<string>($"exec getLoanID").ToListAsync();
                var newLoanID = loanIDResult.FirstOrDefault();
                if (string.IsNullOrEmpty(newLoanID))
                {
                    return Json(new { success = false, message = "無法產生借閱編號，請聯絡管理員。" });
                }

                var unpaidFines = await _context.Fines
                    .Where(f => f.Loan.PatronID == loans.PatronID && !f.ISPaid)
                    .Include(f => f.FineType)
                    .Include(f => f.Loan)
                    .ToListAsync();

                decimal totalOwed = 0;
                foreach (var fine in unpaidFines)
                {

                    int overdueDays = (DateTime.Now.Date - fine.Loan.DueDate.Date).Days;
                    if (overdueDays < 0) overdueDays = 0;


                    decimal fineAmount = fine.FineType.UnitPrice * overdueDays;
                    totalOwed += fineAmount;
                }



                var hasOverdueBooks = await _context.Loans
                    .AnyAsync(l => l.PatronID == loans.PatronID && l.ReturnDate == null && l.DueDate < DateTime.Now);

                if (hasOverdueBooks)
                {
                    return Json(new { success = false, message = "該讀者尚有逾期圖書未歸還，請先歸還舊書。" });
                }

                if (totalOwed >= 100)
                {
                    return Json(new { success = false, message = $"欠款金額已達 ${totalOwed}，請先結清罰金。" });
                }



                loans.LoanID = newLoanID;
                loans.LoanDate = DateTime.Now;
                loans.DueDate = DateTime.Now.AddDays(14);
                loans.RenewalCount = 0;


             
                _context.Loans.Add(loans);
                bookItem.ItmStatus = 2;
                _context.BookItems.Update(bookItem);

                
                if (targetReserve != null)
                {
                    targetReserve.ResStatus = 3;
                    _context.Reserves.Update(targetReserve);
                }

                try
                {
                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        bookId = loans.ItemID,
                        dueDate = loans.DueDate.ToString("yyyy/MM/dd"),
                        time = loans.LoanDate.ToString("HH:mm:ss")
                    });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "系統錯誤：" + ex.Message });
                }
            }

            return Json(new { success = false, message = "資料格式錯誤，請重新掃描。" });
        }






        public IActionResult CheckIn()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(string ItemID, string Notes)
        {

            if (string.IsNullOrWhiteSpace(ItemID))
            {
                return Json(new { success = false, message = "請刷入書籍條碼。" });
            }


            var loan = await _context.Loans
                .FirstOrDefaultAsync(l => l.ItemID == ItemID && l.ReturnDate == null);

            if (loan == null)
            {
                return Json(new { success = false, message = "找不到此書的借出紀錄，可能已歸還或條碼錯誤。" });
            }


            var bookItem = await _context.BookItems.FindAsync(ItemID);
            if (bookItem == null)
            {
                return Json(new { success = false, message = "系統中找不到此書籍主檔。" });
            }

            DateTime today = DateTime.Now;
            loan.ReturnDate = today; // 押上歸還時間

            loan.Notes = Notes;

            int overdueDays = 0;
            decimal fineAmount = 0m;


            if (today.Date > loan.DueDate.Date)
            {
                overdueDays = (int)(today.Date - loan.DueDate.Date).TotalDays;
                var overdueFineType = await _context.FineTypes.FindAsync((byte)1);

                if (overdueFineType != null)
                {
                    fineAmount = overdueDays * overdueFineType.UnitPrice;

                    var fineIDResult = await _context.Database.SqlQuery<string>($"exec GetFineID").ToListAsync();
                    string newFineID = fineIDResult.FirstOrDefault();



                    var newFine = new Fines
                    {
                        FineID = newFineID,
                        CreatedDate = today,
                        ISPaid = false,
                        FTID = overdueFineType.FTID,
                        LoanID = loan.LoanID
                    };


                    _context.Fines.Add(newFine);
                }
            }



            var topReserve = await _context.Reserves
                  .Where(r => r.BookItem.BibID == bookItem.BibID && r.ResStatus == 1)
                  .OrderBy(r => r.ResDate) 
                  .FirstOrDefaultAsync();

            bool isReservedNow = false;
            string reserveMessage = "";

            
            bookItem.ItmStatus = 1;

          
            if (topReserve != null)
            {
               
                bookItem.ItmStatus = 3;

            
                topReserve.ResStatus = 2; 
                topReserve.ItemID = bookItem.ItemID; 
                topReserve.ExpiryDate = today.AddDays(7); 

                _context.Reserves.Update(topReserve);
                isReservedNow = true;
                reserveMessage = $"此書已為預約者 (證號:{topReserve.PatronID}) 保留，請移至預約保留區！";
            }

           
            _context.BookItems.Update(bookItem);
            

            try
            {

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    bookId = loan.ItemID,
                    returnTime = today.ToString("HH:mm:ss"),
                    isOverdue = overdueDays > 0,
                    overdueDays = overdueDays,
                    fine = fineAmount,
                    isReserved = isReservedNow,
                    reserveMsg = reserveMessage
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "系統存檔錯誤：" + ex.Message });
            }
        }
    }




}
