using Cinema.Entities;
using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete;

public class BookSeatsCommand : BaseBookingCommand
{
    public BookSeatsCommand(UserInputHandler inputHandler, ICinemaRepository repository, 
        CinemaDataService dataService, BookingService bookingService, 
        SeatMapHelper seatMapHelper, ScreeningHelper screeningHelper)
        : base(inputHandler, repository, dataService, bookingService, seatMapHelper, screeningHelper)
    {
    }

    public override string Name => "Book Seats (Guest)";
    public override string Description => "Book cinema seats as a guest";

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