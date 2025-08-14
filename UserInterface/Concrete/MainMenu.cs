using Cinema.Commands;
using Cinema.Commands.Concrete;
using Cinema.Repository;
using Cinema.Services;
using Cinema.Utils;

namespace Cinema.UserInterface.Concrete;

public class MainMenu : Menu
{
    // services
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;
    private readonly BookingService _bookingService;
    private readonly AuthenticationService _authService;
    
    // helpers
    private readonly SeatMapHelper _seatMapHelper;
    private readonly ScreeningHelper _screeningHelper;
    
    // menus
    private readonly AuthMenu _authMenu;
    private readonly UserMenu _userMenu;
    
    public MainMenu(UserInputHandler inputHandler, ICinemaRepository repository, CinemaDataService dataService,
                    BookingService bookingService, AuthenticationService authService, SeatMapHelper seatMapHelper,
                    ScreeningHelper screeningHelper, AuthMenu authMenu, UserMenu userMenu)
        : base(inputHandler)
    {
        _repository = repository;
        _dataService = dataService;
        _bookingService = bookingService;
        _authService = authService;
        
        _seatMapHelper = seatMapHelper;
        _screeningHelper = screeningHelper;
        
        _authMenu = authMenu;
        _userMenu = userMenu;
    }
    protected override Dictionary<int, ICommand> Commands => new Dictionary<int, ICommand>()
    {
        { 1, new ViewMoviesCommand(InputHandler, _repository, _dataService) },
        { 2, new BookSeatsCommand(InputHandler, _repository, _dataService, _bookingService, _seatMapHelper,  _screeningHelper) },
        { 3, new SubMenuCommand(InputHandler, _authMenu, "User Login/Register", "Login or create account") },
        { 4, new ViewSeatMapCommand(InputHandler, _repository, _seatMapHelper, _screeningHelper) },
        { 5, new StaatisticsCommand(InputHandler, _repository, _seatMapHelper,  _screeningHelper) },
        { 6, new ExitCommand(InputHandler) }
    };
    protected override int MaxOptions => Commands.Count;
    protected override string MenuHeader => "CINEMA BOOKING SYSTEM";

    public void Start()
    {
        bool? keepRunning = true;
        while (keepRunning == true)
        {
            // Check if user just logged in from auth menu
            if (_authService.IsLoggedIn())
            {
                // Show user menu
                bool? userMenuResult = true;
                while (userMenuResult == true)
                {
                    userMenuResult = _userMenu.Run();
                }
            
                // If user logged out, continue with main menu
                // If userMenuResult is null, exit application
                if (userMenuResult == null)
                {
                    keepRunning = null;
                }
            }
            else
            {
                // Show main menu
                keepRunning = Run();
            }
        }
    }
}