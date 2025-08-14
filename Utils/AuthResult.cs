using Cinema.Entities;

namespace Cinema.Utils;

/// <summary>
/// Result object for authentication operations (login/register).
/// Contains success status, message, and user information.
/// </summary>
public class AuthResult
{
    /// <summary>
    /// Gets or sets whether the authentication operation was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the result message (success confirmation or error description).
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user information (only set on successful login).
    /// </summary>
    public Users? User { get; set; }
}