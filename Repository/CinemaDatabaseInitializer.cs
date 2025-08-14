using Microsoft.Data.Sqlite;

namespace Cinema.Repository;

/// <summary>
/// Handles database initialization, table creation, and sample data seeding.
/// Sets up the SQLite database structure for the cinema booking system.
/// </summary>
public class CinemaDatabaseInitializer
{
    private readonly string _connectionString;
    private readonly string _dbPath;

    /// <summary>
    /// Initializes the database initializer and ensures the Data directory exists.
    /// </summary>
    public CinemaDatabaseInitializer()
    {
        // check that the Data directory exists
        string dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        Directory.CreateDirectory(dataDirectory);
        
        _dbPath = Path.Combine(dataDirectory, "cinema.db");
        _connectionString = $"Data Source={_dbPath}";
    }

    /// <summary>
    /// Initializes the database by creating tables and seeding sample data if it's a new database.
    /// </summary>
    public void InitializeDatabase()
    {
        bool isNewDateBase = !File.Exists(_dbPath);
        Console.Write($"Initializing database at: {_dbPath}\n");

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        CreateTables(connection);

        if (isNewDateBase)
        {
            SeedSampleData(connection);   
        }
        
        Console.Write("The database have been created successfully\n");
    }

    /// <summary>
    /// Creates all necessary tables for the cinema booking system.
    /// </summary>
    /// <param name="connection">Open SQLite connection</param>
    private void CreateTables(SqliteConnection connection)
    {
        string[] tables = new[]
        {
            @"CREATE TABLE IF NOT EXISTS users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT UNIQUE NOT NULL,
                password_hash TEXT NOT NULL,
                role TEXT NOT NULL
            )",

            @"CREATE TABLE IF NOT EXISTS movies (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                title TEXT NOT NULL,
                description TEXT,
                duration_minutes INTEGER NOT NULL
            )",

