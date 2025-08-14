using System.ComponentModel.DataAnnotations;

namespace Cinema.Entities;

public class Hall
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public int Rows { get; set; }
    [Required]
    public int SeatsPerRow { get; set; }
}