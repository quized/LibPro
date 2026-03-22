using LibPro.Models;
using LibPro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibPro.Controllers
{
    [Authorize(Roles = "Patron")]
    public class PatronCenterController : Controller
    {

        private readonly LibproContext _context;

        public PatronCenterController(LibproContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {

            var patronId = User.FindFirstValue("PatronID");

            if (string.IsNullOrEmpty(patronId))
            {
                return RedirectToAction("Logout", "Login");
            }


            var patron = await _context.Patrons
                .Include(p => p.City)
                .FirstOrDefaultAsync(p => p.PatronID == patronId);

            if (patron == null)
            {
                TempData["ErrorMessage"] = "找不到您的個人資料，請聯繫管理員。";
                return RedirectToAction("Logout", "Login");
            }


            return View(patron);
        }

        public async Task<IActionResult> Reserves()
        {

            var patronId = User.FindFirstValue("PatronID");

            if (string.IsNullOrEmpty(patronId))
            {
                return RedirectToAction("Logout", "Login");
            }


            var myReserves = await _context.Reserves
                .Include(r => r.BookItem)
                    .ThenInclude(bi => bi.Biblio)
                .Include(r => r.ReserveStatus)
                .Where(r => r.PatronID == patronId)
                .OrderByDescending(r => r.ResDate)
                .ToListAsync();

            return View(myReserves);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string resId)
        {
            if (string.IsNullOrEmpty(resId))
            {
                return NotFound();
            }

            var patronId = User.FindFirstValue("PatronID");
            if (string.IsNullOrEmpty(patronId))
            {
                return RedirectToAction("Logout", "Login");
            }

            var reserve = await _context.Reserves.Include(r => r.BookItem).FirstOrDefaultAsync(r => r.ResID == resId && r.PatronID == patronId);




            if (reserve == null)
            {

                TempData["ErrorMessage"] = "找不到該筆預約，或您無權取消。";
                return RedirectToAction(nameof(Reserves));
            }

            if (reserve.BookItem == null || reserve.BookItem.ItmStatus != 3)
            {
                TempData["BookErrorMessage"] = "書籍狀態有誤無法取消。";
                return RedirectToAction(nameof(Reserves));
            }





            reserve.ResStatus = 4;
            reserve.BookItem.ItmStatus = 1;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "預約已成功取消，實體書已重新開放借閱！";

            return RedirectToAction(nameof(Reserves));

        }

        public IActionResult Loans()
        {
            return View();
        }



        public async Task<IActionResult> GetNotifications()
        {
          
            var currentPatronId = User.FindFirstValue("PatronID");

    
            if (string.IsNullOrEmpty(currentPatronId))
            {
                return Json(new { count = 0, items = new List<object>() });
            }

            var notifications = new List<object>();
            var today = DateTime.Now.Date;

            var unpaidFines = await _context.Fines
                .Include(f => f.Loan)
                .Include(f => f.FineType)
                .Where(f => f.Loan != null && f.Loan.PatronID == currentPatronId && f.ISPaid == false)
                .ToListAsync();

            decimal totalFine = 0;

            foreach (var fine in unpaidFines)
            {
                if (fine.Loan == null || fine.FineType == null)
                {
                    continue;
                }

                if (fine.FineType.FTName != "逾期")
                {
                    totalFine += fine.FineType.UnitPrice;
                    continue;
                }

                DateTime endDate = today;

                if (fine.Loan.ReturnDate.HasValue)
                {
                    endDate = fine.Loan.ReturnDate.Value.Date;
                }

                var dueDate = fine.Loan.DueDate.Date;
                var overdueDays = (endDate - dueDate).Days;

                if (overdueDays > 0)
                {
                    totalFine += overdueDays * fine.FineType.UnitPrice;
                }
            }

            if (totalFine > 0)
            {
                notifications.Add(new
                {
                    type = "danger",
                    icon = "bi-cash-coin",
                    title = "未繳罰金提醒",
                    message = $"您目前有 {totalFine:0} 元的未繳罰金，請至櫃檯繳納以恢復完整借閱權限。"
                });
            }


         
            var loans = await _context.Loans
                .Include(l => l.BookItem)
                .ThenInclude(bi => bi.Biblio)
                .Where(l => l.PatronID == currentPatronId && l.ReturnDate == null)
                .ToListAsync();

            foreach (var loan in loans)
            {
                if (loan.DueDate.Date < today)
                {
                    notifications.Add(new
                    {
                        type = "danger",
                        icon = "bi-exclamation-octagon-fill",
                        title = "圖書已逾期！",                      
                        message = $"《{loan.BookItem.Biblio.BTitle}》已於 {loan.DueDate:yyyy-MM-dd} 到期，請盡速歸還！"
                    });
                    continue;
                }

                if (loan.DueDate.Date <= today.AddDays(3))
                {
                    notifications.Add(new
                    {
                        type = "warning",
                        icon = "bi-clock-history",
                        title = "即將到期",
                        message = $"《{loan.BookItem.Biblio.BTitle}》將於 {loan.DueDate:yyyy-MM-dd} 到期。"
                    });
                    continue;
                }
            }

          
            var arrivedReserves = await _context.Reserves
                .Include(r => r.BookItem)
                    .ThenInclude(bi => bi.Biblio)
               
                .Where(r => r.PatronID == currentPatronId && r.ResStatus == 2)
                .ToListAsync();

            foreach (var res in arrivedReserves)
            {
                notifications.Add(new
                {
                    type = "success",
                    icon = "bi-bag-check-fill",
                    title = "預約書已到館",
                    message = $"您預約的《{res.BookItem.Biblio.BTitle}》已可取書，保留期限至 {res.ExpiryDate:yyyy-MM-dd}。"
                });
            }

            return Json(new { count = notifications.Count, items = notifications });
        }

        public async Task<IActionResult> Fines()
        {
            var currentPatronId = User.FindFirstValue("PatronID");
            if (string.IsNullOrEmpty(currentPatronId))
            {
                return RedirectToAction("Login", "Login");
            }

            var today = DateTime.Now.Date;

            var finesData = await _context.Fines
                .Include(f => f.Loan)
                    .ThenInclude(l => l.BookItem)
                        .ThenInclude(bi => bi.Biblio)
                .Include(f => f.FineType)
                .Where(f => f.Loan != null && f.Loan.PatronID == currentPatronId)
                .OrderBy(f => f.ISPaid)
                .ThenByDescending(f => f.CreatedDate)
                .ToListAsync();

            
            var model = new List<PatronFineViewModel>();

            foreach (var fine in finesData)
            {
                
                var info = new PatronFineViewModel
                {
                    FineID = fine.FineID,
                    CreatedDate = fine.CreatedDate,
                    BookTitle = fine.Loan.BookItem.Biblio.BTitle,
                    FineTypeName = fine.FineType.FTName,
                    IsPaid = fine.ISPaid,
                    OverdueDays = 0,
                    Amount = 0
                };

                if (fine.FineType != null)
                {
                   
                    if (fine.FineType.FTName != "逾期")
                    {
                        info.Amount = fine.FineType.UnitPrice;
                    }

                  
                    if (fine.FineType.FTName == "逾期")
                    {
                        DateTime endDate = today;

                        if (fine.Loan.ReturnDate != null)
                        {
                            endDate = fine.Loan.ReturnDate.Value.Date;
                        }

                        var dueDate = fine.Loan.DueDate.Date;
                        var overdueDays = (endDate - dueDate).Days;

                        if (overdueDays > 0)
                        {
                            info.OverdueDays = overdueDays;
                            info.Amount = overdueDays * fine.FineType.UnitPrice;
                        }
                    }
                }

                model.Add(info);
            }
            ViewBag.TotalUnpaid = model.Where(m => !m.IsPaid).Sum(m => m.Amount);

            return View(model);
        }

        public async Task<IActionResult> MyLoans()
        {
            var currentPatronId = User.FindFirstValue("PatronID");
            if (string.IsNullOrEmpty(currentPatronId))
            {
                return RedirectToAction("Login", "Login");
            }

            var today = DateTime.Now.Date;

           
            var activeLoans = await _context.Loans
                .Include(l => l.BookItem)
                    .ThenInclude(bi => bi.Biblio)
                .Where(l => l.PatronID == currentPatronId && l.ReturnDate == null)
                .OrderBy(l => l.DueDate) 
                .ToListAsync();

           
            var activeReserves = await _context.Reserves
                .Where(r => r.ResStatus == 1 || r.ResStatus == 2)
                .ToListAsync();

            var model = new List<PatronLoanViewModel>();

            foreach (var loan in activeLoans)
            {
                var info = new PatronLoanViewModel
                {
                    LoanID = loan.LoanID,
                    BookTitle = loan.BookItem.Biblio.BTitle,
                    Author = loan.BookItem.Biblio.Author,
                    LoanDate = loan.LoanDate,
                    DueDate = loan.DueDate,
                    RenewalCount = loan.RenewalCount,

                    
                    CanRenew = true,
                    RenewMessage = "我要續借",
                    ButtonClass = "btn-outline-primary"
                };




              
                if ((loan.DueDate.Date - today).Days > 3)
                {
                    info.CanRenew = false;
                    info.RenewMessage = "尚未開放";
                    info.ButtonClass = "btn-outline-secondary disabled opacity-50";
                }


                if (activeReserves.Any(r => r.ItemID == loan.ItemID))
                {
                    info.CanRenew = false;
                    info.RenewMessage = "有人預約";
                    info.ButtonClass = "btn-warning disabled opacity-75 text-dark fw-bold";
                }


                if (loan.RenewalCount >= 1)
                {
                    info.CanRenew = false;
                    info.RenewMessage = "已達上限";
                    info.ButtonClass = "btn-secondary disabled opacity-75";
                }

                
                if (loan.DueDate.Date < today)
                {
                    info.CanRenew = false;
                    info.RenewMessage = "已逾期";
                    info.ButtonClass = "btn-danger disabled opacity-75";
                }

                model.Add(info);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RenewLoan(string loanId)
        {
            var currentPatronId = User.FindFirstValue("PatronID");
            if (string.IsNullOrEmpty(currentPatronId))
            {
                return RedirectToAction("Login", "Login");
            }

          
            var loan = await _context.Loans
                .Include(l => l.BookItem)
                    .ThenInclude(bi => bi.Biblio)
                .FirstOrDefaultAsync(l => l.LoanID == loanId && l.PatronID == currentPatronId && l.ReturnDate == null);

            if (loan == null)
            {
                TempData["ErrorMessage"] = "找不到該借閱紀錄，或已歸還。";
                return RedirectToAction(nameof(MyLoans));
            }


            var today = DateTime.Now.Date;

            
            if ((loan.DueDate.Date - today).Days > 3)
            {
                TempData["ErrorMessage"] = "系統規定：圖書需於【到期日前 3 天內】方可辦理續借手續！";
                return RedirectToAction(nameof(MyLoans));
            }

            if (loan.DueDate.Date < today)
            {
                TempData["ErrorMessage"] = "該圖書已逾期，無法續借！";
                return RedirectToAction(nameof(MyLoans));
            }

            bool isReserved = await _context.Reserves.AnyAsync(r => r.BookItem!.ItemID == loan.ItemID && (r.ResStatus == 1 || r.ResStatus == 2));

            if (isReserved)
            {
                TempData["ErrorMessage"] = "該圖書目前有其他讀者預約，無法續借！";
                return RedirectToAction(nameof(MyLoans));
            }


            loan.DueDate = loan.DueDate.AddDays(14);
            loan.RenewalCount += 1;

            _context.Update(loan);
            await _context.SaveChangesAsync();

           
            string bookTitle = loan.BookItem.Biblio.BTitle;

            TempData["SuccessMessage"] = $"《{bookTitle}》續借成功！新到期日為：{loan.DueDate:yyyy-MM-dd}";
            return RedirectToAction(nameof(MyLoans));
        }
    }
}


