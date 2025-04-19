using System.ComponentModel.DataAnnotations;

namespace Staff_WebServer.Models;

public class OrderType
{
    [Key]
    public int OrderTypeCode { get; set; }
    public string Name { get; set; } // прием, увольнение, отпуск…
}