using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;
using Cinema.UserInterface.Concrete;
using Cinema.Utils;

namespace Cinema;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        // init db
        CinemaDatabaseInitializer dbInitializer = new CinemaDatabaseInitializer();
        dbInitializer.InitializeDatabase();
        // dbInitializer.TestDataBase();
        
        // initialize services and helpers
        UserInputHandler inputHandler = new UserInputHandler();
        SqliteCinemaRepository repository = new SqliteCinemaRepository(AppConfig.ConnectionString);
        CinemaDataService dataService = new CinemaDataService(AppConfig.ConnectionString);
        BookingService bookingService = new BookingService(repository, dataService);
        PasswordService passwordService = new PasswordService();
        AuthenticationService authService = new AuthenticationService(AppConfig.ConnectionString, passwordService);
        SeatMapHelper seatMapHelper = new SeatMapHelper(dataService);
        ScreeningHelper screeningHelper = new ScreeningHelper(inputHandler, repository, dataService);
        
        // create start menu and run
        AuthMenu authMenu = new AuthMenu(inputHandler, authService);
        UserMenu userMenu = new UserMenu(inputHandler, repository, dataService, bookingService, authService, seatMapHelper, screeningHelper);
        MainMenu mainMenu = new MainMenu(inputHandler, repository, dataService, bookingService, authService, seatMapHelper, screeningHelper, authMenu,  userMenu);
        mainMenu.Start();

        Console.Clear();
    }
}