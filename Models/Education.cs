using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

[Table("образование")]
public class Education
{
    [Key]
    [Column("код_образования")]
    public int EducationCode { get; set; }

    [Column("наименование")]
    public string Name { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}