using Cinema.Entities;
using Cinema.Repository;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete;

public class ViewSeatMapCommand : BaseCommand
{
    private readonly ICinemaRepository _repository;
    private readonly SeatMapHelper _seatMapHelper;
    private readonly ScreeningHelper _screeningHelper;

    public ViewSeatMapCommand(UserInputHandler inputHandler, ICinemaRepository repository, 
        SeatMapHelper seatMapHelper, ScreeningHelper screeningHelper)
        : base(inputHandler)
    {
        _repository = repository;
        _seatMapHelper = seatMapHelper;
        _screeningHelper = screeningHelper;
    }
    
    public override string Name => "View Seat Map";
    public override string Description => "View current seat availability for any screening";
    
    public override bool? Execute()
    {
        InputHandler.Clear();

        try
        {
            Screening? screening = _screeningHelper.SelectScreening("to view seat map");
            if (screening == null)
            {
                Console.Write("No screening selected.\n");
                PressAnyKey();
                return true;
            }
            
            Hall? hall = _repository.GetHallForScreening(screening.Id);
            if (hall == null)
            {
                Console.Write("❌ Hall information not found.\n");
                PressAnyKey();
                return true;
            }
            
            _screeningHelper.DisplayScreeningInfo(screening);
            
            bool[,] seatLayout = _seatMapHelper.CreateSeatLayoutArray(screening.Id, hall);
            _seatMapHelper.DisplaySeatMapWithArray(hall, seatLayout);
            _seatMapHelper.DisplaySeatStatistics(hall, seatLayout);
            
            PressAnyKey();
            return true;
        }
        catch (Exception ex)
        {
            Console.Write("An error occurred: {0}\n", ex.Message);
            PressAnyKey();
            return true;
        }
    }
}