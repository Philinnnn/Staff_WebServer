namespace Staff_WebServer.Models;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string IIN { get; set; }
    public string Address { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public int NationalityCode { get; set; }
    public int EducationCode { get; set; }
    public int Dependents { get; set; }
    public int PensionFundCode { get; set; }
    public int PositionCode { get; set; }
    public decimal Salary { get; set; }
    public int DepartmentCode { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? DismissDate { get; set; }
}
