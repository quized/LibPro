using LibPro.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibPro.ViewComponents
{
    public class VCAnn : ViewComponent
    {
        private readonly LibproContext _context;
        public VCAnn(LibproContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var announcements = _context.Announcements
                .Where(a => a.IsVisible == true)
                .OrderByDescending(a => a.CreatedDate)
                .Take(10) 
                .ToList();
            return View(announcements);
        }
    }
}
