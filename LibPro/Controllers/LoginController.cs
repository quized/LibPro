using LibPro.Models;
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
                return RedirectToAction("Index","Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Account == model.Account);
           
            if (user != null)
            {
                // 呼叫 BCrypt 絞肉機比對密碼
                bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);

                if (isPasswordCorrect)
                {
                    //更新最後登入時間並存檔
                    user.LastLoginTime = DateTime.Now;
                    _context.UserAccounts.Update(user);
                    await _context.SaveChangesAsync();

                    //準備發放 VIP 手環 (Claims)
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.UserID),
                            new Claim(ClaimTypes.Name, user.Account),
                            new Claim(ClaimTypes.Role, user.UserType.ToString())
                        };

                    var claimsIdentity = new ClaimsIdentity(claims, "ManagerLogin");

                    //正式登入，發放名為 "ManagerLogin" 的 Cookie
                    await HttpContext.SignInAsync("ManagerLogin", new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
            }

            // 防駭客的模糊錯誤訊息
            ModelState.AddModelError(string.Empty, "帳號或密碼錯誤，請重新登入。");
        }
            return View(model);
        }

        // GET/POST: 處理登出邏輯
        public async Task<IActionResult> Logout()
        {
           
            await HttpContext.SignOutAsync("ManagerLogin");
            return RedirectToAction("Login", "Login");
        }
    }
}