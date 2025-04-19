using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;
using Staff_WebServer.Models;

namespace Staff_WebServer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.Employees = _context.Employees
                .Where(e => !_context.Users.Any(u => u.ТабельныйНомер == e.Id))
                .OrderBy(e => e.FullName)
                .ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(string email, string password, int табельныйНомер)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email и пароль обязательны.");
                return View();
            }

            var employee = await _context.Employees
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Id == табельныйНомер);

            if (employee == null)
            {
                ModelState.AddModelError("", "Сотрудник не найден.");
                return View();
            }

            // 🧠 Определение роли по должности
            string role = employee.Position.Name.ToLower() switch
            {
                var s when s.Contains("директор") => "Director",
                var s when s.Contains("hr") => "HR",
                var s when s.Contains("менеджер") => "Admin",
                _ => "Employee"
            };
            
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                ТабельныйНомер = employee.Id
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRoleAsync(user, role);

                ViewBag.Message = $"Пользователь создан с ролью: {role}";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.Employees = _context.Employees
                .Where(e => !_context.Users.Any(u => u.ТабельныйНомер == e.Id))
                .ToList();

            return View();
        }
    }
}
