namespace Staff_WebServer.Models;

public class AccessLog
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Action { get; set; }
    public DateTime Timestamp { get; set; }
}
