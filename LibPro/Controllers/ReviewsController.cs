using LibPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibPro.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly LibproContext _context;

        public ReviewsController(LibproContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Index()
        {
          
            var reviews = await _context.Reviews
                .Include(r => r.Biblio)
                .Include(r => r.Patron)
                .OrderBy(r => r.RevStatus)       
                .ThenByDescending(r => r.PostDate) 
                .ToListAsync();

            return View(reviews);
        }

        

        [Authorize(Roles = "Patron")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BibID,Title,Content,Rating")] Reviews review)
        {
            var patronId = User.FindFirstValue("PatronID");
            if (string.IsNullOrEmpty(patronId))
            {
                return RedirectToAction("Logout", "Login");
            }

            ModelState.Remove("PostDate");
            ModelState.Remove("RevStatus");
            ModelState.Remove("PatronID");

            if (ModelState.IsValid)
            {
                
                review.PatronID = patronId;
                review.PostDate = DateTime.Now;
                review.RevStatus = 1; 

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "評論已成功送出！將於館員審核通過後顯示。";

                
                return RedirectToAction("BookDetails", "Home", new { id = review.BibID });
            }

            TempData["ErrorMessage"] = "評論格式有誤，請重新填寫。";
            return RedirectToAction("BookDetails", "Home", new { id = review.BibID });
        }

      
        

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews
                .Include(r => r.Biblio)
                .Include(r => r.Patron)
                .Include(r => r.SystemStatus)
                .FirstOrDefaultAsync(m => m.ReviewID == id);
            if (reviews == null)
            {
                return NotFound();
            }

            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateStatus(long id, byte status)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }


            review.RevStatus = status;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "評論狀態已更新！";
            return RedirectToAction(nameof(Index));
        }



    }
}
