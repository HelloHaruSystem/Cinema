using Cinema.Entities;

namespace Cinema.Repository;

public interface ICinemaRepository
{
    List<Movie> GetAllMovies();
    List<Screening> GetAllScreenings();
    List<Screening> GetScreeningsByMovie(int movieId);
    Hall GetHallForScreening(int screeningId);
    bool BookSeat(int screeningId, int seatId, string? guestName = null, string? guestEmail = null, int? userId = null);
    bool BlockSeat(int screeningId, int seatId, int minutesToBlock = 5);
    int? GetSeatId(int hallId, int rowNumber, int seatNumber);
    void CleanupExpiredBlocks();
}