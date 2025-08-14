using Cinema.UserInterface;

namespace Cinema.Commands;

/// <summary>
/// Abstract base class for all commands implementing the Command pattern.
/// Provides common functionality and structure for menu commands.
/// </summary>
public abstract class BaseCommand : ICommand
{
    protected readonly UserInputHandler InputHandler;

    /// <summary>
    /// Initializes the command with an input handler.
    /// </summary>
    /// <param name="inputHandler">Handler for user input operations</param>
    protected BaseCommand(UserInputHandler inputHandler)
    {
        InputHandler = inputHandler;
    }
    
    /// <summary>
    /// Executes the specific command action
    /// </summary>
    /// <returns>
    /// True to continue current menu,
    /// False to exit current menu,
    /// Null to exit application
    /// </returns>
    public abstract bool? Execute();
    
    /// <summary>
    /// Gets the display name for this command.
    /// </summary>
    public abstract string Name { get; }
    
    /// <summary>
    /// Gets the description of what this command does.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// helper method to pause execution until user presses a key.
    /// </summary>
    protected void PressAnyKey()
    {
        Console.Write("Press any key to continue . . .\n> ");
        Console.ReadKey();
    }
}