using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

public class Department
{
    [Key]
    public int DepartmentCode { get; set; }
    public string Name { get; set; }

    public int? HeadEmployeeId { get; set; } // таб. номер начальника подразделения
    [ForeignKey("HeadEmployeeId")]
    public Employee? Head { get; set; }
}