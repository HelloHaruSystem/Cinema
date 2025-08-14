using Cinema.Entities;

namespace Cinema.Repository;

/// <summary>
/// Repository pattern interface for cinema data operations
/// provides data access methods while hiding the underlying data storage
/// </summary>
public interface ICinemaRepository
{
    /// <summary>
    /// Gets all movies available
    /// </summary>
    /// <returns>List of all movies</returns>
    List<Movie> GetAllMovies();
    
    /// <summary>
    /// Gets all screenings scheduled in the cinema
    /// </summary>
    /// <returns>List of all screenings</returns>
    List<Screening> GetAllScreenings();
    
    /// <summary>
    /// Gets all screenings for a specific movie
    /// </summary>
    /// <param name="movieId">The ID of the movie</param>
    /// <returns>List of screenings for the specific movie</returns>
    List<Screening> GetScreeningsByMovie(int movieId);
    
    /// <summary>
    /// Gets the hall information for a specific screening
    /// </summary>
    /// <param name="screeningId">The ID of the screening</param>
    /// <returns>Hall Information or null if not found</returns>
    Hall GetHallForScreening(int screeningId);
    
    /// <summary>
    /// Books a seat for a screening.
    /// </summary>
    /// <param name="screeningId">The screening to book for</param>
    /// <param name="seatId">The seat to book</param>
    /// <param name="guestName">Guest name (if booking as guest)</param>
    /// <param name="guestEmail">Guest email (if booking as guest)</param>
    /// <param name="userId">User ID (if booking as registered user)</param>
    /// <returns>True if booking was successful, false otherwise</returns>
    bool BookSeat(int screeningId, int seatId, string? guestName = null, string? guestEmail = null, int? userId = null);
    
    /// <summary>
    /// Temporarily blocks a seat to prevent double booking during the booking process.
    /// </summary>
    /// <param name="screeningId">The screening ID</param>
    /// <param name="seatId">The seat to block</param>
    /// <param name="minutesToBlock">How long to block the seat (default 5 minutes)</param>
    /// <returns>True if blocking was successful</returns>
    bool BlockSeat(int screeningId, int seatId, int minutesToBlock = 5);
    
    /// <summary>
    /// Gets the unique seat ID for a specific hall position.
    /// </summary>
    /// <param name="hallId">The hall ID</param>
    /// <param name="rowNumber">The row number</param>
    /// <param name="seatNumber">The seat number within the row</param>
    /// <returns>Seat ID or null if not found</returns>
    int? GetSeatId(int hallId, int rowNumber, int seatNumber);
    
    /// <summary>
    /// Removes expired seat blocks from the system.
    /// </summary>
    void CleanupExpiredBlocks();
}