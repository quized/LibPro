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

    public class ReservesController : Controller
    {
        private readonly LibproContext _context;

        public ReservesController(LibproContext context)
        {
            _context = context;
        }

        // GET: Reserves
        public async Task<IActionResult> Index(string searchString)
        {
     
            ViewData["CurrentFilter"] = searchString;

           
            var reservesQuery = _context.Reserves
                .Include(r => r.BookItem)
                .Include(r => r.Patron)
                .Include(r => r.ReserveStatus)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                reservesQuery = reservesQuery.Where(r =>
                    r.ResID.Contains(searchString) ||
                    r.Patron.Name.Contains(searchString) ||
                    r.ItemID.Contains(searchString)
                );
            }

      
            reservesQuery = reservesQuery.OrderByDescending(r => r.ResDate);

            
            return View(await reservesQuery.ToListAsync());
        }







        [Authorize(Roles = "Patron")]
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
            
            var bookItem = await _context.BookItems.Include(b => b.Biblio).FirstOrDefaultAsync(b => b.ItemID == itemID);

            if (bookItem == null)
            {
                return NotFound();
            }

            if (bookItem.ItmStatus != 1)
            {
                TempData["ErrorBookItemMessage"] = "書籍狀態不正確！";
                return RedirectToAction("BookDetails", "Home", new { id = bookItem.BibID });
            }

            int maxReserveLimit = 5;
            int currentReserveCount = await _context.Reserves.CountAsync(r => r.PatronID == PatronID && (r.ResStatus == 1 || r.ResStatus == 2));

            if (currentReserveCount >= maxReserveLimit)
            {
                TempData["WarningMessage"] = $"您的預約數量已達上限 ({maxReserveLimit} 本)！請先取消其他預約或完成取書。";
                return RedirectToAction("BookDetails", "Home", new { id = bookItem.BibID });
            }

        
            bool hasReservedSameBook = await _context.Reserves
                .AnyAsync(r => r.PatronID == PatronID && (r.ResStatus == 1 || r.ResStatus == 2)
                    && r.BookItem.BibID == bookItem.BibID);

            if (hasReservedSameBook)
            {
                TempData["WarningMessage"] = "您已經預約過這本書了（請留意取書通知），不可重複預約！";
                return RedirectToAction("BookDetails", "Home", new { id = bookItem.BibID });
            }

          
            bool hasBorrowedSameBook = await _context.Loans
                .AnyAsync(l => l.PatronID == PatronID && l.ReturnDate == null
                    && l.BookItem.BibID == bookItem.BibID);

            if (hasBorrowedSameBook)
            {
                TempData["WarningMessage"] = "您目前已經借閱了這本書，尚未歸還前不可預約！";
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(string id, [Bind("ResID,ExpiryDate,Notes,ResStatus")] Reserves reservesUpdate)
        {
            if (id != reservesUpdate.ResID)
                return NotFound();

            ModelState.Remove("PatronID");
            ModelState.Remove("ItemID");
            ModelState.Remove("ResDate");


            if (!ModelState.IsValid)
            {

                ViewData["ResStatus"] = new SelectList(_context.ReserveStatus, "StatusCode", "StatusName", reservesUpdate.ResStatus);
                return View(reservesUpdate);
            }

            try
            {
                var existingReserve = await _context.Reserves
                    .Include(r => r.BookItem)
                    .FirstOrDefaultAsync(r => r.ResID == id);

                if (existingReserve == null)
                {
                    return NotFound();
                }

                if (new[] { 3, 4 }.Contains(reservesUpdate.ResStatus) && existingReserve.ResStatus != reservesUpdate.ResStatus)
                {
                    if (existingReserve.BookItem != null)
                    {
                        existingReserve.BookItem.ItmStatus = 1;
                    }
                }

                if (new[] { 1, 2 }.Contains(reservesUpdate.ResStatus) && existingReserve.ResStatus != reservesUpdate.ResStatus)
                {
                    if (existingReserve.BookItem == null)
                    {
                        ModelState.AddModelError("", "找不到對應的館藏資料！");
                        ViewData["ResStatus"] = new SelectList(_context.ReserveStatus, "StatusCode", "StatusName", reservesUpdate.ResStatus);
                        return View(reservesUpdate);
                    }

                    if (existingReserve.BookItem.ItmStatus != 1)
                    {
                        ModelState.AddModelError("", "無法恢復預約，因為該實體書已被借出或不在架上！");
                        ViewData["ResStatus"] = new SelectList(_context.ReserveStatus, "StatusCode", "StatusName", reservesUpdate.ResStatus);
                        return View(reservesUpdate);
                    }

                    existingReserve.BookItem.ItmStatus = 3;
                }

                existingReserve.ResStatus = reservesUpdate.ResStatus;
                existingReserve.ExpiryDate = reservesUpdate.ExpiryDate;
                existingReserve.Notes = reservesUpdate.Notes;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "預約狀態已成功更新！";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservesExists(reservesUpdate.ResID))
                    return NotFound();
                throw;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }


            var reserves = await _context.Reserves.Include(r => r.BookItem).FirstOrDefaultAsync(r => r.ResID == id);


            if (reserves == null || reserves.BookItem == null)
            {
                return NotFound();
            }



            reserves.ResStatus = 4;

            reserves.BookItem.ItmStatus = 1;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "預約已成功註銷，館藏已釋放！";


            return RedirectToAction(nameof(Index));
        }

        private bool ReservesExists(string id)
        {
            return _context.Reserves.Any(e => e.ResID == id);
        }
    }
}
