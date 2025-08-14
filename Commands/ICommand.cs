
namespace Cinema.Commands;

/// <summary>
/// Dinfes the Command pattern for menu actions
/// Each command represents a specific action
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Executes the command
    /// </summary>
    /// <returns>
    /// True to continue running the current menu
    /// false to exit the current menu
    /// null to exit the entire application
    /// </returns>
    public bool? Execute();
    string Name { get; }
    string Description { get; }
}