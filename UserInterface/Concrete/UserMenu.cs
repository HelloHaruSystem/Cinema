using Cinema.Commands;
using Cinema.Commands.Concrete;
using Cinema.Commands.Concrete.User;
using Cinema.Repository;
using Cinema.Services;
using Cinema.Utils;

namespace Cinema.UserInterface.Concrete;

/// <summary>
/// Menu for authenticated users with personalized functionality.
/// Provides access to user-specific features like booking history and account-based bookings.
/// </summary>
public class UserMenu : Menu
{
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;
    private readonly BookingService _bookingService;
    private readonly AuthenticationService _authService;
    private readonly SeatMapHelper _seatMapHelper;
    private readonly ScreeningHelper _screeningHelper;

    
    /// <summary>
    /// Initializes the user menu with all required services.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="repository">Repository for data access</param>
    /// <param name="dataService">Service for cinema data operations</param>
    /// <param name="bookingService">Service for booking operations</param>
    /// <param name="authService">Service for authentication</param>
    /// <param name="seatMapHelper">Helper for seat map operations</param>
    /// <param name="screeningHelper">Helper for screening operations</param>
    public UserMenu(UserInputHandler inputHandler, ICinemaRepository repository, CinemaDataService dataService,
        BookingService bookingService, AuthenticationService authService, SeatMapHelper seatMapHelper, 
        ScreeningHelper screeningHelper)
        : base(inputHandler)
    {
        _repository = repository;
        
        _dataService = dataService;
        _bookingService = bookingService;
        _authService = authService;
        
        _seatMapHelper = seatMapHelper;
        _screeningHelper = screeningHelper;
    }
    
    /// <summary>
    /// Gets the available commands for authenticated users.
    /// </summary>
    protected override Dictionary<int, ICommand> Commands => new Dictionary<int, ICommand>()
    {
        { 1, new ViewMoviesCommand(InputHandler, _repository, _dataService) },
        { 2, new BookSeatsUserCommand(InputHandler, _repository, _dataService, _bookingService, _seatMapHelper, _screeningHelper, _authService) },
        { 3, new ViewUserBookingsCommand(InputHandler, _repository, _dataService, _authService) },
        { 4, new ViewSeatMapCommand(InputHandler, _repository, _seatMapHelper, _screeningHelper) },
        { 5, new LogoutCommand(InputHandler, _authService) }
    };
    
    /// <summary>
    /// Gets the maximum number of menu options.
    /// </summary>
    protected override int MaxOptions => Commands.Count;
    
    /// <summary>
    /// Gets the personalized menu header with the user's name.
    /// </summary>
    protected override string MenuHeader => $"WELCOME {_authService.CurrentUser?.Username?.ToUpper()}";
}