using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;
using Staff_WebServer.Models;

namespace Staff_WebServer.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult CreateUser()
    {
        ViewBag.Nationalities = _context.Nationalities.ToList();
        ViewBag.Educations = _context.Educations.ToList();
        ViewBag.PensionFunds = _context.PensionFunds.ToList();
        ViewBag.Positions = _context.Positions.ToList();
        ViewBag.Departments = _context.Departments.ToList();

        return View();
    }
    
    [Authorize(Roles = "Admin")]
    public IActionResult AccessLogs()
    {
        var logs = _context.AccessLogs
            .OrderByDescending(log => log.Timestamp)
            .Take(100)
            .ToList();

        return View(logs);
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
        // 1. Сохраняем сотрудника
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

        // 2. Создаём пользователя и привязываем по табельному номеру
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

        // Повторно загружаем ViewBag для возврата на страницу
        ViewBag.Nationalities = _context.Nationalities.ToList();
        ViewBag.Educations = _context.Educations.ToList();
        ViewBag.PensionFunds = _context.PensionFunds.ToList();
        ViewBag.Positions = _context.Positions.ToList();
        ViewBag.Departments = _context.Departments.ToList();

        return View();
    }
        
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateBackup()
    {
        string backupDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Backups");
        if (!Directory.Exists(backupDirectory))
            Directory.CreateDirectory(backupDirectory);

        string backupFile = $"StaffDB_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
        string fullPath = Path.Combine(backupDirectory, backupFile);

        string sql = $@"BACKUP DATABASE [Staff] TO DISK = N'{fullPath}' WITH INIT, FORMAT";

        using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
        {
            connection.Open();
            using (var command = new SqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        TempData["Message"] = "Резервная копия успешно создана.";
        return RedirectToAction("Backups");
    }
    
    [Authorize(Roles = "Admin")]
    public IActionResult Backups()
    {
        string backupDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Backups");
        var files = Directory.Exists(backupDirectory)
            ? Directory.GetFiles(backupDirectory)
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .ToList()
            : new List<FileInfo>();

        return View(files);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteBackup(string fileName)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Backups", fileName);
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            TempData["Message"] = "Резервная копия успешно удалена.";
        }
        else
        {
            TempData["Message"] = "Ошибка: файл не найден.";
        }

        return RedirectToAction("Backups");
    }
}
