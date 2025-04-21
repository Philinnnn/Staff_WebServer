using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;
using Staff_WebServer.Models;

namespace Staff_WebServer.Controllers;

[Authorize]
public class DirectorController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public DirectorController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [Authorize(Roles = "Director")]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = "HR, Director")]
    public IActionResult OrderList(string? search, string? sort)
    {
        var orders = _context.Orders
            .Include(o => o.Employee)
            .Include(o => o.OrderType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search) && int.TryParse(search, out int orderId))
        {
            orders = orders.Where(o => o.Id == orderId);
        }

        orders = sort switch
        {
            "asc" => orders.OrderBy(o => o.Id),
            "desc" => orders.OrderByDescending(o => o.Id),
            _ => orders
        };

        return View("OrderList", orders.ToList());
    }

    [Authorize(Roles = "Director")]
    public IActionResult EmployeesByDepartment()
    {
        var departments = _context.Departments
            .Include(d => d.Employees.Where(e => e.DismissDate == null))
            .ThenInclude(e => e.Position)
            .Include(d => d.Employees)
            .ThenInclude(e => e.Education)
            .Include(d => d.Employees)
            .ThenInclude(e => e.Nationality)
            .Include(d => d.Employees)
            .ThenInclude(e => e.PensionFund)
            .OrderBy(d => d.Name)
            .ToList();

        ViewBag.Positions = _context.Positions.ToList();
        return View(departments);
    }


    [Authorize(Roles = "Director")]
    public IActionResult Reports()
    {
        return View();
    }

    [Authorize(Roles = "Director")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateField(int id, [FromBody] FieldUpdateModel model)
    {
        var employee = await _context.Employees
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return NotFound();

        switch (model.Field)
        {
            case "FullName":
                employee.FullName = model.Value;
                break;
            case "Position":
                if (int.TryParse(model.Value, out var posId))
                    employee.PositionId = posId;
                break;
            case "Salary":
                if (decimal.TryParse(model.Value, out var salary))
                    employee.Salary = salary;
                break;
            default:
                return BadRequest("Недопустимое поле.");
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
}
