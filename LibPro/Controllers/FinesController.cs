using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibPro.Models;
using LibPro.ViewModels;

namespace LibPro.Controllers
{
    public class FinesController : Controller
    {
        private readonly LibproContext _context;

        public FinesController(LibproContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string searchString, string status)
        {

            var finesQuery = _context.Fines
                .Include(f => f.FineType)
                .Include(f => f.Loan)
                    .ThenInclude(l => l.Patron)
                .AsQueryable();


            if (!string.IsNullOrEmpty(searchString))
            {
                finesQuery = finesQuery.Where(f => f.Loan.PatronID.Contains(searchString) || f.Loan.Patron.Name.Contains(searchString));
            }


            if (status == "unpaid")
            {
                finesQuery = finesQuery.Where(f => !f.ISPaid);
            }
            else if (status == "paid")
            {
                finesQuery = finesQuery.Where(f => f.ISPaid);
            }


            var finesData = await finesQuery
                .OrderBy(f => f.ISPaid)
                .ThenByDescending(f => f.CreatedDate)
                .ToListAsync();


            var viewModelList = new List<FineViewModel>();

            foreach (var f in finesData)
            {
                string pName = "未知讀者";
                string pId = "未知";
                string ftName = "未知類型";
                int overdueDays = 0;
                decimal amount = 0;

               
                if (f.FineType != null)
                {
                    ftName = f.FineType.FTName;
                    amount = f.FineType.UnitPrice;
                }

             
                if (f.Loan != null)
                {
                    pId = f.Loan.PatronID;
                    if (f.Loan.Patron != null)
                        pName = f.Loan.Patron.Name;
                }

                
                if (f.FTID == 1 && f.Loan != null && f.FineType != null)
                {
                    overdueDays = (f.CreatedDate.Date - f.Loan.DueDate.Date).Days;
                    if (overdueDays < 0)
                        overdueDays = 0;
                    amount = f.FineType.UnitPrice * overdueDays;
                }

                viewModelList.Add(new FineViewModel
                {
                    FineID = f.FineID,
                    CreatedDate = f.CreatedDate,
                    ISPaid = f.ISPaid,
                    FTName = ftName,
                    PatronID = pId,
                    PatronName = pName,
                    OverdueDays = overdueDays,
                    TotalAmount = amount
                });
            }


            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentStatus = status;


            return View(viewModelList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fine = await _context.Fines.FindAsync(id);

            if (fine == null)
            {
                return NotFound();
            }


            if (!fine.ISPaid)
            {
                fine.ISPaid = true;
                _context.Update(fine);
                await _context.SaveChangesAsync();


                TempData["SuccessMessage"] = $"罰金單號 {id} 已成功付款！";
            }


            return RedirectToAction(nameof(Index));

        }



        public IActionResult Create()
        {
            ViewData["FTID"] = new SelectList(_context.FineTypes, "FTID", "FTName");
            ViewData["LoanID"] = new SelectList(_context.Loans, "LoanID", "LoanID");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fines fines)
        {
           
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ISPaid");
            ModelState.Remove("FineID");

            if (ModelState.IsValid)
            {
                if (fines.LoanID != null)
                {

                    bool isDuplicate = await _context.Fines
                        .AnyAsync(f => f.LoanID == fines.LoanID && f.FTID == fines.FTID);

                    if (isDuplicate)
                    {
                        
                        ModelState.AddModelError("", "這筆借閱紀錄已經開立過「相同類型」的罰單了！");

                     
                        ViewData["FTID"] = new SelectList(_context.FineTypes, "FTID", "FTName", fines.FTID);
                        ViewData["LoanID"] = new SelectList(_context.Loans, "LoanID", "LoanID", fines.LoanID);
                        return View(fines);
                    }
                }

                fines.CreatedDate = DateTime.Now;

                fines.ISPaid = false;

                var FineIDResult = await _context.Database.SqlQuery<string>($"exec GetFineID").ToListAsync();
                var FineID = FineIDResult.FirstOrDefault();

                if (string.IsNullOrWhiteSpace(FineID))
                {
                    ModelState.AddModelError("", "產生 罰金編號 失敗，請聯絡管理員。");
                    ViewData["FTID"] = new SelectList(_context.FineTypes, "FTID", "FTName");
                    ViewData["LoanID"] = new SelectList(_context.Loans, "LoanID", "LoanID", fines.LoanID);
                    return View(fines);
                }

                fines.FineID = FineID;
                _context.Add(fines);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FTID"] = new SelectList(_context.FineTypes, "FTID", "FTName");
            ViewData["LoanID"] = new SelectList(_context.Loans, "LoanID", "LoanID", fines.LoanID);
            return View(fines);
        }

    }
}
