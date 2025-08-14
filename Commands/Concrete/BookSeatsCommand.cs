using Cinema.Entities;
using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;

namespace Cinema.Commands.Concrete;

public class BookSeatsCommand : BaseCommand
{
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;

    public BookSeatsCommand(UserInputHandler inputHandler, ICinemaRepository repository, CinemaDataService dataService)
        : base (inputHandler)
    {
        _repository = repository;
        _dataService = dataService;
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
            Screening? screening = SelectScreening();
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

            bool[,] seatLayout = CreateSeatLayoutArray(screening.Id, hall);
            int[,] seatIds = CreateSeatIdArray(screening.Id, hall);
            
            DisplaySeatMapWithArray(hall, seatLayout);
            
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
                    DisplaySeatMapWithArray(hall, seatLayout);
                }
            }
            
            // Get guest information
            string[] guestNameAndEmail = InputHandler.GetGuestInformation();

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
                
                bool success = _repository.BookSeat(screening.Id, seatId, guestNameAndEmail[0],  guestNameAndEmail[1]);
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
                Console.Write("Name: {0}\n", guestNameAndEmail[0]);
                Console.Write("Email: {0}\n\n", guestNameAndEmail[1]);
            }
        }
        catch (Exception ex)
        {
            Console.Write("An error occurred while booking: {0}\n", ex.Message);
        }
        
        PressAnyKey();
        return true;
    }

    private bool[,] CreateSeatLayoutArray(int screeningId, Hall hall)
    {
        // false = available, true = taken
        bool[,] seatLayout = new bool[hall.Rows, hall.SeatsPerRow];
        
        var seatsWithStatus = _dataService.LoadSeatsWithStatus(hall.Id, screeningId);

        foreach (var (seat, isBooked, isBlocked) in seatsWithStatus)
        {
            int arrayRow = seat.RowNumber - 1;
            int arraySeat = seat.SeatNumber - 1;

            if (arrayRow >= 0 && arrayRow < hall.Rows &&
                arraySeat >= 0 && arraySeat < hall.SeatsPerRow)
            {
                seatLayout[arrayRow, arraySeat] = isBooked ||  isBlocked; // true taken
            }
        }
        
        return seatLayout;
    }
    
    private int[,] CreateSeatIdArray(int screeningId, Hall hall)
    {
        int[,] seatIds = new int[hall.Rows, hall.SeatsPerRow];
    
        var seatsWithStatus = _dataService.LoadSeatsWithStatus(hall.Id, screeningId);
    
        foreach (var (seat, _, _) in seatsWithStatus)
        {
            int arrayRow = seat.RowNumber - 1;
            int arraySeat = seat.SeatNumber - 1;
        
            if (arrayRow >= 0 && arrayRow < hall.Rows &&
                arraySeat >= 0 && arraySeat < hall.SeatsPerRow)
            {
                seatIds[arrayRow, arraySeat] = seat.Id;
            }
        }
    
        return seatIds;
    }

    private void DisplaySeatMapWithArray(Hall hall, bool[,] seatLayout)
    {
        Console.Write("\nSeat Map for {0}\n", hall.Name);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("▮");
        Console.ResetColor();
        Console.Write(" = Taken | ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("▮");
        Console.ResetColor();
        Console.Write(" = Available\n");
        
        // column numbers
        Console.Write("   ");
        for (int seat = 1; seat <= hall.SeatsPerRow; seat++)
        {
            Console.Write($"{seat,2} ");
        }
        Console.Write("\n");
        
        // rows with seats
        for (int row = 0; row < hall.Rows; row++)
        {
            Console.Write($"{row + 1,2} ");

            for (int seat = 0; seat < hall.SeatsPerRow; seat++)
            {
                if (seatLayout[row, seat]) // true = taken
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("▮  ");
                }
                else // false = available
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("▮  ");
                }
                Console.ResetColor();
            }
            Console.Write("\n");
        }
    }

    private Screening? SelectScreening()
    {
        List<Screening> screenings = _repository.GetAllScreenings()
            .Where(s => s.StartTime > DateTime.Now)
            .OrderBy(s => s.StartTime)
            .ToList();

        if (!screenings.Any())
        {
            Console.Write("No upcoming screenings available\n");
            return null;
        }

        Console.Write("Available Screenings:\n");
        for (int i = 0; i < screenings.Count; i++)
        {
            Screening screening = screenings[i];
            string movieTitle = GetMovieTitle(screening.MovieId);
            Console.Write("{0}. {1}\n", i + 1,  movieTitle);
            Console.Write("{0:dd-MM-yyyy HH:mm} |   {1:F2} DKK |   Hall {2}\n\n",
                screening.StartTime, screening.Price, screening.ScreenHallId);
            Console.Write("\n");
        }

        Console.Write("Select screening (1-{0}) or 0 to cancel:\n", screenings.Count);
        int choice = InputHandler.GetMenuChoice(1, screenings.Count);
        
        return screenings[choice - 1];
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