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
                .OrderByDescending(b => b.BookItems!.SelectMany(bi => bi.Loans).Count())
                .Take(3)
                .ToListAsync();
            return View(topBooks);
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
