using Cinema.Entities;
using Cinema.Services;

namespace Cinema.Utils;

public class SeatMapHelper
{
    private readonly CinemaDataService _dataService;

    public SeatMapHelper(CinemaDataService dataService)
    {
        _dataService = dataService;
    }

    public bool[,] CreateSeatLayoutArray(int screeningId, Hall hall)
    {
        // false = available, true = taken
        bool[,] seatLayout = new bool[hall.Rows, hall.SeatsPerRow];
        
        var seatsWithStatus = _dataService.LoadSeatsWithStatus(hall.Id, screeningId);

        foreach (var (seat, isBooked, isBlocked) in seatsWithStatus)
        {
            int arrayRow = seat.RowNumber - 1;
            int arraySeat = seat.SeatNumber - 1;

            if (arrayRow >= 0 && arrayRow < hall.Rows &&
                arraySeat >= 0 && arraySeat < hall.SeatsPerRow)
            {
                seatLayout[arrayRow, arraySeat] = isBooked ||  isBlocked; // true taken
            }
        }
        
        return seatLayout;
    }
    
    public int[,] CreateSeatIdArray(int screeningId, Hall hall)
    {
        int[,] seatIds = new int[hall.Rows, hall.SeatsPerRow];
    
        var seatsWithStatus = _dataService.LoadSeatsWithStatus(hall.Id, screeningId);
    
        foreach (var (seat, _, _) in seatsWithStatus)
        {
            int arrayRow = seat.RowNumber - 1;
            int arraySeat = seat.SeatNumber - 1;
        
            if (arrayRow >= 0 && arrayRow < hall.Rows &&
                arraySeat >= 0 && arraySeat < hall.SeatsPerRow)
            {
                seatIds[arrayRow, arraySeat] = seat.Id;
            }
        }
    
        return seatIds;
    }
    
    public void DisplaySeatMapWithArray(Hall hall, bool[,] seatLayout)
    {
        Console.Write("\nSeat Map for {0}\n", hall.Name);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("▮");
        Console.ResetColor();
        Console.Write(" = Taken | ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("▮");
        Console.ResetColor();
        Console.Write(" = Available\n");
        
        // column numbers
        Console.Write("   ");
        for (int seat = 1; seat <= hall.SeatsPerRow; seat++)
        {
            Console.Write($"{seat,2} ");
        }
        Console.Write("\n");
        
        // rows with seats
        for (int row = 0; row < hall.Rows; row++)
        {
            Console.Write($"{row + 1,2} ");

            for (int seat = 0; seat < hall.SeatsPerRow; seat++)
            {
                if (seatLayout[row, seat]) // true = taken
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("▮  ");
                }
                else // false = available
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("▮  ");
                }
                Console.ResetColor();
            }
            Console.Write("\n");
        }
    }
    
    public int CountTakenSeats(bool[,] seatLayout, int rows, int seatsPerRow)
    {
        int count = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int seat = 0; seat < seatsPerRow; seat++)
            {
                if (seatLayout[row, seat])
                {
                    count++;
                }
            }
        }
        return count;
    }
    
    public void DisplaySeatStatistics(Hall hall, bool[,] seatLayout)
    {
        int totalSeats = hall.Rows * hall.SeatsPerRow;
        int takenSeats = CountTakenSeats(seatLayout, hall.Rows, hall.SeatsPerRow);
        int availableSeats = totalSeats - takenSeats;
        double occupancyPercentage = totalSeats > 0 ? (double)takenSeats / totalSeats * 100 : 0;

        Console.Write("\n📊 Seat Statistics:\n");
        Console.Write("   Total Seats: {0}\n", totalSeats);
        Console.Write("   Available: {0}\n", availableSeats);
        Console.Write("   Taken: {0}\n", takenSeats);
        Console.Write("   Occupancy: {0:F1}%\n", occupancyPercentage);
    }
}