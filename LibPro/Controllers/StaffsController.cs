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
    public class StaffsController : Controller
    {
        private readonly LibproContext _context;

        public StaffsController(LibproContext context)
        {
            _context = context;
        }

        // GET: Staffs
        public async Task<IActionResult> Index(bool isresignde = false)
        {
            var libproContext = _context.Staffs.Include(s => s.City).Include(s => s.Department).Include(s => s.UserAccount).AsQueryable(); ;

            libproContext = libproContext.Where(s => s.IsResigned == isresignde);



            return View(await libproContext.ToListAsync());
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs
                .Include(s => s.City)
                .Include(s => s.Department)
                .Include(s => s.UserAccount)
                .FirstOrDefaultAsync(m => m.StaffID == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staffs/Create
        public IActionResult Create()
        {
            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName");
            ViewData["DeptID"] = new SelectList(_context.Departments, "DeptID", "DeptName");
            ViewData["UserID"] = new SelectList(_context.UserAccounts, "UserID", "UserName");
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff)
        {

            ModelState.Remove("StaffID");

            if (ModelState.IsValid)
            {
                var StaffID = await _context.Database.SqlQuery<string>($"exec GetStaffID").ToListAsync();
                staff.StaffID = StaffID.FirstOrDefault();

                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName", staff.CityID);
            ViewData["DeptID"] = new SelectList(_context.Departments, "DeptID", "DeptName", staff.DeptID);
            ViewData["UserID"] = new SelectList(_context.UserAccounts, "UserID", "UserName", staff.UserID);
            return View(staff);
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName", staff.CityID);
            ViewData["DeptID"] = new SelectList(_context.Departments, "DeptID", "DeptName", staff.DeptID);
            ViewData["UserID"] = new SelectList(_context.UserAccounts, "UserID", "UserName", staff.UserID);
            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("StaffID,Name,Education,Birthday,Gender,Email,Phone,Address,ZipCode,IsResigned,UserID,CityID,DeptID")] Staff staff)
        {
            if (id != staff.StaffID)
            {
                return NotFound();
            }

            ModelState.Remove("StaffID");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.StaffID))
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
            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName", staff.CityID);
            ViewData["DeptID"] = new SelectList(_context.Departments, "DeptID", "DeptName", staff.DeptID);
            ViewData["UserID"] = new SelectList(_context.UserAccounts, "UserID", "UserName", staff.UserID);
            return View(staff);
        }



        // POST: Staffs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff != null)
            {
                staff.IsResigned = true;
                _context.Staffs.Update(staff);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Restore(string id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff != null)
            {
                staff.IsResigned = false;
                _context.Staffs.Update(staff);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(string id)
        {
            return _context.Staffs.Any(e => e.StaffID == id);
        }
    }
}
