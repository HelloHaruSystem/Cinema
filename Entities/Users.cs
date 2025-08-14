using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

public class Users
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string PasswordHash { get; set; }
    [Required]
    public string Role { get; set; }
}