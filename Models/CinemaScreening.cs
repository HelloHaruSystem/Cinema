namespace Cinema.Models;

public class CinemaScreening
{
    public int ScreeningId { get; set; }
    public CinemaMovie Movie { get; set; }
    public CinemaHall Hall { get; set; }
    public DateTime StartTime { get; set; }
    public double Price { get; set; }

    public CinemaScreening(int screeningId, CinemaMovie movie, CinemaHall hall, DateTime startTime, double price)
    {
        ScreeningId = screeningId;
        Movie = movie;
        Hall = hall;
        StartTime = startTime;
        Price = price;
    }
    
    public bool IsAvailableForBooking()
    {
        return StartTime > DateTime.Now && Hall.GetAvailableSeatsCount() > 0;
    }
    
    public bool IsStartingSoon(int minutesThreshold = 30)
    {
        return StartTime <= DateTime.Now.AddMinutes(minutesThreshold);
    }

    public override string ToString()
    {
        return $"{Movie.Title} - {StartTime:dd-MM-yyyy HH:mm} - {Hall.HallName} - {Price:C}";
    }
}