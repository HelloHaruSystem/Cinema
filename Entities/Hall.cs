using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

/// <summary>
/// Represents a cinema hall/screen with seating configuration.
/// </summary>
public class Hall
{
    /// <summary>
    /// Gets or sets the unique hall identifier.
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the hall name (e.g., "Hall A", "Hall B").
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the number of rows in the hall.
    /// </summary>
    [Required]
    public int Rows { get; set; }
    
    /// <summary>
    /// Gets or sets the number of seats per row.
    /// </summary>
    [Required]
    public int SeatsPerRow { get; set; }
}