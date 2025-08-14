using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

public class Screening
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int MovieId { get; set; }
    [Required]
    public int ScreenHallId { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public double Price {get; set; }
}