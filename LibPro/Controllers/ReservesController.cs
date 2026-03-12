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
    [Authorize(Roles = "Patron")]
    public class ReservesController : Controller
    {
        private readonly LibproContext _context;

        public ReservesController(LibproContext context)
        {
            _context = context;
        }

        // GET: Reserves
        public async Task<IActionResult> Index()
        {
            var libproContext = _context.Reserves.Include(r => r.BookItem).Include(r => r.Patron).Include(r => r.ReserveStatus);
            return View(await libproContext.ToListAsync());
        }



      



        // POST: Reserves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string itemID)
        {
            if (string.IsNullOrEmpty(itemID))
            {
                return NotFound();
            }


            var PatronID = User.FindFirstValue("PatronID");

            if (string.IsNullOrEmpty(PatronID))
            {
                return RedirectToAction("Logout", "Login");
            }

            var bookItem = await _context.BookItems
            .Include(b => b.Biblio)
            .FirstOrDefaultAsync(b => b.ItemID == itemID);

            if (bookItem == null)
            {
                return NotFound();
            }


            if (bookItem.ItmStatus != 1)
            {
                TempData["ErrorBookItemMessage"] = "書籍狀態不正確！";
                return RedirectToAction("BookDetails", "Home", new { id = bookItem.BibID });
            }

            var existingReserve = await _context.Reserves
                .FirstOrDefaultAsync(r => r.PatronID == PatronID && r.ItemID == itemID && r.ResStatus == 1);

            if (existingReserve != null)
            {
                TempData["WarningMessage"] = "您已經預約過這本館藏了！";
                return RedirectToAction("BookDetails", "Home", new { id = bookItem.BibID });
            }

            var resIDResult = await _context.Database.SqlQuery<string>($"exec GetResID").ToListAsync();
            var newResID = resIDResult.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(newResID))
            {
                TempData["ErrorResIDMessage"] = "產生預約編號失敗！";
                return RedirectToAction("BookDetails", "Home", new { id = bookItem.BibID });
            }



            var reserve = new Reserves
            {
                ResID = newResID,
                PatronID = PatronID,
                ItemID = itemID,
                ResDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(7),
                ResStatus = 1,
                Notes = "前台線上預約"
            };

            _context.Reserves.Add(reserve);

            bookItem.ItmStatus = 3;
            _context.Update(bookItem);


           
            await _context.SaveChangesAsync();

           
            TempData["SuccessMessage"] = $"預約成功！您已保留《{bookItem.Biblio.BTitle}》，請留意取書通知。";
            return RedirectToAction("BookDetails", "Home", new { id = bookItem.BibID });
        }
    


// GET: Reserves/Edit/5
public async Task<IActionResult> Edit(string id)
{
    if (id == null)
    {
        return NotFound();
    }

    var reserves = await _context.Reserves.FindAsync(id);
    if (reserves == null)
    {
        return NotFound();
    }
    ViewData["ItemID"] = new SelectList(_context.BookItems, "ItemID", "ItemID", reserves.ItemID);
    ViewData["PatronID"] = new SelectList(_context.Patrons, "PatronID", "PatronID", reserves.PatronID);
    ViewData["ResStatus"] = new SelectList(_context.ReserveStatus, "StatusCode", "StatusName", reserves.ResStatus);
    return View(reserves);
}

// POST: Reserves/Edit/5
// To protect from overposting attacks, enable the specific properties you want to bind to.
// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(string id, [Bind("ResID,ResDate,ExpiryDate,Notes,ResStatus,PatronID,ItemID")] Reserves reserves)
{
    if (id != reserves.ResID)
    {
        return NotFound();
    }

    if (ModelState.IsValid)
    {
        try
        {
            _context.Update(reserves);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReservesExists(reserves.ResID))
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
    ViewData["ItemID"] = new SelectList(_context.BookItems, "ItemID", "ItemID", reserves.ItemID);
    ViewData["PatronID"] = new SelectList(_context.Patrons, "PatronID", "PatronID", reserves.PatronID);
    ViewData["ResStatus"] = new SelectList(_context.ReserveStatus, "StatusCode", "StatusName", reserves.ResStatus);
    return View(reserves);
}


// POST: Reserves/Delete/5
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(string id)
{
    var reserves = await _context.Reserves.FindAsync(id);
    if (reserves != null)
    {
        _context.Reserves.Remove(reserves);
    }

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}

private bool ReservesExists(string id)
{
    return _context.Reserves.Any(e => e.ResID == id);
}
    }
}
