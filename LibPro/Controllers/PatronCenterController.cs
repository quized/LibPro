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
    }
}
    

