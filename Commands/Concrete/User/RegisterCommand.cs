using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete.User;

/// <summary>
/// Command for user account registration.
/// </summary>
public class RegisterCommand : BaseCommand
{
    private readonly AuthenticationService _authService;

    /// <summary>
    /// Initializes the register command.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="authService">Service for authentication</param>
    public RegisterCommand(UserInputHandler inputHandler, AuthenticationService authService)
        : base(inputHandler)
    {
        _authService = authService;
    }
    
    /// <summary>
    /// Gets the command display name.
    /// </summary>
    public override string Name => "Register New Account";
    
    /// <summary>
    /// Gets the command description.
    /// </summary>
    public override string Description => "Create a new user account";
    
    /// <summary>
    /// Executes the registration process with validation.
    /// </summary>
    /// <returns>True to stay in auth menu</returns>
    public override bool? Execute()
    {
        InputHandler.Clear();
        Console.Write("CREATE NEW ACCOUNT\n\n");

        Console.Write("Enter username (min 3 characters): ");
        string username = InputHandler.GetString();

        if (username.Length < 3)
        {
            Console.Write("Username must be at least 3 characters long\n");
            PressAnyKey();
            return true;
        }

        Console.Write("Enter password (min 6 characters): ");
        string password = InputHandler.GetPassword();

        Console.Write("\nConfirm password: ");
        string confirmPassword = InputHandler.GetPassword();

        if (password != confirmPassword)
        {
            Console.Write("\n\nPasswords do not match!\n");
            PressAnyKey();
            return true;
        }

        if (password.Length < 6)
        {
            Console.Write("\n\nPassword must be at least 6 characters long\n");
            PressAnyKey();
            return true;
        }

        Console.Write("\n\nCreating account...\n");

        AuthResult result = _authService.RegisterUser(username, password);

        if (result.Success)
        {
            Console.Write("{0}\n", result.Message);
            Console.Write("You can now login with your new account!\n");
        }
        else
        {
            Console.Write("{0}\n", result.Message);
        }

        PressAnyKey();
        return true;
    }
}