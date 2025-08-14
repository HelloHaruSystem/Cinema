using Cinema.UserInterface;

namespace Cinema.Commands.Concrete;

public class ExitCommand : BaseCommand
{

    public ExitCommand(UserInputHandler inputHandler)
        : base(inputHandler)
    {
        
    }
    
    public override string Name => "Exit";
    public override string Description => "Exits the application";

    public override bool? Execute()
    {
        Console.Write("\nThanks you for using Cinema Booking System!\n");
        PressAnyKey();
        return false;
    }
}