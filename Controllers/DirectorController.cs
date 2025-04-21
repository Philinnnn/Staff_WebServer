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
    public IActionResult CreateOrder()
    {
        ViewBag.Employees = _context.Employees.OrderBy(e => e.FullName).ToList();
        ViewBag.OrderTypes = _context.OrderTypes.OrderBy(t => t.Name).ToList();

        return View(new Order
        {
            Date = DateTime.Today
        });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Director")]
    public IActionResult CreateOrder(Order order)
    {
        Console.WriteLine("➡️ Получен POST-запрос на создание приказа");

        // Удаляем из валидации навигационные свойства
        ModelState.Remove(nameof(order.Employee));
        ModelState.Remove(nameof(order.OrderType));

        Console.WriteLine($"📋 Модель приказа:");
        Console.WriteLine($"- EmployeeId: {order.EmployeeId}");
        Console.WriteLine($"- OrderTypeId: {order.OrderTypeId}");
        Console.WriteLine($"- Text: {order.Text}");
        Console.WriteLine($"- Date: {order.Date}");

        if (ModelState.IsValid)
        {
            Console.WriteLine("✅ ModelState — корректен. Сохраняем в БД...");

            _context.Orders.Add(order);
            _context.SaveChanges();

            Console.WriteLine("💾 Приказ успешно сохранён!");

            return RedirectToAction("OrderList");
        }

        Console.WriteLine("❌ ModelState — НЕвалиден. Ошибки:");
        foreach (var key in ModelState.Keys)
        {
            var state = ModelState[key];
            foreach (var error in state.Errors)
            {
                Console.WriteLine($"- Поле: {key}, Ошибка: {error.ErrorMessage}");
            }
        }

        ViewBag.Employees = _context.Employees.OrderBy(e => e.FullName).ToList();
        ViewBag.OrderTypes = _context.OrderTypes.OrderBy(t => t.Name).ToList();

        return View(order);
    }

    [Authorize(Roles = "Director")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RevokeOrder(int id)
    {
        var order = _context.Orders.Find(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }
        return RedirectToAction("OrderList");
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
