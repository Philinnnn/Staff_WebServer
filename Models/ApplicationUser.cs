using Microsoft.AspNetCore.Identity;

namespace Staff_WebServer.Models;

public class ApplicationUser : IdentityUser
{
    public int ТабельныйНомер { get; set; }
}