using Cinema.Services;
using Cinema.UserInterface;

namespace Cinema.Commands.Concrete.User;

public class LogoutCommand : BaseCommand
{
    private readonly AuthenticationService _authService;

    public LogoutCommand(UserInputHandler inputHandler, AuthenticationService authService)
        : base(inputHandler)
    {
        _authService = authService;
    }

    public override string Name => "Logout";
    public override string Description => "Logout and return to main menu";

    public override bool? Execute()
    {
        _authService.Logout();
        Console.Write("You have been logged out successfully.\n");
        PressAnyKey();
        return false; // Exit user menu
    }
}