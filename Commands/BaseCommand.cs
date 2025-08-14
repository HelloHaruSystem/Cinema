using Cinema.UserInterface;

namespace Cinema.Commands;

public abstract class BaseCommand : ICommand
{
    protected readonly UserInputHandler InputHandler;

    protected BaseCommand(UserInputHandler inputHandler)
    {
        InputHandler = inputHandler;
    }
    
    public abstract bool? Execute();
    public abstract string Name { get; }
    public abstract string Description { get; }

    protected void PressAnyKey()
    {
        Console.Write("Press any key to continue . . .\n> ");
        Console.ReadKey();
    }
}