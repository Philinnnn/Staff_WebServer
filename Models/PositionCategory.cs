using System.ComponentModel.DataAnnotations;

namespace Staff_WebServer.Models;

public class PositionCategory
{
    [Key]
    public int CategoryCode { get; set; }
    public string Name { get; set; } // рабочие, ИТР, АУП, служащие…
}