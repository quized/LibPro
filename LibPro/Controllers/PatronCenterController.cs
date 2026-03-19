using LibPro.Models;
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

    }
}


