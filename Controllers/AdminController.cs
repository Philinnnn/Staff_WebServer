using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;

namespace Staff_WebServer.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult AccessLogs()
    {
        var logs = _context.AccessLogs
            .OrderByDescending(log => log.Timestamp)
            .Take(100)
            .ToList();

        return View(logs);
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
