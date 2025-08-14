using Cinema.Entities;
using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;

namespace Cinema.Utils;

/// <summary>
/// Helper class for screening-related operations.
/// Handles screening selection and display functionality.
/// </summary>
public class ScreeningHelper
{
    private readonly UserInputHandler _inputHandler;
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;

    /// <summary>
    /// Initializes the screening helper.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="repository">Repository for data access</param>
    /// <param name="dataService">Service for cinema data operations</param>
    public ScreeningHelper(UserInputHandler inputHandler, ICinemaRepository repository, CinemaDataService dataService)
    {
        _inputHandler = inputHandler;
        _repository = repository;
        _dataService = dataService;
    }
    
    /// <summary>
    /// Displays available screenings and allows user to select one.
    /// </summary>
    /// <param name="purpose">Optional text describing the purpose of selection</param>
    /// <returns>Selected screening or null if cancelled</returns>
    public Screening? SelectScreening(string purpose = "")
    {
        List<Screening> screenings = _repository.GetAllScreenings()
            .Where(s => s.StartTime > DateTime.Now)
            .OrderBy(s => s.StartTime)
            .ToList();

        if (!screenings.Any())
        {
            Console.Write("No upcoming screenings available\n");
            return null;
        }
        
        string headerText = string.IsNullOrEmpty(purpose) ? 
            "Available Screenings:" : 
            $"Select a screening {purpose}:";
        
        for (int i = 0; i < screenings.Count; i++)
        {
            Screening screening = screenings[i];
            string movieTitle = _dataService.GetMovieTitle(screening.MovieId);
            Console.Write("{0}. {1}\n", i + 1,  movieTitle);
            Console.Write("{0:dd-MM-yyyy HH:mm} |   {1:F2} DKK |   Hall {2}\n\n",
                screening.StartTime, screening.Price, screening.ScreenHallId);
            Console.Write("\n");
        }

        Console.Write("Select screening (1-{0}) or 0 to cancel:\n", screenings.Count);
        int? choice = _inputHandler.GetMenuChoiceWithCancel(1, screenings.Count);
        
        if (choice == null)
        {
            return null; // User cancelled
        }
        
        return screenings[choice.Value - 1];
    }
    
    /// <summary>
    /// Displays detailed information about a screening.
    /// </summary>
    /// <param name="screening">The screening to display information for</param>
    public void DisplayScreeningInfo(Screening screening)
    {
        string movieTitle = _dataService.GetMovieTitle(screening.MovieId);
        
        Console.Write("\nMovie: {0}\n", movieTitle);
        Console.Write("Date & Time: {0:dd-MM-yyyy HH:mm}\n", screening.StartTime);
        Console.Write("Price: {0:F0} DKK per seat\n", screening.Price);
        Console.Write("Hall: {0}\n", screening.ScreenHallId);
    }
}