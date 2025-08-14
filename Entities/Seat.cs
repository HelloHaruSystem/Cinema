using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

/// <summary>
/// Represents an individual seat within a cinema hall.
/// </summary>
public class Seat
{
    /// <summary>
    /// Gets or sets the unique seat identifier.
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the hall this seat belongs to.
    /// </summary>
    [Required]
    public int ScreenHallId  { get; set; }
    
    /// <summary>
    /// Gets or sets the row number (1-based).
    /// </summary>
    [Required]
    public int RowNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the seat number within the row (1-based).
    /// </summary>
    [Required]
    public int SeatNumber { get; set; }
}