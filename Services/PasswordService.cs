using BCrypt.Net;
using Cinema.Utils;


namespace Cinema.Services;

public class PasswordService
{

    public string HashPassword(string password)
    {

        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(AppConfig.SaltRounds));
    }
    
    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }
}