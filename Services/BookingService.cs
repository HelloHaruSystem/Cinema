using Cinema.Repository;
using Cinema.Utils;

namespace Cinema.Services;

public class BookingService
{
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;

    public BookingService(ICinemaRepository repository, CinemaDataService dataService)
    {
        _repository = repository;
        _dataService = dataService;
    }

    public BookingResult BookMultipleSeats(int screeningId, List<int[]> selectedSeats, 
                                          int[,] seatIds, string guestName, string guestEmail)
    {
        BookingResult result = new BookingResult();
        
        // Clean up expire blocked seats
        _repository.CleanupExpiredBlocks();
        
        foreach (int[] selectedSeat in selectedSeats)
        {
            int seatId = seatIds[selectedSeat[0] - 1, selectedSeat[1] - 1];
            
            if (seatId == 0)
            {
                result.FailedSeats.Add($"Row {selectedSeat[0]}, Seat {selectedSeat[1]} (Invalid seat ID)");
                continue;
            }
            
            bool success = _repository.BookSeat(screeningId, seatId, guestName, guestEmail);
            
            if (success)
            {
                result.SuccessfulSeats.Add(selectedSeat);
            }
            else
            {
                result.FailedSeats.Add($"Row {selectedSeat[0]}, Seat {selectedSeat[1]}");
            }
        }
        
        result.IsSuccessful = result.FailedSeats.Count == 0;
        return result;
    }

    public bool BookSingleSeat(int screeningId, int seatId, string guestName, string guestEmail)
    {
        _repository.CleanupExpiredBlocks();
        return _repository.BookSeat(screeningId, seatId, guestName, guestEmail);
    }
}