using Cinema.Entities;

namespace Cinema.Utils;

public class AuthResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Users? User { get; set; }
}