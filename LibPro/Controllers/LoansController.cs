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
    public class LoansController : Controller
    {
        private readonly LibproContext _context;

        public LoansController(LibproContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            var libproContext = _context.Loans.Include(l => l.BookItem).Include(l => l.Patron);
            return View(await libproContext.ToListAsync());
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loans = await _context.Loans
                .Include(l => l.BookItem)
                .Include(l => l.Patron)
                .FirstOrDefaultAsync(m => m.LoanID == id);
            if (loans == null)
            {
                return NotFound();
            }

            return View(loans);
        }

        
        public IActionResult Create()
        {           
            ViewData["ItemID"] = new SelectList(_context.BookItems, "ItemID", "ItemID");
            ViewData["PatronID"] = new SelectList(_context.Patrons, "PatronID", "PatronID");
            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Loans loans)
        {
            ModelState.Remove("LoanID");
            ModelState.Remove("LoanDate");
            ModelState.Remove("DueDate");

            if (ModelState.IsValid)
            {
               
            

                var patron = await _context.Patrons.FindAsync(loans.PatronID);
                if (patron == null)
                {
                   
                    return Json(new { success = false, message = "找不到此借閱人，請確認證號是否正確。" });
                }

                var bookItem = await _context.BookItems.FindAsync(loans.ItemID);
                if (bookItem == null || bookItem.ItmStatus != 1)
                {
                    return Json(new { success = false, message = "此書籍不在架上，或已被借出。" });
                }

                var loanIDResult = await _context.Database.SqlQuery<string>($"exec getLoanID").ToListAsync();
                string newLoanID = loanIDResult.FirstOrDefault();
                if (string.IsNullOrEmpty(newLoanID))
                {
                    return Json(new { success = false, message = "無法產生借閱編號，請聯絡管理員。" });
                }

                loans.LoanID = newLoanID;
                loans.LoanDate = DateTime.Now;
                loans.DueDate = DateTime.Now.AddDays(14);
                loans.RenewalCount = 0;

                _context.Loans.Add(loans);
                bookItem.ItmStatus = 2;
                _context.BookItems.Update(bookItem);

                try
                {
                    await _context.SaveChangesAsync();

                   
                    return Json(new
                    {
                        success = true,
                        bookId = loans.ItemID,
                        dueDate = loans.DueDate.ToString("yyyy/MM/dd"),
                        time = loans.LoanDate.ToString("HH:mm:ss")
                    });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "系統錯誤：" + ex.Message });
                }
            }

            return Json(new { success = false, message = "資料格式錯誤，請重新掃描。" });
        }


        // GET: Loans/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loans = await _context.Loans.FindAsync(id);
            if (loans == null)
            {
                return NotFound();
            }
            ViewData["ItemID"] = new SelectList(_context.BookItems, "ItemID", "ItemID", loans.ItemID);
            ViewData["PatronID"] = new SelectList(_context.Patrons, "PatronID", "PatronID", loans.PatronID);
            return View(loans);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("LoanID,LoanDate,DueDate,ReturnDate,RenewalCount,Notes,PatronID,ItemID")] Loans loans)
        {
            if (id != loans.LoanID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loans);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoansExists(loans.LoanID))
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
            ViewData["ItemID"] = new SelectList(_context.BookItems, "ItemID", "ItemID", loans.ItemID);
            ViewData["PatronID"] = new SelectList(_context.Patrons, "PatronID", "PatronID", loans.PatronID);
            return View(loans);
        }


        private bool LoansExists(string id)
        {
            return _context.Loans.Any(e => e.LoanID == id);
        }
    }
}
