using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

public class Employee
{
    [Key]
    public int Id { get; set; }
    public string FullName { get; set; }
    public string IIN { get; set; }
    public string Address { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }

    [ForeignKey("Nationality")]
    public int NationalityCode { get; set; }
    public Nationality Nationality { get; set; }

    [ForeignKey("Education")]
    public int EducationCode { get; set; }
    public Education Education { get; set; }

    public int Dependents { get; set; }

    [ForeignKey("PensionFund")]
    public int PensionFundCode { get; set; }
    public PensionFund PensionFund { get; set; }

    [ForeignKey("Position")]
    public int PositionCode { get; set; }
    public Position Position { get; set; }

    public decimal Salary { get; set; }

    [ForeignKey("Department")]
    public int DepartmentCode { get; set; }
    public Department Department { get; set; }

    public DateTime HireDate { get; set; }
    public DateTime? DismissDate { get; set; }
}