using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete.User;

/// <summary>
/// Command for user login authentication.
/// </summary>
public class LoginCommand : BaseCommand
{
    private readonly AuthenticationService _authService;

    /// <summary>
    /// Initializes the login command.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="authService">Service for authentication</param>
    public LoginCommand(UserInputHandler inputHandler, AuthenticationService authService)
        : base(inputHandler)
    {
        _authService = authService;
    }

    /// <summary>
    /// Gets the command display name.
    /// </summary>
    public override string Name => "Login";
    
    /// <summary>
    /// Gets the command description.
    /// </summary>
    public override string Description => "Login to your account";

    
    /// <summary>
    /// Executes the login process with username and password input.
    /// </summary>
    /// <returns>False to exit auth menu on success, true to stay on failure</returns>
    public override bool? Execute()
    {
        InputHandler.Clear();
        Console.Write("USER LOGIN\n\n");

        Console.Write("Enter username: ");
        string username = InputHandler.GetString();
        Console.Write("Enter password: ");
        string password = InputHandler.GetPassword();

        Console.Write("\n\nLogging in...\n");

        AuthResult result = _authService.Login(username, password);

        if (result.Success)
        {
            Console.Write("{0}\n", result.Message);
            Console.Write("Welcome, {0}!\n", result.User?.Username);
            PressAnyKey();
            return false; // Exit to show user menu
        }
        else
        {
            Console.Write("{0}\n", result.Message);
            PressAnyKey();
            return true; // Stay in login menu
        }
    }
}