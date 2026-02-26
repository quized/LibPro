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

        // GET: Loans/Create
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
        public async Task<IActionResult> Create([Bind("LoanID,LoanDate,DueDate,ReturnDate,RenewalCount,Notes,PatronID,ItemID")] Loans loans)
        {
            ModelState.Remove("LoanID");
            ModelState.Remove("LoanDate");
            ModelState.Remove("DueDate");

            if (ModelState.IsValid)
            {

                _context.Add(loans);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemID"] = new SelectList(_context.BookItems, "ItemID", "ItemID", loans.ItemID);
            ViewData["PatronID"] = new SelectList(_context.Patrons, "PatronID", "PatronID", loans.PatronID);
            return View(loans);
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
