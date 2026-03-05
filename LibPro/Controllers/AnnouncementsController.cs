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
    //[Authorize(Roles = "Staff")]
    public class AnnouncementsController : Controller
    {
        private readonly LibproContext _context;

        public AnnouncementsController(LibproContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            // 直接撈出所有資料，並依日期排序
            var announcements = await _context.Announcements
                .Include(a => a.Staff)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();

            return View(announcements);
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
                var currentStaffID = User.FindFirstValue("StaffID");

                if (string.IsNullOrEmpty(currentStaffID))
                {
                    return RedirectToAction("Logout", "Login");
                }

              


                var annIDResult = await _context.Database.SqlQuery<string>($"exec GetAnnID").ToListAsync();
                var newAnnID = annIDResult.FirstOrDefault();

                if (string.IsNullOrWhiteSpace(newAnnID))
                {
                    ModelState.AddModelError("", "產生 公告編號 失敗，請聯絡管理員。");
                    return View(announcements);
                }

                announcements.AnnID = newAnnID;
                announcements.CreatedDate = DateTime.Now;
                announcements.IsVisible = true;
                announcements.Creator = currentStaffID;

                _context.Add(announcements);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           
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
            return View(announcements);
        }

        // POST: Announcements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Announcements announcements)
        {
            if (id != announcements.AnnID)
            {
                return NotFound();
            }

            ModelState.Remove("CreatedDate");
            ModelState.Remove("Creator");


            if (ModelState.IsValid)
            {
                try
                {
                    var existingAnn = await _context.Announcements.FindAsync(id);
                    if (existingAnn == null)
                    {
                        return NotFound();
                    }

                    existingAnn.Title = announcements.Title;
                    existingAnn.Content = announcements.Content;
                    existingAnn.IsVisible = announcements.IsVisible;

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
            return View(announcements);
        }


        // POST: Announcements/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var announcements = await _context.Announcements.FindAsync(id);
            if (announcements != null)
            {

                announcements.IsVisible = false;
                _context.Announcements.Update(announcements);
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
