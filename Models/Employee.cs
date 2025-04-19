using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Staff_WebServer.Models;

namespace Staff_WebServer.Models;
[Table("работники")]
public class Employee
{
    [Key]
    [Column("табельный_номер")]
    public int Id { get; set; }

    [Column("ФИО")]
    public string FullName { get; set; }

    [Column("адрес_проживания")]
    public string Address { get; set; }

    [Column("дата_рождения")]
    public DateTime BirthDate { get; set; }

    [Column("код_национальности")]
    public int NationalityId { get; set; }

    [Column("пол")]
    public string Gender { get; set; }

    [Column("код_образования")]
    public int EducationId { get; set; }

    [Column("кол_во_иждивенцев")]
    public int Dependents { get; set; }

    [Column("ИИН")]
    public string IIN { get; set; }

    [Column("код_пенсионного_фонда")]
    public int PensionFundId { get; set; }

    [Column("код_должности")]
    public int PositionId { get; set; }

    [Column("оклад")]
    public decimal Salary { get; set; }

    [Column("код_подразделения")]
    public int DepartmentId { get; set; }

    [Column("дата_приема")]
    public DateTime HireDate { get; set; }

    [Column("дата_увольнения")]
    public DateTime? DismissDate { get; set; }

    public Nationality Nationality { get; set; }
    public Education Education { get; set; }
    public PensionFund PensionFund { get; set; }
    public Position Position { get; set; }
    public Department Department { get; set; }
}