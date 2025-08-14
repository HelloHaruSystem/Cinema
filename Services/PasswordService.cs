using Cinema.Utils;

namespace Cinema.Services;

/// <summary>
/// Handles password hashing and verification using BCrypt.
/// Provides secure password storage and authentication.
/// </summary>
public class PasswordService
{

    /// <summary>
    /// Hashes a plain text password using BCrypt with salt.
    /// </summary>
    /// <param name="password">Plain text password to hash</param>
    /// <returns>Hashed password string safe for database storage</returns>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(AppConfig.SaltRounds));
    }
    
    /// <summary>
    /// Verifies a plain text password against a stored hash.
    /// </summary>
    /// <param name="password">Plain text password to verify</param>
    /// <param name="hashedPassword">Stored hashed password from database</param>
    /// <returns>True if password matches, false otherwise</returns>
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