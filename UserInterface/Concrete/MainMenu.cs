using Cinema.Commands;
using Cinema.Commands.Concrete;
using Cinema.Repository;
using Cinema.Services;

namespace Cinema.UserInterface.Concrete;

public class MainMenu : Menu
{
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;

    public MainMenu(UserInputHandler inputHandler, ICinemaRepository repository, CinemaDataService dataService)
        : base(inputHandler)
    {
        _repository = repository;
        _dataService = dataService;
    }
    protected override Dictionary<int, ICommand> Commands => new Dictionary<int, ICommand>()
    {
        { 1, new ViewMoviesCommand(InputHandler, _repository, _dataService) },
        //{ 2, new BookSeatsCommand(InputHandler, _repository, _dataService) },
        //{ 3, new LoginCommand(InputHandler) },
        //{ 4, new ViewSeatMapCommand(InputHandler, _repository, _dataService) },
        //{ 5, new StatisticsCommand(InputHandler, _repository, _dataService) },
        { 2, new ExitCommand(InputHandler) }
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