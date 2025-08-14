namespace Cinema.Utils;

/// <summary>
/// Application configuration constants for UI layout, database, and security settings.
/// </summary>
public static class AppConfig
{
    // UI stuff
    /// <summary>
    /// Width of the menu content area (between borders).
    /// </summary>
    public const int MenuWidth = 39;
    
    /// <summary>
    /// Full width including left and right borders.
    /// </summary>
    public const int MenuFullWidth = MenuWidth + 2;
    
    /// <summary>
    /// Character used for menu borders.
    /// </summary>
    public const char BorderChar = '|';
    
    /// <summary>
    /// Character used for menu header lines.
    /// </summary>
    private const char FillChar = '=';
    
    /// // <summary>
    /// Left padding for menu content.
    /// </summary>
    public const int LeftPadding = 1;
    
    /// <summary>
    /// Right padding for menu content.
    /// </summary>
    public const int RightPadding = 2;
    
    /// <summary>
    /// Pre-formatted header line for menus.
    /// </summary>
    public static readonly string HeaderLine = $"{BorderChar}{new string(FillChar, MenuWidth)}{BorderChar}\n";
    
    /// <summary>
    /// Centers text within a specified width.
    /// </summary>
    /// <param name="text">Text to center</param>
    /// <param name="width">Total width to center within</param>
    /// <returns>Centered text with padding</returns>
    public static string CenterText(string text, int width)
    {
        return text
            .PadLeft((width + text.Length) / 2)
            .PadRight(width);
    }
    
    // Database configuration
    /// <summary>
    /// SQLite database connection string.
    /// </summary>
    public static readonly string ConnectionString = "Data Source=Data/cinema.db";
    
    // Authentication configuration
    /// <summary>
    /// Number of salt rounds for BCrypt password hashing (higher = more secure but slower).
    /// </summary>
    public static readonly int SaltRounds = 12;
}