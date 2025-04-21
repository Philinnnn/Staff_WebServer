using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;
using Staff_WebServer.Models;

namespace Staff_WebServer.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult AccessLogs()
    {
        var logs = context.AccessLogs
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

        using (var connection = new SqlConnection(context.Database.GetConnectionString()))
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

    public IActionResult ManageUsers()
    {
        var users = userManager.Users
            .OrderBy(u => u.Email)
            .ToList();

        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleLock(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow); // unlock
            else
                await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddYears(100)); // lock
        }

        return RedirectToAction("ManageUsers");
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var newPass = GenerateSecurePassword();
            var result = await userManager.ResetPasswordAsync(user, token, newPass);

            TempData["Message"] = result.Succeeded
                ? $"Пароль для {user.Email} сброшен. Новый пароль: {newPass}"
                : $"❌ Ошибка: {string.Join(", ", result.Errors.Select(e => e.Description))}";
        }

        return RedirectToAction("ManageUsers");
    }
    
    private static string GenerateSecurePassword()
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";

        var rnd = new Random();
        return new string(Enumerable.Range(0, 8)
            .Select(i =>
                i switch
                {
                    0 => upper[rnd.Next(upper.Length)],
                    1 => lower[rnd.Next(lower.Length)],
                    2 => digits[rnd.Next(digits.Length)],
                    3 => special[rnd.Next(special.Length)],
                    _ => (upper + lower + digits + special)[rnd.Next(upper.Length + lower.Length + digits.Length + special.Length)]
                }
            )
            .OrderBy(_ => rnd.Next())
            .ToArray());
    }
}
