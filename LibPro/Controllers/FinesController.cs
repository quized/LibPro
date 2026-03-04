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

                //檢查 Loan 與 Patron 資訊，並計算逾期天數
                if (f.Loan != null)
                {
                    pId = f.Loan.PatronID; 

                    if (f.Loan.Patron != null)
                    {
                        pName = f.Loan.Patron.Name; 
                    }

                    // 計算逾期天數
                    overdueDays = (f.CreatedDate.Date - f.Loan.DueDate.Date).Days;

                    if (overdueDays < 0)
                    {
                        overdueDays = 0;
                    }
                }

                // 檢查 FineType 資訊，並計算總金額
                if (f.FineType != null)
                {
                    ftName = f.FineType.FTName; // 取得罰金類型名稱
                    amount = f.FineType.UnitPrice * overdueDays; // 計算總應收金額
                }

                // 將確認過的安全資料裝進 ViewModel
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
            if(id == null)
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

    }
}
