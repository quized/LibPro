using LibPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Index()
        {
            return View();
        }
    }
}
