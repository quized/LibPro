using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibPro.Models;

namespace LibPro.Controllers
{
    public class PublishersController : Controller
    {
        private readonly LibproContext _context;

        public PublishersController(LibproContext context)
        {
            _context = context;
        }

        // GET: Publishers
        public async Task<IActionResult> Index()
        {
            var libproContext = _context.Publishers.Include(p => p.City);
            return View(await libproContext.ToListAsync());
        }

        
        // GET: Publishers/Create
        public IActionResult Create()
        {
            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName");
            return PartialView("_CreatePub");
        }

        // POST: Publishers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Publishers publishers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(publishers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityID", publishers.CityID);
            return View(publishers);
        }

        // GET: Publishers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publishers = await _context.Publishers.FindAsync(id);
            if (publishers == null)
            {
                return NotFound();
            }
            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName", publishers.CityID);           
            return PartialView("_EditPub", publishers);
        }

        // POST: Publishers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("PubID,PubName,Address,Pubtel,CityID")] Publishers publishers)
        {
            if (id != publishers.PubID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publishers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublishersExists(publishers.PubID))
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
            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityID", publishers.CityID);
            return View(publishers);
        }

        

        // POST: Publishers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            bool hasBiblios = _context.Biblios.Any(b => b.PubID == id);
            if (hasBiblios)
            {
                return BadRequest("該出版社下有書籍，無法刪除。");
            }


            var publishers = await _context.Publishers.FindAsync(id);
            if (publishers != null)
            {
                _context.Publishers.Remove(publishers);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublishersExists(long id)
        {
            return _context.Publishers.Any(e => e.PubID == id);
        }
    }
}
