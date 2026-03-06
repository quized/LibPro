using LibPro.Models;
using LibPro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LibPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LibproContext _context;

        public HomeController(ILogger<HomeController> logger, LibproContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async  Task<IActionResult> Index()
        {
            var topBooks = await _context.Biblios
                .Where(b => b.isDeleted == 0) 
                .OrderByDescending(b => b.BibID)
                .Take(5)
                .ToListAsync();
            return View(topBooks);
        }

        public async Task<IActionResult> Search(string searchString)
        {
            var query = _context.Biblios
                .Include(b => b.Category)
                .Where(b => b.isDeleted == 0);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var keyword = searchString.Trim();
                query = query.Where(b =>
                    b.BTitle.Contains(keyword) ||
                    (b.Author != null && b.Author.Contains(keyword)) ||
                    (b.ISBN != null && b.ISBN.Contains(keyword))
                );
            }

            var result = await query
                .OrderByDescending(b => b.BibID)
                .ToListAsync();

            ViewData["CurrentFilter"] = searchString;
            return View(result);
        }

        [Authorize(Roles = "Staff")]
        public IActionResult StaffCenter()
        {
            return View();
        }

        
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
