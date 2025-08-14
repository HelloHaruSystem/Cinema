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
        // init db
        CinemaDatabaseInitializer dbInitializer = new CinemaDatabaseInitializer();
        dbInitializer.InitializeDatabase();
        // dbInitializer.TestDataBase();
        
        // initialize services
        UserInputHandler inputHandler = new UserInputHandler();
        SqliteCinemaRepository repository = new SqliteCinemaRepository(AppConfig.ConnectionString);
        CinemaDataService dataService = new CinemaDataService(AppConfig.ConnectionString);
        BookingService bookingService = new BookingService(repository, dataService);
        SeatMapHelper seatMapHelper = new SeatMapHelper(dataService);
        ScreeningHelper screeningHelper = new ScreeningHelper(inputHandler, repository, dataService);
        
        // create start menu and run
        MainMenu mainMenu = new MainMenu(inputHandler, repository, dataService, bookingService, seatMapHelper, screeningHelper);
        mainMenu.Start();

        Console.Clear();
    }
}