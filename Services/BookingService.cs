using Cinema.Repository;
using Cinema.Utils;

namespace Cinema.Services;

/// <summary>
/// Handles cinema seat booking operations.
/// Provides methods for booking single or multiple seats for guests and registered users.
/// </summary>
public class BookingService
{
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;

    /// <summary>
    /// Initializes the booking service.
    /// </summary>
    /// <param name="repository">Repository for data access</param>
    /// <param name="dataService">Service for cinema data operations</param>
    public BookingService(ICinemaRepository repository, CinemaDataService dataService)
    {
        _repository = repository;
        _dataService = dataService;
    }

    /// <summary>
    /// Books multiple seats for a guest user.
    /// </summary>
    /// <param name="screeningId">The screening to book for</param>
    /// <param name="selectedSeats">List of seat positions [row, seat]</param>
    /// <param name="seatIds">2D array mapping seat positions to seat IDs</param>
    /// <param name="guestName">Guest's name</param>
    /// <param name="guestEmail">Guest's email</param>
    /// <returns>Booking result with success/failure details</returns>
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
    
    /// <summary>
    /// Books multiple seats for a registered user.
    /// </summary>
    /// <param name="screeningId">The screening to book for</param>
    /// <param name="selectedSeats">List of seat positions [row, seat]</param>
    /// <param name="seatIds">2D array mapping seat positions to seat IDs</param>
    /// <param name="userId">ID of the registered user</param>
    /// <returns>Booking result with success/failure details</returns>
    public BookingResult BookMultipleSeatsForUser(int screeningId, List<int[]> selectedSeats, 
        int[,] seatIds, int userId)
    {
        BookingResult result = new BookingResult();
        _repository.CleanupExpiredBlocks();

        foreach (int[] selectedSeat in selectedSeats)
        {
            int seatId = seatIds[selectedSeat[0] - 1, selectedSeat[1] - 1];
        
            if (seatId == 0)
            {
                result.FailedSeats.Add($"Row {selectedSeat[0]}, Seat {selectedSeat[1]} (Invalid seat ID)");
                continue;
            }
        
            bool success = _repository.BookSeat(screeningId, seatId, null, null, userId);
        
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

    /// <summary>
    /// Books a single seat for a guest user.
    /// </summary>
    /// <param name="screeningId">The screening to book for</param>
    /// <param name="seatId">The seat ID to book</param>
    /// <param name="guestName">Guest's name</param>
    /// <param name="guestEmail">Guest's email</param>
    /// <returns>True if booking was successful, false otherwise</returns>
    public bool BookSingleSeat(int screeningId, int seatId, string guestName, string guestEmail)
    {
        _repository.CleanupExpiredBlocks();
        return _repository.BookSeat(screeningId, seatId, guestName, guestEmail);
    }
}