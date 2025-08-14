namespace Cinema.Models;

public class CinemaMovie
{
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    
    public CinemaMovie(int movieId, string title, string description, int durationMinutes)
    {
        MovieId = movieId;
        Title = title;
        Description = description;
        DurationMinutes = durationMinutes;
    }
    
    public void DisplayDetails()
    {
        Console.Write($"Movie: {0}\n", Title);
        Console.Write("Duration: {0} minutes ({1}h {2}m)\n", DurationMinutes,  DurationMinutes / 60, DurationMinutes % 6);
        Console.Write($"Description: {Description}\n");
    }

    public override string ToString()
    {
        return $"{Title} ({DurationMinutes} min)";
    }
}