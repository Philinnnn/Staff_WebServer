using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

public class StaffingTable
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Department")]
    public int DepartmentCode { get; set; }
    public Department Department { get; set; }

    [ForeignKey("Position")]
    public int PositionCode { get; set; }
    public Position Position { get; set; }

    public int TotalPositions { get; set; }
    public int VacantPositions { get; set; }
}