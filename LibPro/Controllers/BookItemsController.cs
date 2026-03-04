using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibPro.Models;
using Microsoft.Data.SqlClient;

namespace LibPro.Controllers
{
    public class BookItemsController : Controller
    {
        private readonly LibproContext _context;

        public BookItemsController(LibproContext context)
        {
            _context = context;
        }

        // GET: BookItems
        public async Task<IActionResult> Index(string searchString, byte? statusFilter)
        {
            ViewData["CurrentFilter"] = searchString;
            

            var lib = _context.BookItems.Include(b => b.Biblio).Include(b => b.ItemStatus).Include(b => b.Location).AsQueryable();
           
            if(statusFilter.HasValue)
            {
                lib = lib.Where(b => b.ItmStatus == statusFilter.Value);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                lib = lib.Where(b => b.Biblio.BTitle.Contains(searchString) || b.ItemID.Contains(searchString));
            }

            return View(await lib.ToListAsync());
        }

        // GET: BookItems/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookItems = await _context.BookItems
                .Include(b => b.Biblio)
                .Include(b => b.ItemStatus)
                .Include(b => b.Location)
                .FirstOrDefaultAsync(m => m.ItemID == id);
            if (bookItems == null)
            {
                return NotFound();
            }

            return View(bookItems);
        }

        // GET: BookItems/Create
        public IActionResult Create()
        {
            ViewData["BibID"] = new SelectList(_context.Biblios, "BibID", "BTitle");
            ViewData["ItmStatus"] = new SelectList(_context.ItemStatus, "StatusCode", "StatusName");
            ViewData["LocID"] = new SelectList(_context.Locations, "LocationID", "LocationName");
            return View();
        }

        // POST: BookItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( BookItems bookItems)
        {
            if (bookItems == null)
            {
                return NotFound();
            }

            ModelState.Remove("ItemID");

            if (ModelState.IsValid)
            {
                var itemIDResult = await _context.Database.SqlQuery<string>($"exec GetItemID").ToListAsync();
                var ItemID = itemIDResult.FirstOrDefault();


                if (string.IsNullOrWhiteSpace(ItemID))
                {
                    ModelState.AddModelError("", "產生 館藏編號 失敗，請聯絡管理員。");
                    ViewData["BibID"] = new SelectList(_context.Biblios, "BibID", "BTitle", bookItems.BibID);
                    ViewData["ItmStatus"] = new SelectList(_context.ItemStatus, "StatusCode", "StatusName", bookItems.ItmStatus);
                    ViewData["LocID"] = new SelectList(_context.Locations, "LocationID", "LocationName", bookItems.LocID);
                    return View(bookItems);
                }

                bookItems.ItemID = ItemID;

                _context.Add(bookItems);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BibID"] = new SelectList(_context.Biblios, "BibID", "BTitle", bookItems.BibID);
            ViewData["ItmStatus"] = new SelectList(_context.ItemStatus, "StatusCode", "StatusName", bookItems.ItmStatus);
            ViewData["LocID"] = new SelectList(_context.Locations, "LocationID", "LocationName", bookItems.LocID);
            return View(bookItems);
        }

        // GET: BookItems/Edit/5
        public async Task<IActionResult> Edit(string id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookItems = await _context.BookItems.FindAsync(id);
            if (bookItems == null)
            {
                return NotFound();
            }
            ViewData["BibID"] = new SelectList(_context.Biblios, "BibID", "BTitle", bookItems.BibID);
            ViewData["ItmStatus"] = new SelectList(_context.ItemStatus, "StatusCode", "StatusName", bookItems.ItmStatus);
            ViewData["LocID"] = new SelectList(_context.Locations, "LocationID", "LocationName", bookItems.LocID);
            ViewBag.ReturnUrl = returnUrl;
            return View(bookItems);
        }

        // POST: BookItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,BookItems bookItems,string returnUrl = null)
        {
            if (id != bookItems.ItemID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookItems);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookItemsExists(bookItems.ItemID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BibID"] = new SelectList(_context.Biblios, "BibID", "BTitle", bookItems.BibID);
            ViewData["ItmStatus"] = new SelectList(_context.ItemStatus, "StatusCode", "StatusName", bookItems.ItmStatus);
            ViewData["LocID"] = new SelectList(_context.Locations, "LocationID", "LocationName", bookItems.LocID);
            return View(bookItems);
        }

    

        // POST: BookItems/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var bookItems = await _context.BookItems.FindAsync(id);
            if (bookItems != null)
            {
                bookItems.ItmStatus = 6;
                _context.BookItems.Update(bookItems);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookItemsExists(string id)
        {
            return _context.BookItems.Any(e => e.ItemID == id);
        }
    }
}
