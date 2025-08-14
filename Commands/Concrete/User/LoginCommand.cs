using Cinema.Services;
using Cinema.UserInterface;
using Cinema.Utils;

namespace Cinema.Commands.Concrete.User;

public class LoginCommand : BaseCommand
{
    private readonly AuthenticationService _authService;

    public LoginCommand(UserInputHandler inputHandler, AuthenticationService authService)
        : base(inputHandler)
    {
        _authService = authService;
    }

    public override string Name => "Login";
    public override string Description => "Login to your account";

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