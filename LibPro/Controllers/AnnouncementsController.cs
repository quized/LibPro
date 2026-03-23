using LibPro.Models;
using LibPro.Services;
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
    [Authorize(Roles = "Staff")]
    public class AnnouncementsController : Controller
    {
        private readonly AnnService _annService;

        public AnnouncementsController(AnnService annService)
        {
            _annService = annService;
        }


        public async Task<IActionResult> Index()
        {
            var announcements = await _annService.GetAnnIndex();


            return View(announcements);
        }

        // GET: Announcements/Details/5
        public async Task<IActionResult> Details(string id)
        {

            var announcements = await _annService.GetAnnDetails(id);

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

                bool result = await _annService.GetAnnCreate(announcements, currentStaffID);

                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }


            }

            ModelState.AddModelError("", "公告建立失敗，請聯絡管理員。");
            return View(announcements);
        }

        // GET: Announcements/Edit/5
        public async Task<IActionResult> Edit(string id)
        {           

            var announcements = await _annService.GetAnnEditView(id);
          
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
                    bool editResult = await _annService.GetAnnEdit(id, announcements);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_annService.AnnouncementsExists(announcements.AnnID))
                    {
                        return NotFound();
                    }
                        throw;
                    
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

            bool isDeleted = await _annService.GetAnnDelete(id);

            if (!isDeleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        
    }
}
