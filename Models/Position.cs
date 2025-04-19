using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

public class Position
{
    [Key]
    public int PositionCode { get; set; }
    public string Name { get; set; }

    [ForeignKey("PositionCategory")]
    public int CategoryCode { get; set; }
    public PositionCategory PositionCategory { get; set; }
}