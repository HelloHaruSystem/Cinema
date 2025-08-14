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

            DisplaySeatMap(screening.Id, hall);

            // Get seat selection
            int[] rowSeat = InputHandler.GetSeatSelection(hall);

            // Get guest information
            string[] guestNameAndEmail = InputHandler.GetGuestInformation();

            // try to book
            var seatId = _repository.GetSeatId(hall.Id, rowSeat[0], rowSeat[1]);
            if (seatId == null)
            {
                Console.Write("Invalid seat selection\n");
            }
            else
            {
                bool success = _repository.BookSeat(screening.Id, seatId.Value, guestNameAndEmail[0], guestNameAndEmail[1]);

                if (success)
                {
                    Console.Write("\n Booking Successful!");
                    Console.Write("Movie: {0}", GetMovieTitle(screening.MovieId));
                    Console.Write("Time: {0:dd-MM-yyyy HH:mm}", screening.StartTime);
                    Console.Write("Seat: Row {0}, Seat {1}", rowSeat[0], rowSeat[1]);
                    Console.Write("Price: {0:F0} DKK", screening.Price);
                    Console.Write("Name: {0}\n", guestNameAndEmail[0]);
                }
                else
                {
                    Console.Write("\n Booking Failed! Seat may already be taken\n");
                }
            }
        }
        catch (Exception ex)
        {
            Console.Write("An error occurred while booking: {0}\n", ex.Message);
        }
        
        PressAnyKey();
        return true;
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

    private void DisplaySeatMap(int screeningId, Hall hall)
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
        
        List<(Seat seat, bool isBooked, bool isBlocked)> seatsWithStatus = _dataService.LoadSeatsWithStatus(hall.Id, screeningId);
        
        // column numbers
        Console.Write("   ");
        for (int seat = 1; seat <= hall.SeatsPerRow; seat++)
        {
            Console.Write($"{seat,2} ");
        }
        Console.Write("\n");
        
        // rows with seats
        for (int row = 1; row <= hall.Rows; row++)
        {
            Console.Write($"{row,2} ");

            for (int seat = 1; seat <= hall.SeatsPerRow; seat++)
            {
                var seatStatus = seatsWithStatus.FirstOrDefault(s => 
                    s.seat.RowNumber == row && s.seat.SeatNumber == seat);

                if (seatStatus.seat != null)
                {
                    if (seatStatus.isBooked)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("▮  ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("▮  ");
                    }
                }
                Console.ResetColor();
            }
            Console.Write("\n");
        }
    }

    private string GetMovieTitle(int movieId)
    {
        Movie? movie = _dataService.GetMovieById(movieId);
        return movie?.Title ?? $"Movie {movieId}";
    }
}