using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

/// <summary>
/// Represents a scheduled movie screening at a specific time and hall.
/// </summary>
public class Screening
{
    /// <summary>
    /// Gets or sets the unique screening identifier.
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the movie being screened.
    /// </summary>
    [Required]
    public int MovieId { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the hall where the screening takes place.
    /// </summary>
    [Required]
    public int ScreenHallId { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the screening starts.
    /// </summary>
    [Required]
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// Gets or sets the ticket price for this screening.
    /// </summary>
    [Required]
    public double Price {get; set; }
}