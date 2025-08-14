using Cinema.UserInterface;

namespace Cinema.Commands.Concrete;

// ExitCommand.cs
/// <summary>
/// Command for exiting the application.
/// </summary>
public class ExitCommand : BaseCommand
{

    /// <summary>
    /// Initializes the exit command.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    public ExitCommand(UserInputHandler inputHandler)
        : base(inputHandler)
    {
        
    }
    
    /// <summary>
    /// Gets the command display name.
    /// </summary>
    public override string Name => "Exit";
    
    /// <summary>
    /// Gets the command description.
    /// </summary>
    public override string Description => "Exits the application";

    /// <summary>
    /// Executes the exit command with farewell message.
    /// </summary>
    /// <returns>False to exit the application</returns>
    public override bool? Execute()
    {
        Console.Write("\nThanks you for using Cinema Booking System!\n");
        PressAnyKey();
        return false;
    }
}