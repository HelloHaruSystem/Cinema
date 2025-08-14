using Cinema.Repository;

namespace Cinema;

class Program
{
    static void Main(string[] args)
    {
        CinemaDatabaseInitializer dbInitializer = new();
        dbInitializer.InitializeDatabase();
        dbInitializer.TestDataBase();
        
        
    }
}