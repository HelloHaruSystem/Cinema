using Cinema.Entities;
using Microsoft.Data.Sqlite;

namespace Cinema.Services;

/// <summary>
/// Service class for cinema data operations using SQLite database.
/// Handles all database interactions for movies, screenings, seats, and bookings.
/// </summary>
public class CinemaDataService
{
    private readonly string _connectionString;
    
    /// <summary>
    /// Initializes the cinema data service.
    /// </summary>
    /// <param name="connectionString">SQLite database connection string</param>
    public CinemaDataService(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Loads all movies from the database.
    /// </summary>
    /// <returns>List of all movies</returns>
    public List<Movie> LoadMovies()
    {
        List<Movie> movies = new();
        
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT id, title, description, duration_minutes FROM movies";
        
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            movies.Add(new Movie
            {
                Id = Convert.ToInt32(reader.GetInt64(0)), 
                Title = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                DurationMinutes = Convert.ToInt32(reader.GetInt64(3)) 
            });
        }
        
        return movies;
    }

    /// <summary>
    /// Loads all screenings from the database.
    /// </summary>
    /// <returns>List of all screenings ordered by start time</returns>
    public List<Screening> LoadScreenings()
    {
        List<Screening> screenings = new();
        
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                s.id as screening_id,
                s.movie_id,
                s.screen_hall_id,
                s.start_time,
                s.price
            FROM screenings s
            ORDER BY s.start_time";

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            screenings.Add(new Screening
            {
                Id = Convert.ToInt32(reader.GetInt64(0)),        
                MovieId = Convert.ToInt32(reader.GetInt64(1)),   
                ScreenHallId = Convert.ToInt32(reader.GetInt64(2)),
                StartTime = DateTime.Parse(reader.GetString(3)),
                Price = reader.GetDouble(4)
            });
        }

