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
    public class DepartmentsController : Controller
    {
        private readonly LibproContext _context;

        public DepartmentsController(LibproContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Departments.ToListAsync());
        }

       
        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeptName")] Departments departments)
        {

            ModelState.Remove("DeptID");

            if (ModelState.IsValid)
            {
                var maxDeptID = await _context.Departments.OrderByDescending(d => d.DeptID).Select(d => d.DeptID).FirstOrDefaultAsync();
                string newDeptID = "D01";

                if (!string.IsNullOrEmpty(maxDeptID))
                {                    
               
                    int currentNum = int.Parse(maxDeptID.Substring(1));
                    int nextNum = currentNum + 1;


                    newDeptID = "D" + nextNum.ToString("D2");
                }

                departments.DeptID = newDeptID;

                _context.Add(departments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(departments);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departments = await _context.Departments.FindAsync(id);
            if (departments == null)
            {
                return NotFound();
            }
            return View(departments);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("DeptID,DeptName")] Departments departments)
        {
            if (id != departments.DeptID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(departments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentsExists(departments.DeptID))
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
            return View(departments);
        }

      

        private bool DepartmentsExists(string id)
        {
            return _context.Departments.Any(e => e.DeptID == id);
        }
    }
}
