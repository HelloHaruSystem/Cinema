using Cinema.UserInterface;

namespace Cinema.Commands.Concrete;

public class BackToMainMenuCommand : BaseCommand
{
    public BackToMainMenuCommand(UserInputHandler inputHandler)
        : base(inputHandler)
    {
        
    }

    public override string Name => "Back to Main Menu";
    public override string Description => "Return to main menu";

    public override bool? Execute()
    {
        return false; // Exit current menu
    }
}