using LibPro.Models;
using LibPro.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibPro.Controllers
{

    public class LoginController : Controller
    {
        private readonly LibproContext _context;

        public LoginController(LibproContext context)
        {
            _context = context;
        }


        public IActionResult login()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }

            if (User.IsInRole("Patron"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "UserAccounts");
            }

            return RedirectToAction("StaffCenter", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.UserAccounts
                                     .Include(u => u.UserRole)
                                     .FirstOrDefaultAsync(u => u.Account == model.Account);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "帳號或密碼錯誤，請重新登入。");
                return View(model);
            }

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
            if (!isPasswordCorrect)
            {
                ModelState.AddModelError(string.Empty, "帳號或密碼錯誤，請重新登入。");
                return View(model);
            }

            user.LastLoginTime = DateTime.Now;
            await _context.SaveChangesAsync();

            string roleName = "Patron";
            string realName = user.Account;


            if (user.UserRole != null)
            {
                roleName = user.UserRole.RoleName;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID),
                new Claim(ClaimTypes.Role, roleName)
            };

            if (roleName == "Staff" || roleName == "Admin")
            {
                var staffInfo = await _context.Staffs.FirstOrDefaultAsync(s => s.UserID == user.UserID);
                if (staffInfo != null)
                {

                    if (staffInfo.IsResigned)
                    {
                        ModelState.AddModelError(string.Empty, "該員工帳號已被註銷。");
                        return View(model);
                    }

                    claims.Add(new Claim("StaffID", staffInfo.StaffID));
                    realName = staffInfo.Name;
                }
            }

            if (roleName == "Patron")
            {
                var patronsInfo = await _context.Patrons.FirstOrDefaultAsync(s => s.UserID == user.UserID);
                if (patronsInfo != null)
                {
                    claims.Add(new Claim("PatronID", patronsInfo.PatronID));
                    realName = patronsInfo.Name;
                }
            }

            claims.Add(new Claim(ClaimTypes.Name, realName));

            var claimsIdentity = new ClaimsIdentity(claims, "ManagerLogin");
            await HttpContext.SignInAsync("ManagerLogin", new ClaimsPrincipal(claimsIdentity));

            if (roleName == "Patron")
            {
                return RedirectToAction("Index", "Home");
            }

            if (roleName == "Admin")
            {
                return RedirectToAction("Index", "UserAccounts");
            }

            return RedirectToAction("StaffCenter", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("ManagerLogin");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAPI(string oldPassword, string newPassword, string confirmPassword)
        {

            if (newPassword != confirmPassword)
            {
                return Json(new { success = false, message = "兩次輸入的新密碼不一致！" });
            }

            if (newPassword.Length < 8)
            {
                return Json(new { success = false, message = "新密碼長度不能小於 8 個字元！" });
            }

            var currentUserId = User.Identity!.Name;
            var account = await _context.UserAccounts.FindAsync(currentUserId);

            if (account == null)
            {
                return Json(new { success = false, message = "系統錯誤，找不到該帳號狀態！" });
            }

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, account.Password))
            {
                return Json(new { success = false, message = "原密碼輸入錯誤，請確認！" });
            }
            //加密並更新新密碼
            account.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            try
            {
                _context.Update(account);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "密碼修改成功！系統將在 3 秒後自動登出，請使用新密碼重新登入。" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "資料庫更新失敗，請聯繫系統管理員。" });
            }
        }

    }
}

