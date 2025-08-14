using Cinema.Commands;
using Cinema.Commands.Concrete;
using Cinema.Commands.Concrete.User;
using Cinema.Services;

namespace Cinema.UserInterface.Concrete;

public class AuthMenu : Menu
{
    private readonly AuthenticationService _authService;

    public AuthMenu(UserInputHandler inputHandler, AuthenticationService authService)
        : base(inputHandler)
    {
        _authService = authService;
    }

    protected override Dictionary<int, ICommand> Commands => new Dictionary<int, ICommand>()
    {
        { 1, new LoginCommand(InputHandler, _authService) },
        { 2, new RegisterCommand(InputHandler, _authService) },
        { 3, new BackToMainMenuCommand(InputHandler) }
    };

    protected override int MaxOptions => Commands.Count;
    protected override string MenuHeader => "USER LOGIN / REGISTER";
}