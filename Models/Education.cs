using System.ComponentModel.DataAnnotations;

namespace Staff_WebServer.Models;

public class Education
{
    [Key]
    public int EducationCode { get; set; }
    public string Name { get; set; } // среднее, средне-проф., высшее и т.д.
}