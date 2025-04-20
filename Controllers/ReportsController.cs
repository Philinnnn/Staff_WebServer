using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;

namespace Staff_WebServer.Controllers;

[Authorize(Roles = "HR, Director")]
public class ReportsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Reports()
    {
        ViewBag.Departments = _context.Departments.OrderBy(d => d.Name).ToList();
        return View();
    }

    public IActionResult PensionerReport()
    {
        var result = new List<dynamic>();

        var connection = _context.Database.GetDbConnection();
        connection.Open();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            SELECT 
                табельный_номер,
                ФИО,
                пол,
                дата_рождения,
                дата_увольнения,
                dbo.fn_ПолныхЛет(дата_рождения) AS возраст
            FROM Работники
            WHERE дата_увольнения IS NOT NULL 
              AND YEAR(дата_увольнения) = YEAR(GETDATE())
              AND (
                  (пол = N'м' AND dbo.fn_ПолныхЛет(дата_рождения) >= 63)
                  OR (пол = N'ж' AND dbo.fn_ПолныхЛет(дата_рождения) >= 58)
              )";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        FullName = reader["ФИО"].ToString(),
                        Gender = reader["пол"].ToString(),
                        BirthDate = Convert.ToDateTime(reader["дата_рождения"]),
                        DismissDate = Convert.ToDateTime(reader["дата_увольнения"]),
                        Age = reader["возраст"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["возраст"])
                    });
                }
            }
        }

        connection.Close();
        return View(result);
    }

    public IActionResult DependentAllowances(int departmentId)
    {
        var result = new List<dynamic>();

        var connection = _context.Database.GetDbConnection();
        connection.Open();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = """

                                              SELECT 
                                                  табельный_номер,
                                                  ФИО,
                                                  оклад,
                                                  кол_во_иждивенцев,
                                                  dbo.fn_Рассчитать_начисления_на_иждивенцев(табельный_номер) AS Начисления
                                              FROM Работники
                                              WHERE код_подразделения = @dep
                                  """;

            var param = command.CreateParameter();
            param.ParameterName = "@dep";
            param.Value = departmentId;
            command.Parameters.Add(param);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        Id = reader["табельный_номер"],
                        FullName = reader["ФИО"].ToString(),
                        Salary = Convert.ToDecimal(reader["оклад"]),
                        Dependents = Convert.ToInt32(reader["кол_во_иждивенцев"]),
                        Total = Convert.ToInt32(reader["Начисления"])
                    });
                }
            }
        }

        connection.Close();
        return View(result);
    }
    
    public IActionResult EmployeesByOrderAndLetter(int orderId, string letter)
    {
        var result = new List<dynamic>();
        var conn = _context.Database.GetDbConnection();
        conn.Open();

        using (var command = conn.CreateCommand())
        {
            command.CommandText = @"
            SELECT r.ФИО, p.номер_приказа, p.дата
            FROM Приказы p
            JOIN Работники r ON p.табельный_номер = r.табельный_номер
            WHERE p.номер_приказа = @orderId AND r.ФИО LIKE @letter + '%'";

            var param1 = command.CreateParameter();
            param1.ParameterName = "@orderId";
            param1.Value = orderId;
            command.Parameters.Add(param1);

            var param2 = command.CreateParameter();
            param2.ParameterName = "@letter";
            param2.Value = letter;
            command.Parameters.Add(param2);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        FullName = reader["ФИО"].ToString(),
                        OrderNumber = Convert.ToInt32(reader["номер_приказа"]),
                        OrderDate = Convert.ToDateTime(reader["дата"])
                    });
                }
            }
        }

        conn.Close();
        return View(result);
    }

    [HttpGet]
    public IActionResult ManyChildren(int departmentId)
    {
        var result = new List<dynamic>();
        var conn = _context.Database.GetDbConnection();
        conn.Open();

        using (var command = conn.CreateCommand())
        {
            command.CommandText = @"
            SELECT табельный_номер, ФИО, кол_во_иждивенцев
            FROM Работники
            WHERE код_подразделения = @dep AND кол_во_иждивенцев > 3";
            var param = command.CreateParameter();
            param.ParameterName = "@dep";
            param.Value = departmentId;
            command.Parameters.Add(param);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        Id = reader["табельный_номер"],
                        FullName = reader["ФИО"].ToString(),
                        Dependents = Convert.ToInt32(reader["кол_во_иждивенцев"])
                    });
                }
            }
        }

        conn.Close();
        return View(result);
    }

    public IActionResult CountByPensionFund()
    {
        var result = new List<dynamic>();
        var conn = _context.Database.GetDbConnection();
        conn.Open();

        using (var command = conn.CreateCommand())
        {
            command.CommandText = @"
            SELECT pf.наименование, COUNT(*) AS Количество
            FROM Работники r
            JOIN Пенсионные_фонды pf ON r.код_пенсионного_фонда = pf.код_фонда
            GROUP BY pf.наименование";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        FundName = reader["наименование"].ToString(),
                        Count = Convert.ToInt32(reader["Количество"])
                    });
                }
            }
        }

        conn.Close();
        return View(result);
    }

    public IActionResult EmployeeWorkYears(string sort)
    {
        var result = new List<dynamic>();

        var connection = _context.Database.GetDbConnection();
        connection.Open();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            SELECT 
                табельный_номер,
                ФИО,
                дата_приема,
                dbo.fn_ПолныхЛет(дата_приема) AS стаж
            FROM Работники";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        Id = reader["табельный_номер"],
                        FullName = reader["ФИО"].ToString(),
                        HireDate = Convert.ToDateTime(reader["дата_приема"]),
                        Years = Convert.ToInt32(reader["стаж"])
                    });
                }
            }
        }

        connection.Close();

        if (sort == "desc")
            result = result.OrderByDescending(r => r.Years).ToList();
        else
            result = result.OrderBy(r => r.Years).ToList();

        return View(result);
    }

