using Cinema.Entities;
using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands;

/// <summary>
/// Abstract base class for seat booking commands using the Template Method pattern.
/// Defines the common booking flow while allowing derived classes to customize the booking process.
/// </summary>
public abstract class BaseBookingCommand : BaseCommand
{
    protected readonly ICinemaRepository _repository;
    protected readonly CinemaDataService _dataService;
    protected readonly BookingService _bookingService;
    protected readonly SeatMapHelper _seatMapHelper;
    protected readonly ScreeningHelper _screeningHelper;

    /// <summary>
    /// Initializes the base booking command with required services.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="repository">Repository for data access</param>
    /// <param name="dataService">Service for cinema data operations</param>
    /// <param name="bookingService">Service for booking operations</param>
    /// <param name="seatMapHelper">Helper for seat map operations</param>
    /// <param name="screeningHelper">Helper for screening operations</param>
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
    
    /// <summary>
    /// Template method defining the booking process flow.
    /// Handles screening selection, seat map display, seat selection, and delegates final booking to derived classes.
    /// </summary>
    /// <returns>True to continue menu, false to exit, null to quit application</returns>
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

    /// <summary>
    /// Abstract method that derived classes must implement to handle the actual booking.
    /// Template Method pattern - the specific booking logic varies by implementation.
    /// </summary>
    /// <param name="selectedSeats">List of selected seat positions [row, seat]</param>
    /// <param name="seatIds">2D array mapping positions to seat IDs</param>
    /// <param name="screening">The selected screening</param>
    /// <param name="numberOfSeats">Total number of seats being booked</param>
    protected abstract void ProcessBooking(List<int[]> selectedSeats, int[,] seatIds, 
                                         Screening screening, int numberOfSeats);

    /// <summary>
    /// Gets the number of seats the user wants to book.
    /// </summary>
    /// <returns>Number of seats (1-10)</returns>
    protected int GetNumberOfSeats()
    {
        Console.Write("\nHow many seats would you like to book? (1-10): ");
        return InputHandler.GetMenuChoice(1, 10);
    }

    /// <summary>
    /// Gets a seat selection from the user with validation against already taken/selected seats.
    /// </summary>
    /// <param name="hall">Hall information for validation</param>
    /// <param name="seatLayout">Current seat availability layout</param>
    /// <param name="alreadySelected">Previously selected seats in this booking</param>
    /// <returns>Selected seat position [row, seat]</returns>
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

    /// <summary>
    /// Displays the result of a booking operation with success/failure details.
    /// </summary>
    /// <param name="result">Booking result object</param>
    /// <param name="screening">The screening that was booked</param>
    /// <param name="numberOfSeats">Total number of seats attempted</param>
    /// <param name="bookedBy">Name/identifier of who made the booking</param>
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