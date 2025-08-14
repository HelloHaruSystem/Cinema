using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

/// <summary>
/// Represents temporarily blocked seats to prevent double booking during the selection process.
/// </summary>
public class BlockedSeats
{
    /// <summary>
    /// Gets or sets the unique block identifier.
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the screening ID.
    /// </summary>
    [Required]
    public int ScreeningId { get; set; }
    
    /// <summary>
    /// Gets or sets the seat ID that is blocked.
    /// </summary>
    [Required]
    public int SeatId { get; set; }
    
    /// <summary>
    /// Gets or sets when the block expires and the seat becomes available again.
    /// </summary>
    [Required]
    public DateTime BlockedUntil { get; set; }
}