using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

public class BlockedSeats
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int ScreeningId { get; set; }
    [Required]
    public int SeatId { get; set; }
    [Required]
    public DateTime BlockedUntil { get; set; }
}