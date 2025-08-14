using Cinema.Services;
using Cinema.UserInterface;

namespace Cinema.Commands.Concrete.User;

/// <summary>
/// Command for user logout.
/// </summary>
public class LogoutCommand : BaseCommand
{
    private readonly AuthenticationService _authService;

    /// <summary>
    /// Initializes the logout command.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="authService">Service for authentication</param>
    public LogoutCommand(UserInputHandler inputHandler, AuthenticationService authService)
        : base(inputHandler)
    {
        _authService = authService;
    }

    /// <summary>
    /// Gets the command display name.
    /// </summary>
    public override string Name => "Logout";
    
    /// <summary>
    /// Gets the command description.
    /// </summary>
    public override string Description => "Logout and return to main menu";

    /// <summary>
    /// Executes the logout process.
    /// </summary>
    /// <returns>False to exit user menu</returns>
    public override bool? Execute()
    {
        _authService.Logout();
        Console.Write("You have been logged out successfully.\n");
        PressAnyKey();
        return false; // Exit user menu
    }
}