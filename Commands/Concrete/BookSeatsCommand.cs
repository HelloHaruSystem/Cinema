using Cinema.Entities;
using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete;

public class BookSeatsCommand : BaseCommand
{
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;
    private readonly SeatMapHelper _seatMapHelper;
    private readonly ScreeningHelper _screeningHelper;

    public BookSeatsCommand(UserInputHandler inputHandler, ICinemaRepository repository, CinemaDataService dataService,
                            SeatMapHelper seatMapHelper, ScreeningHelper screeningHelper)
        : base (inputHandler)
    {
        _repository = repository;
        _dataService = dataService;
        _seatMapHelper = seatMapHelper;
        _screeningHelper = screeningHelper;
    }

    public override string Name => "Book Seats (Guest)";
    public override string Description => "Book cinema seats as a guest";

    public override bool? Execute()
    {
        int numberOfSeats;
        
        InputHandler.Clear();
        
        try
        {
            // Show available screenings and let user select
            Screening? screening = _screeningHelper.SelectScreening("to book");
            if (screening == null)
            {
                Console.Write("No screening selected. Press any key to return...\n> ");
                Console.ReadKey();
                return true;
            }

            // Show seat map
            Hall? hall = _repository.GetHallForScreening(screening.Id);
            if (hall == null)
            {
                Console.WriteLine("Hall information not found. Press any key to return...\n> ");
                Console.ReadKey();
                return true;
            }

            bool[,] seatLayout = _seatMapHelper.CreateSeatLayoutArray(screening.Id, hall);
            int[,] seatIds = _seatMapHelper.CreateSeatIdArray(screening.Id, hall);
            
            _seatMapHelper.DisplaySeatMapWithArray(hall, seatLayout);
            
            // get the number 
            numberOfSeats = GetNumberOfSeats();
            List<int[]> selectedSeats = new List<int[]>();
            
            // get multiple seat selection
            for (int i = 0; i < numberOfSeats; i++)
            {
                int row;
                int seat;
                int[] selectedSeat;
                
                Console.Write("\nSelecting seat {0} of {1}:\n", i + 1, numberOfSeats);
                    
                selectedSeat = GetSeatSelectionWithArray(hall, seatLayout, selectedSeats);
                row = selectedSeat[0];
                seat = selectedSeat[1];
                
                selectedSeats.Add(selectedSeat);
            
                // ✅ ASSIGNMENT REQUIREMENT: Mark seat as taken in array
                seatLayout[row - 1, seat - 1] = true;
            
                // Show updated map after each selection
                if (i < numberOfSeats - 1)
                {
                    Console.Write("\nUpdated seat map:\n");
                    _seatMapHelper.DisplaySeatMapWithArray(hall, seatLayout);
                }
            }
            
            // Get guest information
            string[] guestNameAndEmail = InputHandler.GetGuestInformation();
            ProcessBookings(selectedSeats, seatIds, screening, guestNameAndEmail, numberOfSeats);
            
        }
        catch (Exception ex)
        {
            Console.Write("An error occurred while booking: {0}\n", ex.Message);
        }
        
        PressAnyKey();
        return true;
    }
    
    private void ProcessBookings(List<int[]> selectedSeats, int[,] seatIds, Screening screening, 
                                string[] guestInfo, int numberOfSeats)
    {
        // try to book selected seats
        bool allSuccessful = true;
        List<string> failedSeats = new List<string>();

        foreach (int[] selectedSeat in selectedSeats)
        {
            int seatId = seatIds[selectedSeat[0] - 1, selectedSeat[1] - 1];
            if (seatId == 0)
            {
                failedSeats.Add($"Row {selectedSeat[0]}, Seat {selectedSeat[1]} (Invalid seat ID)");
                allSuccessful = false;
                continue;
            }
                
            bool success = _repository.BookSeat(screening.Id, seatId, guestInfo[0],  guestInfo[1]);
            if (!success)
            {
                failedSeats.Add($"Row {selectedSeat[0]}, Seat {selectedSeat[1]}");
                allSuccessful = false;
            }
        }
            
        // Display booking results
        if (allSuccessful)
        {
            Console.Write("\nAll bookings successful!\n");
            Console.Write("Movie: {0}\n", GetMovieTitle(screening.MovieId));
            Console.Write("Time: {0:dd-MM-yyyy HH:mm}\n", screening.StartTime);
            Console.Write("Seats booked: \n");
            foreach (int[] selectedSeat in selectedSeats)
            {
                Console.Write("Row {0}, Seat {1}\n", selectedSeat[0], selectedSeat[1]);
            }
            Console.Write("Total Price: {0:F2} DKK\n", screening.Price * numberOfSeats);
            Console.Write("Name: {0}\n", guestInfo[0]);
            Console.Write("Email: {0}\n\n", guestInfo[1]);
        }
    }
    
    private int GetNumberOfSeats()
    {
        int numberOfSeats;
        
        Console.Write("\nHow many seats would you like to book? (1-10): ");
        numberOfSeats = InputHandler.GetMenuChoice(1, 10);
    
        return numberOfSeats;
    }

    private int[] GetSeatSelectionWithArray(Hall hall, bool[,] seatLayout, List<int[]> alreadySelected)
    {
        int row;
        int seat;
        int[] result = new int[2];
        bool validInput = false;

        while (!validInput)
        {
            result = InputHandler.GetSeatSelection(hall);
            row = result[0];
            seat = result[1];

            if (seatLayout[row - 1, seat - 1])
            {
                Console.Write("That seat is already taken! Please choose another seat\n");
            } else if (alreadySelected.Any(s => s[0] == row && s[1] == seat))
            {
                Console.WriteLine("You've already selected that seat! Please choose another seat\n");
            }
            else
            {
                validInput = true;
            }
        }
        
        return result;
    }

    private string GetMovieTitle(int movieId)
    {
        Movie? movie = _dataService.GetMovieById(movieId);
        return movie?.Title ?? $"Movie {movieId}";
    }
}