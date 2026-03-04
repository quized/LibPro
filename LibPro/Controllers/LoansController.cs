using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibPro.Models;

namespace LibPro.Controllers
{
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

                var bookItem = await _context.BookItems.FindAsync(loans.ItemID);
                if (bookItem == null || bookItem.ItmStatus != 1)
                {
                    return Json(new { success = false, message = "此書籍不在架上，或已被借出。" });
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


            bookItem.ItmStatus = 1;
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
                    fine = fineAmount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "系統存檔錯誤：" + ex.Message });
            }
        }
    }




}
