using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Staff_WebServer.Data;
using Staff_WebServer.Models;

namespace Staff_WebServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email и пароль обязательны для заполнения.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _context.AccessLogs.Add(new AccessLog
                {
                    UserName = email,
                    Action = "Вход в систему",
                    Timestamp = DateTime.Now
                });
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            else
            {
                ModelState.AddModelError("", "Неверный email или пароль.");
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var user = User.Identity?.Name;
    
            await _signInManager.SignOutAsync();

            if (user != null)
            {
                _context.AccessLogs.Add(new AccessLog
                {
                    UserName = user,
                    Action = "Выход из системы",
                    Timestamp = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Login", "Account");
        }
    }
}