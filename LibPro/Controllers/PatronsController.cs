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
    public class PatronsController : Controller
    {
        private readonly LibproContext _context;

        public PatronsController(LibproContext context)
        {
            _context = context;
        }

        // GET: Patrons
        public async Task<IActionResult> Index(byte? statusFilter)
        {
            ViewData["StatusFilter"] = new SelectList(_context.PatronsStatus, "StatusCode", "StatusName", statusFilter);
            var libproContext = _context.Patrons
                 .Include(p => p.City)
                 .Include(p => p.PatronsStatus)
                 .AsQueryable();

            if (statusFilter.HasValue)
            {
                libproContext = libproContext.Where(p => p.PtrStatus == statusFilter.Value);
            }


            return View(await libproContext.ToListAsync());
        }

        // GET: Patrons/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patrons = await _context.Patrons
                .Include(p => p.City)
                .Include(p => p.PatronsStatus)
                .Include(p => p.UserAccount)
                .FirstOrDefaultAsync(m => m.PatronID == id);
            if (patrons == null)
            {
                return NotFound();
            }

            return View(patrons);
        }

        // GET: Patrons/Create
        public IActionResult Create()
        {
            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName");
            ViewData["PtrStatus"] = new SelectList(_context.PatronsStatus, "StatusCode", "StatusName");
            ViewData["UserID"] = new SelectList(_context.UserAccounts, "UserID", "UserName");
            return View();
        }

        // POST: Patrons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patrons patrons)
        {

            ModelState.Remove("PatronID");

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var patronID = await _context.Database.SqlQuery<string>($"exec GetPatronsID").ToListAsync();
                    string newPatronID = patronID.FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(newPatronID))
                    {
                        ModelState.AddModelError("", "產生 PatronID 失敗，請聯絡管理員。");
                        ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName", patrons.CityID);
                        ViewData["PtrStatus"] = new SelectList(_context.PatronsStatus, "StatusCode", "StatusName", patrons.PtrStatus);
                        ViewData["UserID"] = new SelectList(_context.UserAccounts, "UserID", "UserName", patrons.UserID);
                        return View(patrons);
                    }

                    string newUserID = "L" + newPatronID;

                    string loginAccount = patrons.Phone;



                    string loginPassword = patrons.Birthday.ToString("yyyyMMdd");

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginPassword);

                    var newUserAccount = new UserAccounts
                    {
                        UserID = newUserID,
                        Account = loginAccount,
                        Password = hashedPassword,
                        CreatedDate = DateTime.Now,
                        UserType = 3
                    };
                    _context.UserAccounts.Add(newUserAccount);

                    patrons.PatronID = newPatronID;
                    patrons.UserID = newUserID;
                    _context.Add(patrons);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "新增失敗，系統發生錯誤：" + ex.Message);
                }


            }
            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName", patrons.CityID);
            ViewData["PtrStatus"] = new SelectList(_context.PatronsStatus, "StatusCode", "StatusName", patrons.PtrStatus);
            ViewData["UserID"] = new SelectList(_context.UserAccounts, "UserID", "UserName", patrons.UserID);
            return View(patrons);
        }

        // GET: Patrons/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patrons = await _context.Patrons.FindAsync(id);
            if (patrons == null)
            {
                return NotFound();
            }
            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName", patrons.CityID);
            ViewData["PtrStatus"] = new SelectList(_context.PatronsStatus, "StatusCode", "StatusName", patrons.PtrStatus);
            ViewData["UserID"] = new SelectList(_context.UserAccounts, "UserID", "UserName", patrons.UserID);
            return View(patrons);
        }

        // POST: Patrons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PatronID,Name,Education,Birthday,Gender,Email,Phone,Profession,District,Address,ZipCode,Memo,UserID,CityID,PtrStatus")] Patrons patrons)
        {
            if (id != patrons.PatronID)
            {
                return NotFound();
            }


            ModelState.Remove("PatronID");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patrons);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatronsExists(patrons.PatronID))
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
            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityID", patrons.CityID);
            ViewData["PtrStatus"] = new SelectList(_context.PatronsStatus, "StatusCode", "StatusName", patrons.PtrStatus);
            ViewData["UserID"] = new SelectList(_context.UserAccounts, "UserID", "UserID", patrons.UserID);
            return View(patrons);
        }


        // POST: Patrons/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(string id, byte status)
        {
            var patrons = await _context.Patrons.FindAsync(id);
            if (patrons != null)
            {
                patrons.PtrStatus = status;
                _context.Patrons.Update(patrons);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool PatronsExists(string id)
        {
            return _context.Patrons.Any(e => e.PatronID == id);
        }
    }
}
