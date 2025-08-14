namespace Cinema.Services;

public class CinemaDataService
{
    private readonly string _connectionString;
    
    public CinemaDataService(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    
}