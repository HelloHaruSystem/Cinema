using Cinema.Entities;
using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete.User;

/// <summary>
/// Command for booking seats as an authenticated user.
/// Extends BaseBookingCommand to handle user-specific booking process.
/// </summary>
public class BookSeatsUserCommand : BaseBookingCommand
{
    private readonly AuthenticationService _authService;

    /// <summary>
    /// Initializes the user booking command.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="repository">Repository for data access</param>
    /// <param name="dataService">Service for cinema data operations</param>
    /// <param name="bookingService">Service for booking operations</param>
    /// <param name="seatMapHelper">Helper for seat map operations</param>
    /// <param name="screeningHelper">Helper for screening operations</param>
    /// <param name="authService">Service for authentication</param>
    public BookSeatsUserCommand(UserInputHandler inputHandler, ICinemaRepository repository, 
        CinemaDataService dataService, BookingService bookingService, 
        SeatMapHelper seatMapHelper, ScreeningHelper screeningHelper,
        AuthenticationService authService)
        : base(inputHandler, repository, dataService, bookingService, seatMapHelper, screeningHelper)
    {
        _authService = authService;
    }

    /// <summary>
    /// Gets the command display name.
    /// </summary>
    public override string Name => "Book Seats";
    
    /// <summary>
    /// Gets the command description.
    /// </summary>
    public override string Description => "Book cinema seats with your account";

    /// <summary>
    /// Processes the booking for authenticated users using their account.
    /// </summary>
    /// <param name="selectedSeats">List of selected seat positions</param>
    /// <param name="seatIds">2D array mapping positions to seat IDs</param>
    /// <param name="screening">The selected screening</param>
    /// <param name="numberOfSeats">Total number of seats being booked</param>
    protected override void ProcessBooking(List<int[]> selectedSeats, int[,] seatIds, 
        Screening screening, int numberOfSeats)
    {
        // Perform the booking for authenticated user
        BookingResult result = _bookingService.BookMultipleSeatsForUser(
            screening.Id, selectedSeats, seatIds, _authService.CurrentUser?.Id ?? throw new InvalidOperationException("User not logged in"));

        // Display the result
        string bookedBy = _authService.CurrentUser.Username ?? "Unknown User";
        DisplayBookingResult(result, screening, numberOfSeats, bookedBy);
    }
}