        return screenings;
    }
    
    /// <summary>
    /// Loads hall information for a specific screening.
    /// </summary>
    /// <param name="screeningId">The screening ID</param>
    /// <returns>Hall information or null if not found</returns>
    public Hall LoadHallForScreening(int screeningId)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        SqliteCommand hallCommand = connection.CreateCommand();
        hallCommand.CommandText = @"
            SELECT sh.id, sh.name, sh.rows, sh.seats_per_row
            FROM screen_halls sh
            JOIN screenings s ON sh.id = s.screen_hall_id
            WHERE s.id = @screeningId";
        hallCommand.Parameters.AddWithValue("@screeningId", screeningId);

        using SqliteDataReader hallReader = hallCommand.ExecuteReader();
        if (!hallReader.Read())
            return null;

        Hall hall = new Hall
        {
            Id = Convert.ToInt32(hallReader.GetInt64(0)),      
            Name = hallReader.GetString(1),
            Rows = Convert.ToInt32(hallReader.GetInt64(2)),      
            SeatsPerRow = Convert.ToInt32(hallReader.GetInt64(3))
        };

        return hall;
    }
    
    /// <summary>
    /// Loads all seats for a hall with their booking and blocking status for a specific screening.
    /// </summary>
    /// <param name="hallId">The hall ID</param>
    /// <param name="screeningId">The screening ID</param>
    /// <returns>List of seats with their availability status</returns>
    public List<(Seat seat, bool isBooked, bool isBlocked)> LoadSeatsWithStatus(int hallId, int screeningId)
    {
        List<(Seat seat, bool isBooked, bool isBlocked)> seatsWithStatus = new();

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                s.id,
                s.screen_hall_id,
                s.row_number,
                s.seat_number,
                CASE WHEN b.seat_id IS NOT NULL THEN 1 ELSE 0 END as is_booked,
                CASE WHEN bs.seat_id IS NOT NULL AND bs.blocked_until > datetime('now') THEN 1 ELSE 0 END as is_blocked
            FROM seats s
            LEFT JOIN bookings b ON s.id = b.seat_id AND b.screening_id = @screeningId
            LEFT JOIN blocked_seats bs ON s.id = bs.seat_id AND bs.screening_id = @screeningId
            WHERE s.screen_hall_id = @hallId
            ORDER BY s.row_number, s.seat_number";

        command.Parameters.AddWithValue("@hallId", hallId);
        command.Parameters.AddWithValue("@screeningId", screeningId);

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Seat seat = new Seat
            {
                Id = Convert.ToInt32(reader.GetInt64(0)),
                ScreenHallId = Convert.ToInt32(reader.GetInt64(1)),
                RowNumber = Convert.ToInt32(reader.GetInt64(2)),
                SeatNumber = Convert.ToInt32(reader.GetInt64(3)),
            };

            bool isBooked = Convert.ToInt32(reader.GetInt64(4)) == 1;
            bool isBlocked = Convert.ToInt32(reader.GetInt64(5)) == 1;

            seatsWithStatus.Add((seat, isBooked, isBlocked));
        }

        return seatsWithStatus;
    }
    
    /// <summary>
    /// Books a seat for a screening.
    /// </summary>
    /// <param name="screeningId">The screening ID</param>
    /// <param name="seatId">The seat ID to book</param>
    /// <param name="guestName">Guest name (for guest bookings)</param>
    /// <param name="guestEmail">Guest email (for guest bookings)</param>
    /// <param name="userId">User ID (for registered user bookings)</param>
    /// <returns>True if booking was successful, false if seat was already taken</returns>
    public bool BookSeat(int screeningId, int seatId, string guestName = null, string guestEmail = null, int? userId = null)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO bookings (user_id, guest_name, guest_email, screening_id, seat_id, booking_time)
            VALUES (@userId, @guestName, @guestEmail, @screeningId, @seatId, @bookingTime)";

        command.Parameters.AddWithValue("@userId", userId.HasValue ? userId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@guestName", guestName ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@guestEmail", guestEmail ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@screeningId", screeningId);
        command.Parameters.AddWithValue("@seatId", seatId);
        command.Parameters.AddWithValue("@bookingTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        try
        {
            command.ExecuteNonQuery();
            return true;
        }
        catch (SqliteException)
        {
            return false; // Booking failed
        }
    }
    
    /// <summary>
    /// Temporarily blocks a seat to prevent double booking during the selection process.
    /// </summary>
    /// <param name="screeningId">The screening ID</param>
    /// <param name="seatId">The seat ID to block</param>
    /// <param name="minutesToBlock">How many minutes to block the seat (default 5)</param>
    /// <returns>True if blocking was successful</returns>
    public bool BlockSeat(int screeningId, int seatId, int minutesToBlock = 5)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT OR REPLACE INTO blocked_seats (screening_id, seat_id, blocked_until)
            VALUES (@screeningId, @seatId, @blockedUntil)";

        command.Parameters.AddWithValue("@screeningId", screeningId);
        command.Parameters.AddWithValue("@seatId", seatId);
        command.Parameters.AddWithValue("@blockedUntil", DateTime.Now.AddMinutes(minutesToBlock).ToString("yyyy-MM-dd HH:mm:ss"));

        try
        {
            command.ExecuteNonQuery();
            return true;
        }
        catch (SqliteException)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Gets the database ID for a seat at a specific position in a hall.
    /// </summary>
    /// <param name="hallId">The hall ID</param>
    /// <param name="rowNumber">Row number (1-based)</param>
    /// <param name="seatNumber">Seat number within row (1-based)</param>
    /// <returns>Seat ID or null if not found</returns>
    public int? GetSeatId(int hallId, int rowNumber, int seatNumber)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id FROM seats 
            WHERE screen_hall_id = @hallId 
            AND row_number = @rowNumber 
            AND seat_number = @seatNumber";
        
        command.Parameters.AddWithValue("@hallId", hallId);
        command.Parameters.AddWithValue("@rowNumber", rowNumber);
        command.Parameters.AddWithValue("@seatNumber", seatNumber);

        //TODO: refactor this
        var result = command.ExecuteScalar();
        if (result != null && result != DBNull.Value)
        {
            return Convert.ToInt32((long)result);
        }
        return null;
    }
    
    /// <summary>
    /// Removes expired seat blocks from the database.
    /// </summary>
    public void CleanupExpiredBlocks()
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM blocked_seats WHERE blocked_until < datetime('now')";
        command.ExecuteNonQuery();
    }
    
    /// <summary>
    /// Gets a movie by its ID.
    /// </summary>
    /// <param name="movieId">The movie ID</param>
    /// <returns>Movie object or null if not found</returns>
    public Movie? GetMovieById(int movieId)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id, title, description, duration_minutes FROM movies WHERE id = @movieId";
        command.Parameters.AddWithValue("@movieId", movieId);

        using SqliteDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Movie
            {
                Id = Convert.ToInt32(reader.GetInt64(0)),   
                Title = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                DurationMinutes = Convert.ToInt32(reader.GetInt64(3))
            };
        }

        return null;
    }
    
    /// <summary>
    /// Gets all bookings for a specific user with complete details.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>List of booking details including screening, movie, hall, and seat information</returns>
    public List<(Bookings booking, Screening screening, Movie movie, Hall hall, Seat seat)> GetUserBookings(int userId)
    {
        List<(Bookings booking, Screening screening, Movie movie, Hall hall, Seat seat)> bookings = new();
        
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                b.id as booking_id, b.user_id, b.guest_name, b.guest_email, 
                b.screening_id, b.seat_id, b.booking_time,
                s.id as screening_id2, s.movie_id, s.screen_hall_id, s.start_time, s.price,
                m.id as movie_id2, m.title, m.description, m.duration_minutes,
                sh.id as hall_id, sh.name as hall_name, sh.rows, sh.seats_per_row,
                se.id as seat_id2, se.screen_hall_id as seat_hall_id, se.row_number, se.seat_number
            FROM bookings b
            JOIN screenings s ON b.screening_id = s.id
            JOIN movies m ON s.movie_id = m.id
            JOIN screen_halls sh ON s.screen_hall_id = sh.id
            JOIN seats se ON b.seat_id = se.id
            WHERE b.user_id = @userId
            ORDER BY b.booking_time DESC";
        
        command.Parameters.AddWithValue("@userId", userId);
        
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Bookings booking = new Bookings
            {
                Id = Convert.ToInt32((long)reader["booking_id"]),
                UserId = Convert.ToInt32(reader["user_id"]),
                GuestName = reader["guest_name"] == DBNull.Value ? null : reader["guest_name"].ToString(),
                GuestEmail = reader["guest_email"] == DBNull.Value ? null : reader["guest_email"].ToString(),
                ScreeningId = Convert.ToInt32(reader["screening_id"]),
                SeatId = Convert.ToInt32(reader["seat_id"]),
                BookingTime = DateTime.Parse(reader["booking_time"].ToString())
            };

            Screening screening = new Screening
            {
                Id = Convert.ToInt32(reader["screening_id2"]),
                MovieId = Convert.ToInt32(reader["movie_id"]),
                ScreenHallId = Convert.ToInt32(reader["screen_hall_id"]),
                StartTime = DateTime.Parse(reader["start_time"].ToString()),
                Price = Convert.ToDouble(reader["price"])
            };

            Movie movie = new Movie
            {
                Id = Convert.ToInt32(reader["movie_id2"]),
                Title = reader["title"].ToString(),
                Description = reader["description"] == DBNull.Value ? null : reader["description"].ToString(),
                DurationMinutes = Convert.ToInt32(reader["duration_minutes"])
            };

            Hall hall = new Hall
            {
                Id = Convert.ToInt32(reader["hall_id"]),
                Name = reader["hall_name"].ToString(),
                Rows = Convert.ToInt32(reader["rows"]),
                SeatsPerRow = Convert.ToInt32(reader["seats_per_row"])
            };

            Seat seat = new Seat
            {
                Id = Convert.ToInt32(reader["seat_id2"]),
                ScreenHallId = Convert.ToInt32(reader["seat_hall_id"]),
                RowNumber = Convert.ToInt32(reader["row_number"]),
                SeatNumber = Convert.ToInt32(reader["seat_number"])
            };

            bookings.Add((booking, screening, movie, hall, seat));
        }
        
        return bookings;
    }
    
    /// <summary>
    /// Gets a movie title by ID, with fallback if not found.
    /// </summary>
    /// <param name="movieId">The movie ID</param>
    /// <returns>Movie title or fallback text if not found</returns>
    public string GetMovieTitle(int movieId)
    {
        Movie? movie = GetMovieById(movieId);
        return movie?.Title ?? $"Movie {movieId}";
    }
}