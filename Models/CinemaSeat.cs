namespace Cinema.Models;

public class CinemaSeat
{
    public int SeatNumber { get; set; }
    public SeatStatus Status { get; set; }
    public int DataBaseId  { get; set; }

    public CinemaSeat(int seatNumber, int databaseId, SeatStatus status = SeatStatus.Free)
    {
        SeatNumber = seatNumber;
        DataBaseId = databaseId;
        Status = status;
    }
    
    public bool IsAvailable() => Status == SeatStatus.Free;

    public override string ToString()
    {
        return $"Seat {SeatNumber} - {Status}";
    }
}