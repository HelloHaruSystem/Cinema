namespace Cinema.Utils;

public class BookingResult
{
    public bool IsSuccessful { get; set; }
    public List<int[]> SuccessfulSeats { get; set; } = new List<int[]>();
    public List<string> FailedSeats { get; set; } = new List<string>();
}