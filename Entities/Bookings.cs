using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

/// <summary>
/// Represents a seat booking for a specific screening.
/// Can be made by registered users or guests.
/// </summary>
public class Bookings
{
    
    /// <summary>
    /// Gets or sets the unique booking identifier.
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the user ID (null for guest bookings).
    /// </summary>
    public int? UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the guest name (null for user bookings).
    /// </summary>
    public string? GuestName  { get; set; }
    
    /// <summary>
    /// Gets or sets the guest email (null for user bookings).
    /// </summary>
    public string? GuestEmail { get; set; }
    
    /// <summary>
    /// Gets or sets the screening ID this booking is for.
    /// </summary>
    [Required]
    public int ScreeningId { get; set; }
    
    /// <summary>
    /// Gets or sets the seat ID being booked.
    /// </summary>
    [Required]
    public int SeatId { get; set; }
    
    /// <summary>
    /// Gets or sets when the booking was made.
    /// </summary>
    [Required]
    public DateTime BookingTime { get; set; }
}