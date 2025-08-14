using Cinema.Entities;
using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete.User;

/// <summary>
/// Command for displaying a user's booking history with detailed information.
/// Shows comprehensive booking details including screening status and total spending.
/// </summary>
public class ViewUserBookingsCommand : BaseCommand
{
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;
    private readonly AuthenticationService _authService;

    /// <summary>
    /// Initializes the view user bookings command.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="repository">Repository for data access</param>
    /// <param name="dataService">Service for cinema data operations</param>
    /// <param name="authService">Service for authentication</param>
    public ViewUserBookingsCommand(UserInputHandler inputHandler, ICinemaRepository repository, 
                                  CinemaDataService dataService, AuthenticationService authService)
        : base(inputHandler)
    {
        _repository = repository;
        _dataService = dataService;
        _authService = authService;
    }

    /// <summary>
    /// Gets the command display name.
    /// </summary>
    public override string Name => "View My Bookings";
    
    /// <summary>
    /// Gets the command description.
    /// </summary>
    public override string Description => "View all your current bookings";

    /// <summary>
    /// Executes the command to display user's booking history.
    /// </summary>
    /// <returns>True to continue current menu</returns>
    public override bool? Execute()
    {
        InputHandler.Clear();
        
        try
        {
            DisplayUserBookings();
        }
        catch (Exception ex)
        {
            Console.Write("An error occurred while loading bookings: {0}\n", ex.Message);
        }

        PressAnyKey();
        return true;
    }

    /// <summary>
    /// Displays formatted user booking information with grouping and statistics.
    /// Shows bookings grouped by screening with status indicators and totals.
    /// </summary>
    private void DisplayUserBookings()
    {
        string header = AppConfig.CenterText($"BOOKINGS FOR {_authService.CurrentUser?.Username?.ToUpper() ?? "USER"}", AppConfig.MenuWidth);
        Console.Write("{0}{1}{0}\n", AppConfig.BorderChar, header);
        Console.Write("{0}", AppConfig.HeaderLine);

        var userBookings = _dataService.GetUserBookings(_authService.CurrentUser?.Id ?? throw new InvalidOperationException("User not logged in"));

        if (!userBookings.Any())
        {
            Console.Write("\nYou don't have any bookings yet.\n");
            Console.Write("Use the 'Book Seats' option to make your first booking!\n");
            return;
        }

        Console.Write("\nYour Bookings:\n");
        Console.Write("".PadRight(AppConfig.MenuWidth, '='));
        Console.Write("\n");

        double totalSpent = 0;
        var groupedBookings = userBookings
            .GroupBy(b => new { b.screening.Id, b.movie.Title, b.screening.StartTime, b.screening.Price })
            .OrderByDescending(g => g.Key.StartTime)
            .ToArray();

        foreach (var bookingGroup in groupedBookings)
        {
            // avoid compiler type checking with dynamic
            // so we can access .Title .StartTime .Price
            dynamic screening = bookingGroup.Key;
            List<(Bookings booking, Screening screening, Movie movie, Hall hall, Seat seat)> seats = bookingGroup.ToList();
            
            Console.Write("\n{0}\n", screening.Title);
            Console.Write("Date & Time: {0:dd-MM-yyyy HH:mm}\n", screening.StartTime);
            Console.Write("Hall: {0}\n", seats.First().hall.Name);
            Console.Write("Price per seat: {0:F2} DKK\n", screening.Price);
            
            Console.Write("Your seats: ");
            List<string> seatNumbers = seats.Select(s => $"Row {s.seat.RowNumber}, Seat {s.seat.SeatNumber}").ToList();
            Console.Write("{0}\n", string.Join(" | ", seatNumbers));
            
            double bookingTotal = screening.Price * seats.Count;
            totalSpent += bookingTotal;
            Console.Write("Booking total: {0:F2} DKK\n", bookingTotal);
            Console.Write("Booked on: {0:dd-MM-yyyy HH:mm}\n", seats.First().booking.BookingTime);
            
            // Show status based on screening time
            if (screening.StartTime < DateTime.Now)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("Status: Past screening\n");
                Console.ResetColor();
            }
            else if (screening.StartTime <= DateTime.Now.AddHours(2))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Status: Starting soon!\n");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Status: Upcoming\n");
                Console.ResetColor();
            }
            
            Console.Write("".PadRight(AppConfig.MenuWidth, '-'));
            Console.Write("\n");
        }

        Console.Write("\nSummary:\n");
        Console.Write("Total bookings: {0}\n", groupedBookings.Count());
        Console.Write("Total seats booked: {0}\n", userBookings.Count);
        Console.Write("Total amount spent: {0:F2} DKK\n", totalSpent);
    }
}