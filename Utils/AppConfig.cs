namespace Cinema.Utils;

public static class AppConfig
{
    // UI stuff
    // width of the menu content area (between border)
    public const int MenuWidth = 39;
    
    // full width including border
    public const int MenuFullWidth = MenuWidth + 2;
    
    // characters used to build the menu
    public const char BorderChar = '|';
    public const char FillChar = '=';
    
    // padding for readability
    public const int LeftPadding = 1;
    public const int RightPadding = 2;
    
    // header line format
    public static readonly string HeaderLine = $"{BorderChar}{new string(FillChar, MenuWidth)}{BorderChar}\n";
    
    // center text in menu
    public static string CenterText(string text, int width)
    {
        return text
            .PadLeft((width + text.Length) / 2)
            .PadRight(width);
    }
    
    // db stuff 
    public static readonly string ConnectionString = "Data Source=Data/cinema.db";
    
    // auth stuff
    public static readonly int SaltRounds = 12;
}