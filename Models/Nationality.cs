using System.ComponentModel.DataAnnotations;

namespace Staff_WebServer.Models;

public class Nationality
{
    [Key]
    public int NationalityCode { get; set; }
    public string Name { get; set; }
}