using Cinema.Commands;
using Cinema.Commands.Concrete;
using Cinema.Repository;
using Cinema.Services;
using Cinema.Utils;

namespace Cinema.UserInterface.Concrete;

public class MainMenu : Menu
{
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;
    private readonly SeatMapHelper _seatMapHelper;
    private readonly ScreeningHelper _screeningHelper;
    
    public MainMenu(UserInputHandler inputHandler, ICinemaRepository repository, CinemaDataService dataService,
                    SeatMapHelper seatMapHelper, ScreeningHelper screeningHelper)
        : base(inputHandler)
    {
        _repository = repository;
        _dataService = dataService;
        _seatMapHelper = seatMapHelper;
        _screeningHelper = screeningHelper;
    }
    protected override Dictionary<int, ICommand> Commands => new Dictionary<int, ICommand>()
    {
        { 1, new ViewMoviesCommand(InputHandler, _repository, _dataService) },
        { 2, new BookSeatsCommand(InputHandler, _repository, _dataService, _seatMapHelper,  _screeningHelper) },
        { 3, new ViewSeatMapCommand(InputHandler, _repository, _seatMapHelper, _screeningHelper) },
        //{ 4, new ViewSeatMapCommand(InputHandler, _repository, _dataService) },
        //{ 5, new StatisticsCommand(InputHandler, _repository, _dataService) },
        { 4, new ExitCommand(InputHandler) }
    };
    protected override int MaxOptions => Commands.Count;
    protected override string MenuHeader => "CINEMA BOOKING SYSTEM";

    public void Start()
    {
        bool? keepRunning = true;
        while (keepRunning == true)
        {
            keepRunning = Run();
        }
    }
}