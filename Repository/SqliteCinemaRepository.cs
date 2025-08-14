using Cinema.Entities;
using Cinema.Services;

namespace Cinema.Repository;

public class SqliteCinemaRepository : ICinemaRepository
{
    private readonly CinemaDataService _dataService;
    
    public SqliteCinemaRepository(string connectionString)
    {
        _dataService = new CinemaDataService(connectionString);
    }
    
    public List<Movie> GetAllMovies()
    {
        return _dataService.LoadMovies();
    }
    
    public List<Screening> GetAllScreenings()
    {
        return _dataService.LoadScreenings();
    }
    
    public List<Screening> GetScreeningsByMovie(int movieId)
    {
        return _dataService.LoadScreenings()
            .Where(s => s.MovieId == movieId)
            .ToList();
    }
    
    public Hall GetHallForScreening(int screeningId)
    {
        return _dataService.LoadHallForScreening(screeningId);
    }
    
    public bool BookSeat(int screeningId, int seatId, string guestName = null, string guestEmail = null, int? userId = null)
    {
        return _dataService.BookSeat(screeningId, seatId, guestName, guestEmail, userId);
    }
    
    public bool BlockSeat(int screeningId, int seatId, int minutesToBlock = 5)
    {
        throw new NotImplementedException();
    }
    
    public int? GetSeatId(int hallId, int rowNumber, int seatNumber)
    {
        return _dataService.GetSeatId(hallId, rowNumber, seatNumber);
    }
    
    public void CleanupExpiredBlocks()
    {
        throw new NotImplementedException();
    }
}