            @"CREATE TABLE IF NOT EXISTS screen_halls (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                rows INTEGER NOT NULL,
                seats_per_row INTEGER NOT NULL
            )",

            @"CREATE TABLE IF NOT EXISTS screenings (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                movie_id INTEGER NOT NULL,
                screen_hall_id INTEGER NOT NULL,
                start_time TIMESTAMP NOT NULL,
                price REAL NOT NULL,
                FOREIGN KEY (movie_id) REFERENCES movies(id),
                FOREIGN KEY (screen_hall_id) REFERENCES screen_halls(id)
            )",

            @"CREATE TABLE IF NOT EXISTS seats (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                screen_hall_id INTEGER NOT NULL,
                row_number INTEGER NOT NULL,
                seat_number INTEGER NOT NULL,
                FOREIGN KEY (screen_hall_id) REFERENCES screen_halls(id)
            )",

            @"CREATE TABLE IF NOT EXISTS bookings (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_id INTEGER,
                guest_name TEXT,
                guest_email TEXT,
                screening_id INTEGER NOT NULL,
                seat_id INTEGER NOT NULL,
                booking_time TIMESTAMP NOT NULL,
                FOREIGN KEY (user_id) REFERENCES users(id),
                FOREIGN KEY (screening_id) REFERENCES screenings(id),
                FOREIGN KEY (seat_id) REFERENCES seats(id),
                UNIQUE(screening_id, seat_id)
            )",

            @"CREATE TABLE IF NOT EXISTS blocked_seats (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                screening_id INTEGER NOT NULL,
                seat_id INTEGER NOT NULL,
                blocked_until TIMESTAMP NOT NULL,
                FOREIGN KEY (screening_id) REFERENCES screenings(id),
                FOREIGN KEY (seat_id) REFERENCES seats(id)
            )"
        };
        
        using SqliteCommand command = connection.CreateCommand();

        foreach (string tableQuery in tables)
        {
            command.CommandText = tableQuery;
            command.ExecuteNonQuery();
        }
        
        Console.Write("All tables created successfully!\n");
    }

    /// <summary>
    /// Seeds the database with sample data for testing and demonstration.
    /// </summary>
    /// <param name="connection">Open SQLite connection</param>
    private void SeedSampleData(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();    
        
        // Add sample movies
        command.CommandText = @"
            INSERT OR IGNORE INTO movies (title, description, duration_minutes) VALUES 
            ('Demon Slayer: Kimetsu no Yaiba Infinity Castle', 'The Infinity Castle is a pocket dimension and the primary setting for the final confrontation between the Demon Slayer Corps and Muzan Kibutsuji', 155),
            ('Ternet Ninja 3', 'It follows the adventures of Aske and his unlikely companion, the vengeful, checkered ninja doll', 88),
            ('Bring Her Back', ' 2025 Australian supernatural psychological horror film about two orphaned siblings, Andy and Piper, who are placed in the care of a new foster mother, Laura, after their father''s death', 104)";
        command.ExecuteNonQuery();

        // Add sample screen halls
        command.CommandText = @"
            INSERT OR IGNORE INTO screen_halls (name, rows, seats_per_row) VALUES 
            ('Hall A', 10, 12),
            ('Hall B', 8, 10)";
        command.ExecuteNonQuery();

        // Generate seats for each hall
        GenerateSeats(connection);

        // Add sample user (password is 'admin123' hashed)
        command.CommandText = @"
            INSERT OR IGNORE INTO users (username, password_hash, role) VALUES 
            ('john_doe', '$2a$11$vFlmHaFdVu3MFWnGqyDj5OH/VG8XaIJ1p7O6MftqkWBBi5NuN1.0u', 'customer')";
        command.ExecuteNonQuery();

        // Add sample screenings
        string tomorrow = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
        string dayAfter = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss");
        
        command.CommandText = $@"
            INSERT OR IGNORE INTO screenings (movie_id, screen_hall_id, start_time, price) VALUES 
            (1, 1, '{tomorrow}', 95.00),
            (2, 2, '{dayAfter}', 125.00),
            (3, 1, '{DateTime.Now.AddDays(1).AddHours(3).ToString("yyyy-MM-dd HH:mm:ss")}', 100.00)";
        command.ExecuteNonQuery();

        Console.WriteLine("Sample data seeded successfully!");
    }

    /// <summary>
    /// Generates individual seat records for each hall based on their row and seat configuration.
    /// </summary>
    /// <param name="connection">Open SQLite connection</param>
    private void GenerateSeats(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        
        // get screen halls
        command.CommandText = "SELECT id, rows, seats_per_row FROM screen_halls";
        using SqliteDataReader reader = command.ExecuteReader();
        
        List<(int id, int rows, int seatsPerRow)> halls = new List<(int id, int rows, int seatsPerRow)>();
        while (reader.Read())
        {
            halls.Add((reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2)));
        }
        reader.Close();
        
        // generate seats for each hall
        foreach ((int id, int rows, int seatsPerRow) hall in halls)
        {
            for (int row = 1; row <= hall.rows; row++)
            {
                for (int seat = 1; seat <= hall.seatsPerRow; seat++)
                {
                    command.CommandText = @"
                        INSERT OR IGNORE INTO seats (screen_hall_id, row_number, seat_number) 
                        VALUES (@hallId, @row, @seat)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@hallId", hall.id);
                    command.Parameters.AddWithValue("@row", row);
                    command.Parameters.AddWithValue("@seat", seat);
                    command.ExecuteNonQuery();
                }
            }
        }
            
        Console.Write($"Generated seats for {halls.Count} screen halls\n");
    }

    /// <summary>
    /// Tests the database by running a sample query and displaying results.
    /// Useful for verifying the database setup and sample data.
    /// </summary>
    public void TestDataBase()
    {
        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();
            
            using SqliteCommand command = connection.CreateCommand();
            
            // test query
            command.CommandText = @"
                SELECT m.title, s.start_time, sh.name as hall_name, s.price
                FROM movies m
                JOIN screenings s ON m.id = s.movie_id
                JOIN screen_halls sh ON s.screen_hall_id = sh.id
                ORDER BY s.start_time";
            
            using SqliteDataReader reader = command.ExecuteReader();
            
            Console.Write("\nCurrent Screenings\n");
            while (reader.Read())
            {
                DateTime.TryParse(reader["start_time"].ToString(), out DateTime startTime);
                Console.Write("{0} - {1} - {2} - {3}.- DKK\n",
                    reader["title"].ToString()?.PadRight(50),
                    startTime.ToString("dd-MM-yyyy HH:mm").PadRight(16),
                    reader["hall_name"].ToString()?.PadRight(8),
                    reader["price"]);
            }
        }
        catch (Exception ex)
        {
            Console.Write("Database test failed: {0}\n",  ex.Message);
        }
    }
}