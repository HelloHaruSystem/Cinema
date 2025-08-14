using System.Text;
using Cinema.Commands;
using Cinema.Utils;

namespace Cinema.UserInterface;

public abstract class Menu
{
    private readonly UserInputHandler _inputHandler;
    protected abstract int MaxOptions { get;  }
    protected abstract Dictionary<int, ICommand> Commands { get; }
    protected abstract string MenuHeader { get;  }

    protected Menu(UserInputHandler inputHandler)
    {
        _inputHandler = inputHandler;
    }

    protected virtual void Display()
    {
        StringBuilder sb = new StringBuilder();

        string headerLine = AppConfig.HeaderLine;
        string titleLine = $"{AppConfig.BorderChar}" +
                           $"{AppConfig.CenterText(MenuHeader, AppConfig.MenuWidth)}" +
                           $"{AppConfig.BorderChar}\n";
        
        sb.Append(headerLine);
        sb.Append(titleLine);
        sb.Append(headerLine);

        foreach (KeyValuePair<int, ICommand> kvp in Commands.OrderBy(x => x.Key))
        {
            string optionText = $"{AppConfig.BorderChar} {kvp.Key}: ";
            int remainingSpace = AppConfig.MenuFullWidth - optionText.Length - AppConfig.RightPadding;
            string paddedName = kvp.Value.Name.PadLeft(remainingSpace - 1);
            sb.Append($"{optionText}{paddedName} {AppConfig.BorderChar}\n");
        }
        
        sb.Append(headerLine);
        Console.Write("{0}\n", sb);
    }

    protected virtual bool? HandleChoice(int choice)
    {
        if (this.Commands.ContainsKey(choice))
        {
            return Commands[choice].Execute();
        }
        else
        {
            Console.Write("Invalid Choice!\n");
            return true;
        }
    }

    protected bool? Run()
    {
        _inputHandler.Clear();
        Display();
        int choice = _inputHandler.GetMenuChoice(1, MaxOptions);
        return HandleChoice(choice);
    }
}