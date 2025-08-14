using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

public class Users
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    [Required]
    public string Role { get; set; } = string.Empty;
}