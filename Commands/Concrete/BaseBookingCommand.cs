using Cinema.Entities;
using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete;

public abstract class BaseBookingCommand : BaseCommand
{
    protected readonly ICinemaRepository _repository;
    protected readonly CinemaDataService _dataService;
    protected readonly BookingService _bookingService;
    protected readonly SeatMapHelper _seatMapHelper;
    protected readonly ScreeningHelper _screeningHelper;

    protected BaseBookingCommand(UserInputHandler inputHandler, ICinemaRepository repository, 
        CinemaDataService dataService, BookingService bookingService, 
        SeatMapHelper seatMapHelper, ScreeningHelper screeningHelper)
        : base(inputHandler)
    {
        _repository = repository;
        _dataService = dataService;
        _bookingService = bookingService;
        _seatMapHelper = seatMapHelper;
        _screeningHelper = screeningHelper;
    }
    
    public override bool? Execute()
    {
        InputHandler.Clear();

        try
        {
            // Select screening
            Screening? screening = _screeningHelper.SelectScreening("to book");
            if (screening == null)
            {
                Console.Write("No screening selected.\n");
                PressAnyKey();
                return true;
            }

            // Get hall information
            Hall? hall = _repository.GetHallForScreening(screening.Id);
            if (hall == null)
            {
                Console.Write("Hall information not found.\n");
                PressAnyKey();
                return true;
            }

            // Display seat map and get seat layout
            bool[,] seatLayout = _seatMapHelper.CreateSeatLayoutArray(screening.Id, hall);
            int[,] seatIds = _seatMapHelper.CreateSeatIdArray(screening.Id, hall);
            _seatMapHelper.DisplaySeatMapWithArray(hall, seatLayout);

            // Get number of seats to book
            int numberOfSeats = GetNumberOfSeats();
            List<int[]> selectedSeats = new List<int[]>();

            // Seat selection loop
            for (int i = 0; i < numberOfSeats; i++)
            {
                Console.Write("\nSelecting seat {0} of {1}:\n", i + 1, numberOfSeats);
                int[] selectedSeat = GetSeatSelectionWithArray(hall, seatLayout, selectedSeats);
                selectedSeats.Add(selectedSeat);
                
                // Mark seat as taken in the visual array
                seatLayout[selectedSeat[0] - 1, selectedSeat[1] - 1] = true;

                // Show updated map if more seats to select
                if (i < numberOfSeats - 1)
                {
                    Console.Write("\nUpdated seat map:\n");
                    _seatMapHelper.DisplaySeatMapWithArray(hall, seatLayout);
                }
            }

            // Process the booking (implemented by derived classes)
            ProcessBooking(selectedSeats, seatIds, screening, numberOfSeats);
        }
        catch (Exception ex)
        {
            Console.Write("An error occurred while booking: {0}\n", ex.Message);
        }

        PressAnyKey();
        return true;
    }

    // Abstract method that derived classes must implement
    protected abstract void ProcessBooking(List<int[]> selectedSeats, int[,] seatIds, 
                                         Screening screening, int numberOfSeats);

    // Shared helper methods
    protected int GetNumberOfSeats()
    {
        Console.Write("\nHow many seats would you like to book? (1-10): ");
        return InputHandler.GetMenuChoice(1, 10);
    }

    protected int[] GetSeatSelectionWithArray(Hall hall, bool[,] seatLayout, List<int[]> alreadySelected)
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

    // Shared method to display booking results
    protected void DisplayBookingResult(BookingResult result, Screening screening, 
                                      int numberOfSeats, string bookedBy)
    {
        if (result.IsSuccessful)
        {
            Console.Write("\nAll bookings successful!\n");
            Console.Write("Movie: {0}\n", _dataService.GetMovieTitle(screening.MovieId));
            Console.Write("Time: {0:dd-MM-yyyy HH:mm}\n", screening.StartTime);
            Console.Write("Seats booked:\n");
            foreach (int[] selectedSeat in result.SuccessfulSeats)
            {
                Console.Write("Row {0}, Seat {1}\n", selectedSeat[0], selectedSeat[1]);
            }
            Console.Write("Total Price: {0:F2} DKK\n", screening.Price * numberOfSeats);
            Console.Write("Booked by: {0}\n\n", bookedBy);
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
}