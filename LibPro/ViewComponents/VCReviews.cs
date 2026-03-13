using LibPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibPro.ViewComponents
{
    public class VCReviews : ViewComponent
    {
        private readonly LibproContext _context;

        public VCReviews(LibproContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(long bibId)
        {
           
            var reviews = await _context.Reviews
                .Include(r => r.Patron)
                .Where(r => r.BibID == bibId && r.RevStatus == 2)
                .OrderByDescending(r => r.PostDate)
                .ToListAsync();

           
            ViewBag.BibID = bibId;

            return View(reviews);
        }

    }
}
