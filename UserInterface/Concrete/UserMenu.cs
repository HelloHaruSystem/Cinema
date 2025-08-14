using Cinema.Commands;
using Cinema.Commands.Concrete;
using Cinema.Commands.Concrete.User;
using Cinema.Repository;
using Cinema.Services;
using Cinema.Utils;

namespace Cinema.UserInterface.Concrete;

public class UserMenu : Menu
{
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;
    private readonly BookingService _bookingService;
    private readonly AuthenticationService _authService;
    private readonly SeatMapHelper _seatMapHelper;
    private readonly ScreeningHelper _screeningHelper;

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
    
    protected override Dictionary<int, ICommand> Commands => new Dictionary<int, ICommand>()
    {
        { 1, new ViewMoviesCommand(InputHandler, _repository, _dataService) },
        { 2, new BookSeatsUserCommand(InputHandler, _repository, _dataService, _bookingService, _seatMapHelper, _screeningHelper, _authService) },
        //{ 3, new ViewUserBookingsCommand(InputHandler, _repository, _dataService, _authService) },
        { 4, new ViewSeatMapCommand(InputHandler, _repository, _seatMapHelper, _screeningHelper) },
        { 5, new LogoutCommand(InputHandler, _authService) }
    };
    
    protected override int MaxOptions => Commands.Count;
    protected override string MenuHeader => $"WELCOME {_authService.CurrentUser?.Username?.ToUpper()}";
}