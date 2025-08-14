using Cinema.UserInterface;

namespace Cinema.Commands.Concrete;

/// <summary>
/// Command for returning to the main menu from sub-menus.
/// </summary>
public class BackToMainMenuCommand : BaseCommand
{
    
    /// <summary>
    /// Initializes the back to main menu command.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    public BackToMainMenuCommand(UserInputHandler inputHandler)
        : base(inputHandler)
    {
        
    }
    
    /// <summary>
    /// Gets the command display name.
    /// </summary>
    public override string Name => "Back to Main Menu";
    
    /// <summary>
    /// Gets the command description.
    /// </summary>
    public override string Description => "Return to main menu";
    
    /// <summary>
    /// Executes the command to exit the current menu.
    /// </summary>
    /// <returns>False to exit current menu</returns>
    public override bool? Execute()
    {
        return false; // Exit current menu
    }
}