[HttpGet]
public IActionResult ExperienceInRange(int min, int max)
{
    var result = new List<dynamic>();
    var conn = _context.Database.GetDbConnection();
    conn.Open();

    using (var command = conn.CreateCommand())
    {
        command.CommandText = @"
        SELECT табельный_номер, ФИО, дата_приема, DATEDIFF(YEAR, дата_приема, GETDATE()) AS Стаж
        FROM Работники
        WHERE DATEDIFF(YEAR, дата_приема, GETDATE()) BETWEEN @min AND @max";

        var p1 = command.CreateParameter();
        p1.ParameterName = "@min";
        p1.Value = min;
        command.Parameters.Add(p1);

        var p2 = command.CreateParameter();
        p2.ParameterName = "@max";
        p2.Value = max;
        command.Parameters.Add(p2);

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                result.Add(new
                {
                    Id = reader["табельный_номер"],
                    FullName = reader["ФИО"].ToString(),
                    HireDate = Convert.ToDateTime(reader["дата_приема"]),
                    Years = Convert.ToInt32(reader["Стаж"])
                });
            }
        }
    }

    conn.Close();
    
    ViewBag.Min = min;
    ViewBag.Max = max;

    return View(result);
}
    
    [HttpGet]
    public IActionResult OrderStartsWith(int orderId)
    {
        var result = new List<dynamic>();
        var conn = _context.Database.GetDbConnection();
        conn.Open();

        using (var command = conn.CreateCommand())
        {
            command.CommandText = @"
            SELECT r.ФИО, p.номер_приказа
            FROM Приказы p
            JOIN Работники r ON r.табельный_номер = p.табельный_номер
            WHERE номер_приказа = @id AND LEFT(r.ФИО, 1) = N'К'";

            var param = command.CreateParameter();
            param.ParameterName = "@id";
            param.Value = orderId;
            command.Parameters.Add(param);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        FullName = reader["ФИО"].ToString(),
                        Order = reader["номер_приказа"]
                    });
                }
            }
        }

        conn.Close();
        return View(result);
    }
}