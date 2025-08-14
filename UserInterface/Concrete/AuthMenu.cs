using Cinema.Commands;
using Cinema.Commands.Concrete;
using Cinema.Commands.Concrete.User;
using Cinema.Services;

namespace Cinema.UserInterface.Concrete;

/// <summary>
/// Authentication menu for user login and registration.
/// Provides options for existing users to log in or new users to create accounts.
/// </summary>
public class AuthMenu : Menu
{
    private readonly AuthenticationService _authService;

    /// <summary>
    /// Initializes the authentication menu.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="authService">Service for authentication operations</param>
    public AuthMenu(UserInputHandler inputHandler, AuthenticationService authService)
        : base(inputHandler)
    {
        _authService = authService;
    }

    /// <summary>
    /// Gets the available authentication commands.
    /// </summary>
    protected override Dictionary<int, ICommand> Commands => new Dictionary<int, ICommand>()
    {
        { 1, new LoginCommand(InputHandler, _authService) },
        { 2, new RegisterCommand(InputHandler, _authService) },
        { 3, new BackToMainMenuCommand(InputHandler) }
    };

    /// <summary>
    /// Gets the maximum number of menu options.
    /// </summary>
    protected override int MaxOptions => Commands.Count;
    
    /// <summary>
    /// Gets the authentication menu header text.
    /// </summary>
    protected override string MenuHeader => "USER LOGIN / REGISTER";
}