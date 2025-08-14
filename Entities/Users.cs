using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

/// <summary>
/// Represents a user account in the cinema system.
/// </summary>
public class Users
{
    /// <summary>
    /// Gets or sets the unique user identifier.
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the username (must be unique).
    /// </summary>
    [Required]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the BCrypt hashed password.
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user role (e.g., "customer", "admin").
    /// </summary>
    [Required]
    public string Role { get; set; } = string.Empty;
}