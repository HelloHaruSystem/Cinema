using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

public class Movie
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public string? Description { get; set; }
    [Required]
    public int DurationMinutes { get; set; }
}