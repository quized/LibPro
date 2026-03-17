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
    public class UserAccountsController : Controller
    {
        private readonly LibproContext _context;

        public UserAccountsController(LibproContext context)
        {
            _context = context;
        }

        // GET: UserAccounts
        public async Task<IActionResult> Index()
        {
            var accounts = await _context.UserAccounts.Include(u => u.UserRole).ToListAsync();
            return View(accounts);
        }

      

        // POST: UserAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string UserID, string Account, string? NewPassword, string? ConfirmPassword)
        {
          
            var userAccount = await _context.UserAccounts.FindAsync(UserID);
            if (userAccount == null)
            {
                TempData["ErrorMessage"] = "找不到該帳號！";
                return RedirectToAction(nameof(Index));
            }

            //更新帳號
            if (string.IsNullOrWhiteSpace(Account) || Account.Length < 8)
            {
                TempData["ErrorMessage"] = "帳號不能為空，且必須至少 8 個字元！";
                return RedirectToAction(nameof(Index));
            }
            userAccount.Account = Account;

            //處理密碼邏輯
            if (!string.IsNullOrEmpty(NewPassword))
            {
                if (NewPassword.Length < 8)
                {
                    TempData["ErrorMessage"] = "新密碼長度不能小於 8 個字元！";
                    return RedirectToAction(nameof(Index));
                }

                if (NewPassword != ConfirmPassword)
                {
                    TempData["ErrorMessage"] = "兩次輸入的密碼不一致，請重新輸入！";
                    return RedirectToAction(nameof(Index));
                }

                userAccount.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            }

            try
            {
                _context.Update(userAccount);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"帳號 {UserID} 資料已成功更新！";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "更新失敗，可能帳號發生重複或其他錯誤。";
            }

            return RedirectToAction(nameof(Index));
        }

       
    }
}
