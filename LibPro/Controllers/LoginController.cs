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
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Patron"))
                {
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("StaffCenter", "Home");
            }

            return View();
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

            // 呼叫 BCrypt 比對密碼
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
            if (!isPasswordCorrect)
            {
                ModelState.AddModelError(string.Empty, "帳號或密碼錯誤，請重新登入。");
                return View(model);
            }

            user.LastLoginTime = DateTime.Now;
            await _context.SaveChangesAsync();


            string roleName = "Patron";
            if (user.UserRole != null)
            {
                roleName = user.UserRole.RoleName;
            }

            string realName = user.Account;

            // 建立 Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID),
                new Claim(ClaimTypes.Role, roleName)
            };

            if(roleName == "Staff")
            {
                var staffInfo = await _context.Staffs.FirstOrDefaultAsync(s => s.UserID == user.UserID);
                if (staffInfo != null)
                {
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

            // 智慧分流
            string action = "Index";
            if (roleName == "Admin" || roleName == "Staff")
                action = "StaffCenter";
            return RedirectToAction(action, "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("ManagerLogin");
            return RedirectToAction("Login", "Login");
        }
    }
}
    
