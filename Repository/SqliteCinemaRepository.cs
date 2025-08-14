using Cinema.Entities;
using Cinema.Services;

namespace Cinema.Repository;

/// <summary>
/// SQLite implementation of the Repository pattern for cinema data.
/// Acts as a facade over CinemaDataService to provide a clean repository interface.
/// </summary>
public class SqliteCinemaRepository : ICinemaRepository
{
    private readonly CinemaDataService _dataService;
    
    /// <summary>
    /// Initializes the SQLite repository.
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    public SqliteCinemaRepository(string connectionString)
    {
        _dataService = new CinemaDataService(connectionString);
    }
    
    /// <summary>
    /// Gets all movies from the database.
    /// </summary>
    /// <returns>List of all movies</returns>
    public List<Movie> GetAllMovies()
    {
        return _dataService.LoadMovies();
    }
    
    /// <summary>
    /// Gets all screenings from the database.
    /// </summary>
    /// <returns>List of all screenings</returns>
    public List<Screening> GetAllScreenings()
    {
        return _dataService.LoadScreenings();
    }
    
    /// <summary>
    /// Gets screenings filtered by movie ID.
    /// </summary>
    /// <param name="movieId">The movie ID to filter by</param>
    /// <returns>List of screenings for the specified movie</returns>
    public List<Screening> GetScreeningsByMovie(int movieId)
    {
        return _dataService.LoadScreenings()
            .Where(s => s.MovieId == movieId)
            .ToList();
    }
    
    /// <summary>
    /// Gets hall information for a specific screening.
    /// </summary>
    /// <param name="screeningId">The screening ID</param>
    /// <returns>Hall information or null if not found</returns>
    public Hall GetHallForScreening(int screeningId)
    {
        return _dataService.LoadHallForScreening(screeningId);
    }
    
    /// <summary>
    /// Books a seat for a screening.
    /// </summary>
    /// <param name="screeningId">The screening to book for</param>
    /// <param name="seatId">The seat to book</param>
    /// <param name="guestName">Guest name (if booking as guest)</param>
    /// <param name="guestEmail">Guest email (if booking as guest)</param>
    /// <param name="userId">User ID (if booking as registered user)</param>
    /// <returns>True if booking was successful, false otherwise</returns>
    public bool BookSeat(int screeningId, int seatId, string guestName = null, string guestEmail = null, int? userId = null)
    {
        return _dataService.BookSeat(screeningId, seatId, guestName, guestEmail, userId);
    }
    
    /// <summary>
    /// Temporarily blocks a seat to prevent double booking.
    /// </summary>
    /// <param name="screeningId">The screening ID</param>
    /// <param name="seatId">The seat to block</param>
    /// <param name="minutesToBlock">How long to block the seat (default 5 minutes)</param>
    /// <returns>True if blocking was successful</returns>
    public bool BlockSeat(int screeningId, int seatId, int minutesToBlock = 5)
    {
        return _dataService.BlockSeat(screeningId, seatId, minutesToBlock);
    }
    
    /// <summary>
    /// Gets the unique seat ID for a specific hall position.
    /// </summary>
    /// <param name="hallId">The hall ID</param>
    /// <param name="rowNumber">The row number</param>
    /// <param name="seatNumber">The seat number within the row</param>
    /// <returns>Seat ID or null if not found</returns>
    public int? GetSeatId(int hallId, int rowNumber, int seatNumber)
    {
        return _dataService.GetSeatId(hallId, rowNumber, seatNumber);
    }
    
    /// <summary>
    /// Removes expired seat blocks from the system.
    /// </summary>
    public void CleanupExpiredBlocks()
    {
        _dataService.CleanupExpiredBlocks();
    }
}