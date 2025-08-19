using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;
using Cinema.UserInterface.Concrete;
using Cinema.Utils;

namespace Cinema;

/// <summary>
/// Main entry point for the Cinema Booking System application.
/// Initializes all services and starts the application.
/// </summary>
class Program
{
    
    /// <summary>
    /// Application entry point. Sets up dependency injection and starts the main menu.
    /// if the command argument "dbtest" is given it will try to fetch from the db
    /// to test the db connection
    /// </summary>
    /// <param name="args">Command line arguments. Use 'dbtest' to run database test only</param>
    static void Main(string[] args)
    {
        Console.Clear();
        
        // Initialize database
        CinemaDatabaseInitializer dbInitializer = new CinemaDatabaseInitializer();
        dbInitializer.InitializeDatabase();
        
        // Check for dbtest argument
        if (args.Length > 0 && args[0].ToLower() == "dbtest")
        {
            Console.Write("\nRunning database test...\n");
            dbInitializer.TestDataBase();
            Console.Write("\nDatabase test completed. Press any key to exit...\n");
            Console.ReadKey();
        }
        else
        {
            // Initialize services and helpers (Dependency Injection pattern)
            UserInputHandler inputHandler = new UserInputHandler();
            SqliteCinemaRepository repository = new SqliteCinemaRepository(AppConfig.ConnectionString);
            CinemaDataService dataService = new CinemaDataService(AppConfig.ConnectionString);
            BookingService bookingService = new BookingService(repository, dataService);
            PasswordService passwordService = new PasswordService();
            AuthenticationService authService = new AuthenticationService(AppConfig.ConnectionString, passwordService);
            SeatMapHelper seatMapHelper = new SeatMapHelper(dataService);
            ScreeningHelper screeningHelper = new ScreeningHelper(inputHandler, repository, dataService);
        
            // Create menus and start application
            AuthMenu authMenu = new AuthMenu(inputHandler, authService);
            UserMenu userMenu = new UserMenu(inputHandler, repository, dataService, bookingService, authService, seatMapHelper, screeningHelper);
            MainMenu mainMenu = new MainMenu(inputHandler, repository, dataService, bookingService, authService, seatMapHelper, screeningHelper, authMenu,  userMenu);
            mainMenu.Start();

            Console.Clear();    
        }
    }
}