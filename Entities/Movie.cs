using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

/// <summary>
/// Represents a movie that can be screened in the cinema.
/// </summary>
public class Movie
{
    /// <summary>
    /// Gets or sets the unique movie identifier.
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the movie title.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the movie description (optional).
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the movie duration in minutes.
    /// </summary>
    [Required]
    public int DurationMinutes { get; set; }
}