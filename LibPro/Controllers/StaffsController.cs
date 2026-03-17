using LibPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibPro.Controllers
{
    [Authorize(Roles = "Admin")]
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
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff, byte userType)
        {
            ModelState.Remove("StaffID");

            if (ModelState.IsValid)
            {

                var staffIDResult = await _context.Database.SqlQuery<string>($"exec GetStaffID").ToListAsync();
                var newStaffID = staffIDResult.FirstOrDefault();

                if (string.IsNullOrEmpty(newStaffID))
                {

                    ModelState.AddModelError("", "產生 員工ID 失敗，請聯絡管理員。");
                    ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName", staff.CityID);
                    ViewData["DeptID"] = new SelectList(_context.Departments, "DeptID", "DeptName", staff.DeptID);
                    return View(staff);
                }

                string newUserID = "L" + newStaffID;
                string loginAccount = staff.Phone;
                string loginPassword = staff.Birthday.ToString("yyyyMMdd");
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginPassword);

                var newUserAccount = new UserAccounts
                {
                    UserID = newUserID,
                    Account = loginAccount,
                    Password = hashedPassword,
                    CreatedDate = DateTime.Now,
                    UserType = userType
                };

                _context.UserAccounts.Add(newUserAccount);

                staff.StaffID = newStaffID;
                staff.UserID = newUserID;
                _context.Add(staff);


                try
                {

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", "新增員工失敗: " + ex.Message);
                }
            }


            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName", staff.CityID);
            ViewData["DeptID"] = new SelectList(_context.Departments, "DeptID", "DeptName", staff.DeptID);
            return View(staff);
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs.Include(s => s.UserAccount).FirstOrDefaultAsync(m => m.StaffID == id);

            if (staff == null)
            {
                return NotFound();
            }


            var userTypeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "2", Text = "一般館員 (僅能操作流通、預約與基礎業務)" },
                new SelectListItem { Value = "1", Text = "系統管理員 (可異動人事、部門與系統設定)" }
            };
            string currentUserType = staff.UserAccount.UserType.ToString();

           
            ViewBag.UserTypeList = new SelectList(userTypeOptions, "Value", "Text", currentUserType);

            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName", staff.CityID);
            ViewData["DeptID"] = new SelectList(_context.Departments, "DeptID", "DeptName", staff.DeptID);

            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("StaffID,Name,Education,Birthday,Gender,Email,Phone,Address,ZipCode,IsResigned,UserID,CityID,DeptID")] Staff staff, byte userType)
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

                    var userAccount = await _context.UserAccounts.FirstOrDefaultAsync(u => u.UserID == id);
                    if (userAccount != null)
                    {
                        userAccount.UserType = userType;
                        _context.Update(userAccount);
                    }

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

            var userTypeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "2", Text = "一般館員 (僅能操作流通、預約與基礎業務)" },
                new SelectListItem { Value = "1", Text = "系統管理員 (可異動人事、部門與系統設定)" }
            };
            string currentUserType = staff.UserAccount.UserType.ToString();


            ViewBag.UserTypeList = new SelectList(userTypeOptions, "Value", "Text", currentUserType);

            ViewData["CityID"] = new SelectList(_context.Cities, "CityID", "CityName", staff.CityID);
            ViewData["DeptID"] = new SelectList(_context.Departments, "DeptID", "DeptName", staff.DeptID);
            return View(staff);
        }



        // POST: Staffs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleResignStatus(string id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff != null)
            {
                staff.IsResigned = !staff.IsResigned;
                _context.Staffs.Update(staff);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction(nameof(Index));
        }



        private bool StaffExists(string id)
        {
            return _context.Staffs.Any(e => e.StaffID == id);
        }
    }
}
