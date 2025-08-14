using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

public class Bookings
{
    [Key]
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? GuestName  { get; set; }
    public string? GuestEmail { get; set; }
    [Required]
    public int ScreeningId { get; set; }
    [Required]
    public int SeatId { get; set; }
    [Required]
    public DateTime BookingTime { get; set; }
}