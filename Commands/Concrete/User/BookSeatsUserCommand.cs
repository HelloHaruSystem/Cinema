using Cinema.Entities;
using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete.User;

public class BookSeatsUserCommand : BaseCommand
{
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;
    private readonly BookingService _bookingService;
    private readonly SeatMapHelper _seatMapHelper;
    private readonly ScreeningHelper _screeningHelper;
    private readonly AuthenticationService _authService;

    public BookSeatsUserCommand(UserInputHandler inputHandler, ICinemaRepository repository, CinemaDataService dataService,
                               BookingService bookingService, SeatMapHelper seatMapHelper, ScreeningHelper screeningHelper,
                               AuthenticationService authService)
        : base(inputHandler)
    {
        _repository = repository;
        _dataService = dataService;
        _bookingService = bookingService;
        _seatMapHelper = seatMapHelper;
        _screeningHelper = screeningHelper;
        _authService = authService;
    }

    public override string Name => "Book Seats";
    public override string Description => "Book cinema seats with your account";

    // TODO: refactor so there is not this much duplicate code with BookSeatsCommand
    public override bool? Execute()
    {
        // Similar to BookSeatsCommand but uses user ID instead of guest info
        InputHandler.Clear();

        try
        {
            Screening? screening = _screeningHelper.SelectScreening("to book");
            if (screening == null)
            {
                Console.Write("No screening selected.\n");
                PressAnyKey();
                return true;
            }

            Hall? hall = _repository.GetHallForScreening(screening.Id);
            if (hall == null)
            {
                Console.Write("Hall information not found.\n");
                PressAnyKey();
                return true;
            }

            bool[,] seatLayout = _seatMapHelper.CreateSeatLayoutArray(screening.Id, hall);
            int[,] seatIds = _seatMapHelper.CreateSeatIdArray(screening.Id, hall);

            _seatMapHelper.DisplaySeatMapWithArray(hall, seatLayout);

            int numberOfSeats = GetNumberOfSeats();
            List<int[]> selectedSeats = new List<int[]>();

            for (int i = 0; i < numberOfSeats; i++)
            {
                Console.Write("\nSelecting seat {0} of {1}:\n", i + 1, numberOfSeats);
                int[] selectedSeat = GetSeatSelectionWithArray(hall, seatLayout, selectedSeats);
                selectedSeats.Add(selectedSeat);
                seatLayout[selectedSeat[0] - 1, selectedSeat[1] - 1] = true;

                if (i < numberOfSeats - 1)
                {
                    Console.Write("\nUpdated seat map:\n");
                    _seatMapHelper.DisplaySeatMapWithArray(hall, seatLayout);
                }
            }

            ProcessUserBookings(selectedSeats, seatIds, screening, numberOfSeats);
        }
        catch (Exception ex)
        {
            Console.Write("An error occurred while booking: {0}\n", ex.Message);
        }

        PressAnyKey();
        return true;
    }
    
    private void ProcessUserBookings(List<int[]> selectedSeats, int[,] seatIds, Screening screening, int numberOfSeats)
    {
        BookingResult result = _bookingService.BookMultipleSeatsForUser(
            screening.Id, selectedSeats, seatIds, _authService.CurrentUser.Id);

        if (result.IsSuccessful)
        {
            Console.Write("\nAll bookings successful!\n");
            Console.Write("Movie: {0}\n", _dataService.GetMovieById(screening.MovieId));
            Console.Write("Time: {0:dd-MM-yyyy HH:mm}\n", screening.StartTime);
            Console.Write("Seats booked: \n");
            foreach (int[] selectedSeat in result.SuccessfulSeats)
            {
                Console.Write("Row {0}, Seat {1}\n", selectedSeat[0], selectedSeat[1]);
            }
            Console.Write("Total Price: {0:F2} DKK\n", screening.Price * numberOfSeats);
            Console.Write("Booked by: {0}\n\n", _authService.CurrentUser.Username);
        }
        else
        {
            Console.Write("\nSome bookings failed:\n");
            foreach (string failedSeat in result.FailedSeats)
            {
                Console.Write("Failed: {0}\n", failedSeat);
            }
        }
    }

    private int GetNumberOfSeats()
    {
        Console.Write("\nHow many seats would you like to book? (1-10): ");
        return InputHandler.GetMenuChoice(1, 10);
    }

    private int[] GetSeatSelectionWithArray(Hall hall, bool[,] seatLayout, List<int[]> alreadySelected)
    {
        int[] result = new int[2];
        bool validInput = false;

        while (!validInput)
        {
            result = InputHandler.GetSeatSelection(hall);
            int row = result[0];
            int seat = result[1];

            if (seatLayout[row - 1, seat - 1])
            {
                Console.Write("That seat is already taken! Please choose another seat\n");
            }
            else if (alreadySelected.Any(s => s[0] == row && s[1] == seat))
            {
                Console.Write("You've already selected that seat! Please choose another seat\n");
            }
            else
            {
                validInput = true;
            }
        }

        return result;
    }
}