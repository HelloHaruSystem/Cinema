using Cinema.Entities;
using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete;

/// <summary>
/// Command for booking seats as a guest user.
/// Extends BaseBookingCommand to handle guest-specific booking process.
/// </summary>
public class BookSeatsCommand : BaseBookingCommand
{
    
    /// <summary>
    /// Initializes the guest booking command.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="repository">Repository for data access</param>
    /// <param name="dataService">Service for cinema data operations</param>
    /// <param name="bookingService">Service for booking operations</param>
    /// <param name="seatMapHelper">Helper for seat map operations</param>
    /// <param name="screeningHelper">Helper for screening operations</param>
    public BookSeatsCommand(UserInputHandler inputHandler, ICinemaRepository repository, 
        CinemaDataService dataService, BookingService bookingService, 
        SeatMapHelper seatMapHelper, ScreeningHelper screeningHelper)
        : base(inputHandler, repository, dataService, bookingService, seatMapHelper, screeningHelper)
    {
        
    }

    /// <summary>
    /// Gets the command display name.
    /// </summary>
    public override string Name => "Book Seats (Guest)";
    
    /// <summary>
    /// Gets the command description.
    /// </summary>
    public override string Description => "Book cinema seats as a guest";

    
    /// <summary>
    /// Processes the booking for guest users by collecting guest information.
    /// </summary>
    /// <param name="selectedSeats">List of selected seat positions</param>
    /// <param name="seatIds">2D array mapping positions to seat IDs</param>
    /// <param name="screening">The selected screening</param>
    /// <param name="numberOfSeats">Total number of seats being booked</param>
    protected override void ProcessBooking(List<int[]> selectedSeats, int[,] seatIds, 
        Screening screening, int numberOfSeats)
    {
        // Get guest information
        string[] guestInfo = InputHandler.GetGuestInformation();
        
        // Perform the booking
        BookingResult result = _bookingService.BookMultipleSeats(
            screening.Id, selectedSeats, seatIds, guestInfo[0], guestInfo[1]);

        // Display the result
        string bookedBy = $"{guestInfo[0]} ({guestInfo[1]})";
        DisplayBookingResult(result, screening, numberOfSeats, bookedBy);
    }
}