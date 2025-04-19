using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;
using Staff_WebServer.Models;

namespace Staff_WebServer.Controllers;

[Authorize(Roles = "Director")]
public class DirectorController : Controller
{
    private readonly ApplicationDbContext _context;

    public DirectorController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }
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


    public IActionResult EmployeesByDepartment()
    {
        var departments = _context.Departments
            .Include(d => d.Employees)
            .ThenInclude(e => e.Position)
            .OrderBy(d => d.Name)
            .ToList();

        ViewBag.Positions = _context.Positions.ToList();

        return View(departments);
    }
    public IActionResult Reports()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateField(int id, [FromBody] FieldUpdateModel model)
    {
        var employee = await _context.Employees
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();

        switch (model.Field)
        {
            case "FullName":
                employee.FullName = model.Value;
                break;
            case "Position":
                if (int.TryParse(model.Value, out var posId))
                    employee.PositionId = posId;
                break;
            case "HireDate":
                if (DateTime.TryParse(model.Value, out var date))
                    employee.HireDate = date;
                break;
            case "Salary":
                if (decimal.TryParse(model.Value, out var salary))
                    employee.Salary = salary;
                break;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
}