namespace Cinema.Models;

public class CinemaHall
{
    public int HallId { get; set; }
    public string HallName { get; set; }
    public int Rows  { get; set; }
    public int SeatsPerRow { get; set; }
    public CinemaSeat[,] SeatLayout { get; set; }

    public CinemaHall(int hallId, string hallName, int rows, int seatsPerRow)
    {
        HallId = hallId;
        HallName = hallName;
        Rows = rows;
        SeatsPerRow = seatsPerRow;
        SeatLayout = new CinemaSeat[rows, seatsPerRow];
    }

    public void SetSeat(int row, int seatNumber, CinemaSeat seat)
    {
        if (IsValidPosition(row, seatNumber))
        {
            SeatLayout[row, seatNumber] = seat;
        }
    }

    public CinemaSeat? GetSeat(int row, int seatNumber)
    {
        if (IsValidPosition(row, seatNumber))
        {
            return SeatLayout[row, seatNumber];
        }
        return null;
    }

    private bool IsValidPosition(int row, int seatNumber)
    {
        return row >= 0 && row < this.Rows && seatNumber >= 0 && seatNumber <= this.SeatsPerRow;
    }
    
    public bool IsSeatAvailable(int row, int seatNumber)
    {
        // convert to 0 based index
        CinemaSeat seat = GetSeat(row - 1, seatNumber - 1);
        return seat != null && seat.IsAvailable();
    }

    public bool ReserveSeat(int row, int seatNumber)
    {
        // convert to 0 based index
        CinemaSeat seat = GetSeat(row - 1, seatNumber - 1);
        if (seat != null && seat.IsAvailable())
        {
            seat.Status = SeatStatus.Reserved;
            return true;
        }

        return false;
    }

    public int GetAvailableSeatsCount()
    {
        int count = 0;
        for (int row = 0; row < this.Rows; row++)
        {
            for (int seat = 0; seat < this.SeatsPerRow; seat++)
            {
                if (SeatLayout[row, seat].IsAvailable())
                {
                    count++;
                }
            }
        }
        return count;
    }

    public int GetTotalSeatsCount()
    {
        return Rows * SeatsPerRow;
    }
    
    public int GetReservedSeatsCount()
    {
        return GetTotalSeatsCount() - GetAvailableSeatsCount();
    }

    public void PrintStatistics()
    {
        int total = GetTotalSeatsCount();
        int available = GetAvailableSeatsCount();
        int reserved = GetReservedSeatsCount();
        double reservedPercentage = total > 0 ? (double)reserved / total * 100 : 0;

        Console.Write("\n=== {0} Statistics ===\n", HallName);
        Console.Write("Total Seats: {0}\n", total);
        Console.Write("Available: {0}\n", available);
        Console.Write("Reserved: {0}\n", reserved);
        Console.Write("Percentage reserved: {0:F1}%\n", reservedPercentage);
    }
}