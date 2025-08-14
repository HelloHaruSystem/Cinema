using Cinema.Entities;
using Cinema.Repository;
using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete;

/// <summary>
/// Command for displaying all movies and their upcoming screenings.
/// </summary>
public class ViewMoviesCommand : BaseCommand
{
    private readonly ICinemaRepository _repository;
    private readonly CinemaDataService _dataService;

    /// <summary>
    /// Initializes the view movies command.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="repository">Repository for data access</param>
    /// <param name="dataService">Service for cinema data operations</param>
    public ViewMoviesCommand(UserInputHandler inputHandler, ICinemaRepository repository, CinemaDataService dataService) 
        : base(inputHandler)
    {
        _repository = repository;
        _dataService = dataService;
    }

    /// <summary>
    /// Gets the command display name.
    /// </summary>
    public override string Name => "View currently running movies";
    
    /// <summary>
    /// Gets the command description.
    /// </summary>
    public override string Description => "Views all movies in the current cinema.";

    /// <summary>
    /// Executes the command to display movies and screenings.
    /// </summary>
    /// <returns>True to continue current menu</returns>
    public override bool? Execute()
    {
        Console.Clear();
        DisplayMoviesAndScreenings();
        
        Console.Write("\nPress any key to return to the main menu . . .\n");
        Console.ReadKey();
        return true;
    }

    /// <summary>
    /// Displays formatted list of movies with their screening information.
    /// </summary>
    private void DisplayMoviesAndScreenings()
    {
        string header = AppConfig.CenterText("MOVIES & SCREENINGS", AppConfig.MenuWidth);
        Console.Write("{0}{1}{0}\n", AppConfig.BorderChar, header);
        Console.Write("{0}", AppConfig.HeaderLine);

        List<Movie> movies = _repository.GetAllMovies();
        List<Screening> screenings = _repository.GetAllScreenings();

        foreach (Movie movie in movies)
        {
            Console.Write("\n{0}\n", movie.Title);
            Console.Write("Duration: {0} minutes ({1}h {2}m)\n",
                movie.DurationMinutes, movie.DurationMinutes / 60, movie.DurationMinutes % 60);
            Console.Write("Description: {0}\n",
                movie.Description ?? "No description available");

            List<Screening> movieScreenings = screenings.Where(s => s.MovieId == movie.Id && s.StartTime > DateTime.Now).ToList();

            if (movieScreenings.Any())
            {
                Console.Write("Upcoming Screenings:\n");
                foreach (Screening screening in movieScreenings.OrderBy(s => s.StartTime))
                {
                    Console.Write("{0:dd-MM-yyyy HH:mm} - {1:F2} DKK - Hall {2}\n\n",
                        screening.StartTime, screening.Price, screening.ScreenHallId);
                }
            }
            else
            {
                Console.Write("No upcoming screenings\n\n");
            }
        }
    }
}