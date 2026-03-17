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
                        ModelState.AddModelError(string.Empty, "該員工已離職。");
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
    }
}
    
