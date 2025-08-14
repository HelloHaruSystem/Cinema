using Cinema.Entities;
using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete.User;

public class BookSeatsUserCommand : BaseBookingCommand
{
    private readonly AuthenticationService _authService;

    public BookSeatsUserCommand(UserInputHandler inputHandler, ICinemaRepository repository, 
        CinemaDataService dataService, BookingService bookingService, 
        SeatMapHelper seatMapHelper, ScreeningHelper screeningHelper,
        AuthenticationService authService)
        : base(inputHandler, repository, dataService, bookingService, seatMapHelper, screeningHelper)
    {
        _authService = authService;
    }

    public override string Name => "Book Seats";
    public override string Description => "Book cinema seats with your account";

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