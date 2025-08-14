using Cinema.Entities;
using Cinema.Repository;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete;

/// <summary>
/// Command for viewing seat statistics for any screening.
/// </summary>
public class StatisticsCommand : BaseCommand
{
    private readonly ICinemaRepository _repository;
    private readonly SeatMapHelper _seatMapHelper;
    private readonly ScreeningHelper _screeningHelper;

    /// <summary>
    /// Initializes the statistics command.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="repository">Repository for data access</param>
    /// <param name="seatMapHelper">Helper for seat map operations</param>
    /// <param name="screeningHelper">Helper for screening operations</param>
    public StatisticsCommand(UserInputHandler inputHandler, ICinemaRepository repository, 
        SeatMapHelper seatMapHelper, ScreeningHelper screeningHelper)
        : base(inputHandler)
    {
        _repository = repository;
        _seatMapHelper = seatMapHelper;
        _screeningHelper = screeningHelper;
    }

    
    /// <summary>
    /// Gets the command display name.
    /// </summary>
    public override string Name => "View Statistics";
    
    /// <summary>
    /// Gets the command description.
    /// </summary>
    public override string Description => "View seat statistics for any screening";
    
    /// <summary>
    /// Executes the command to display statistics for a selected screening.
    /// </summary>
    /// <returns>True to continue current menu</returns>
    public override bool? Execute()
    {
        InputHandler.Clear();

        try
        {
            Screening? screening = _screeningHelper.SelectScreening("to view statistics");
            if (screening == null)
            {
                Console.Write("No screening selected\n");
                PressAnyKey();
                return true;
            }

            Hall? hall = _repository.GetHallForScreening(screening.Id);
            if (hall == null)
            {
                Console.Write("No information found\n");
                PressAnyKey();
                return true;
            }

            _screeningHelper.DisplayScreeningInfo(screening);

            bool[,] seatLayout = _seatMapHelper.CreateSeatLayoutArray(screening.Id, hall);
            _seatMapHelper.DisplaySeatStatistics(hall, seatLayout);
        }
        catch (Exception ex)
        {
            Console.Write("An error occured: {0}\n", ex.Message);
        }
        
        PressAnyKey();
        return true;
    }
}