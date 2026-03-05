using LibPro.Models;
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
    public class AnnouncementsController : Controller
    {
        private readonly LibproContext _context;

        public AnnouncementsController(LibproContext context)
        {
            _context = context;
        }

        // GET: Announcements
        public async Task<IActionResult> Index()
        {
            var libproContext = _context.Announcements.Include(a => a.Staff);
            return View(await libproContext.ToListAsync());
        }

        // GET: Announcements/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var announcements = await _context.Announcements
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(m => m.AnnID == id);
            if (announcements == null)
            {
                return NotFound();
            }

            return View(announcements);
        }

        // GET: Announcements/Create
        public IActionResult Create()
        {
            ViewData["Creator"] = new SelectList(_context.Staffs, "StaffID", "StaffID");
            return View();
        }

        // POST: Announcements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Announcements announcements)
        {
            ModelState.Remove("AnnID");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("IsVisible");
            ModelState.Remove("Creator");

            if (ModelState.IsValid)
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized("無法取得登入者資訊，請重新登入。");
                }


                var annIDResult = await _context.Database.SqlQuery<string>($"exec GetAnnID").ToListAsync();
                var newAnnID = annIDResult.FirstOrDefault();

                if (string.IsNullOrWhiteSpace(newAnnID))
                {
                    ModelState.AddModelError("", "產生 公告編號 失敗，請聯絡管理員。");
                    ViewData["Creator"] = new SelectList(_context.Staffs, "StaffID", "StaffID", announcements.Creator);
                    return View(announcements);
                }

                announcements.AnnID = newAnnID;
                announcements.CreatedDate = DateTime.Now;
                announcements.IsVisible = true;
                announcements.Creator = currentUserId;

                _context.Add(announcements);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Creator"] = new SelectList(_context.Staffs, "StaffID", "StaffID", announcements.Creator);
            return View(announcements);
        }

        // GET: Announcements/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var announcements = await _context.Announcements.FindAsync(id);
            if (announcements == null)
            {
                return NotFound();
            }
            ViewData["Creator"] = new SelectList(_context.Staffs, "StaffID", "StaffID", announcements.Creator);
            return View(announcements);
        }

        // POST: Announcements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AnnID,Title,Content,CreatedDate,Creator,IsVisible")] Announcements announcements)
        {
            if (id != announcements.AnnID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(announcements);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnouncementsExists(announcements.AnnID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Creator"] = new SelectList(_context.Staffs, "StaffID", "StaffID", announcements.Creator);
            return View(announcements);
        }

        // GET: Announcements/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var announcements = await _context.Announcements
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(m => m.AnnID == id);
            if (announcements == null)
            {
                return NotFound();
            }

            return View(announcements);
        }

        // POST: Announcements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var announcements = await _context.Announcements.FindAsync(id);
            if (announcements != null)
            {
                _context.Announcements.Remove(announcements);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnnouncementsExists(string id)
        {
            return _context.Announcements.Any(e => e.AnnID == id);
        }
    }
}
