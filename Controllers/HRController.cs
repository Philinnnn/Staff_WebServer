using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;
using Staff_WebServer.Models;

namespace Staff_WebServer.Controllers;

[Authorize(Roles = "HR")]
public class HRController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    
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
    
    public IActionResult CreateUser()
    {
        ViewBag.Nationalities = _context.Nationalities.ToList();
        ViewBag.Educations = _context.Educations.ToList();
        ViewBag.PensionFunds = _context.PensionFunds.ToList();
        ViewBag.Positions = _context.Positions.ToList();
        ViewBag.Departments = _context.Departments.ToList();

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser(
        string FullName,
        string Address,
        DateTime BirthDate,
        string Gender,
        string IIN,
        int NationalityId,
        int EducationId,
        int PensionFundId,
        int PositionId,
        int DepartmentId,
        int Dependents,
        decimal Salary,
        DateTime HireDate,

        string Email,
        string Password,
        string Role
    )
    {
        var employee = new Employee
        {
            FullName = FullName,
            Address = Address,
            BirthDate = BirthDate,
            Gender = Gender,
            IIN = IIN,
            NationalityId = NationalityId,
            EducationId = EducationId,
            PensionFundId = PensionFundId,
            PositionId = PositionId,
            DepartmentId = DepartmentId,
            Dependents = Dependents,
            Salary = Salary,
            HireDate = HireDate
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        
        var user = new ApplicationUser
        {
            UserName = Email,
            Email = Email,
            ТабельныйНомер = employee.Id
        };

        var result = await _userManager.CreateAsync(user, Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, Role);
            ViewBag.Message = $"Пользователь и сотрудник успешно созданы. Табельный номер: {employee.Id}";
        }
        else
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
        
        ViewBag.Nationalities = _context.Nationalities.ToList();
        ViewBag.Educations = _context.Educations.ToList();
        ViewBag.PensionFunds = _context.PensionFunds.ToList();
        ViewBag.Positions = _context.Positions.ToList();
        ViewBag.Departments = _context.Departments.ToList();

        return View();
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