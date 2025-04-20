using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;

namespace Staff_WebServer.Controllers;

[Authorize(Roles = "HR")]
public class HRController : Controller
{
    private readonly ApplicationDbContext _context;

    public HRController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Employees()
    {
        var employees = _context.Employees
            .Include(e => e.Position)
            .Include(e => e.Department)
            .Include(e => e.Education)
            .Include(e => e.PensionFund)
            .Include(e => e.Nationality)
            .ToList();

        return View(employees);
    }

    public IActionResult Orders()
    {
        var orders = _context.Orders
            .Include(o => o.Employee)
            .Include(o => o.OrderType)
            .ToList();

        return View(orders);
    }
}