using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;
using Staff_WebServer.Models;

namespace Staff_WebServer.Controllers;

[Authorize]
public class EmployeeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public EmployeeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);

        var employee = await _context.Employees
            .Include(e => e.Nationality)
            .Include(e => e.Education)
            .Include(e => e.Position)
            .Include(e => e.Department)
            .Include(e => e.PensionFund)
            .FirstOrDefaultAsync(e => e.Id == user.ТабельныйНомер);

        if (employee == null) return NotFound();

        ViewBag.Email = user.Email;
        return View(employee);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        if (!ModelState.IsValid) return RedirectToAction("Profile");

        var user = await _userManager.GetUserAsync(User);
        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

        if (result.Succeeded)
        {
            TempData["Message"] = "Пароль успешно изменён.";
        }
        else
        {
            TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
        }

        return RedirectToAction("Profile");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(Employee updated)
    {
        var user = await _userManager.GetUserAsync(User);
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == user.ТабельныйНомер);

        if (employee == null) return NotFound();

        // Только разрешённые поля
        employee.Address = updated.Address;
        employee.Dependents = updated.Dependents;

        await _context.SaveChangesAsync();
        ViewBag.Message = "Данные успешно обновлены!";
        return RedirectToAction("Profile");
    }
    
    public async Task<IActionResult> Orders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || user.ТабельныйНомер == 0)
        {
            return Unauthorized();
        }

        var orders = await _context.Orders
            .Include(o => o.OrderType)
            .Where(o => o.EmployeeId == user.ТабельныйНомер)
            .OrderByDescending(o => o.Date)
            .ToListAsync();

        return View(orders);
    }
}