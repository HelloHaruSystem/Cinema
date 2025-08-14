using Cinema.Entities;
using Cinema.Utils;
using Microsoft.Data.Sqlite;

namespace Cinema.Services;

public class AuthenticationService
{
    private readonly string _connectionString;
    private readonly PasswordService _passwordService;
    public Users? CurrentUser { get; private set; }

    public AuthenticationService(string connectionString, PasswordService passwordService)
    {
        _connectionString = connectionString;
        _passwordService = passwordService;
    }

    public AuthResult RegisterUser(string username, string password, string role = "customer")
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
        {
            return new AuthResult { Success = false, Message = "Username must be at least 3 characters long" };
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            return new AuthResult { Success = false, Message = "Password must be at least 6 characters long" };
        }

        if (UserExists(username))
        {
            return new AuthResult { Success = false, Message = "Username already exists" };
        }

        try
        {
            string hashedPassword = _passwordService.HashPassword(password);

            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO users (username, password_hash, role)
                VALUES (@username, @passwordHash, @role)";

            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@passwordHash", hashedPassword);
            command.Parameters.AddWithValue("@role", role);

            command.ExecuteNonQuery();

            return new AuthResult { Success = true, Message = "User registered successfully" };
        }
        catch (Exception ex)
        {
            return new AuthResult { Success = false, Message = $"Registration failed: {ex.Message}" };
        }
    }

    public AuthResult Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return new AuthResult { Success = false, Message = "Username and password are required" };
        }

        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT id, username, password_hash, role 
                FROM users 
                WHERE username = @username";

            command.Parameters.AddWithValue("@username", username);

            using SqliteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string storedHash = reader.GetString(2);
                
                if (_passwordService.VerifyPassword(password, storedHash))
                {
                    CurrentUser = new Users
                    {
                        Id = Convert.ToInt32(reader.GetInt64(0)),
                        Username = reader.GetString(1),
                        PasswordHash = storedHash,
                        Role = reader.GetString(3)
                    };

                    return new AuthResult { Success = true, Message = "Login successful", User = CurrentUser };
                }
            }

            return new AuthResult { Success = false, Message = "Invalid username or password" };
        }
        catch (Exception ex)
        {
            return new AuthResult { Success = false, Message = $"Login failed: {ex.Message}" };
        }
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public bool IsLoggedIn()
    {
        return CurrentUser != null;
    }

    private bool UserExists(string username)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM users WHERE username = @username";
        command.Parameters.AddWithValue("@username", username);

        long count = (long)(command.ExecuteScalar() ?? 0);
        return count > 0;
    }
}

