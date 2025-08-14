using System.Text;
using Cinema.Commands;
using Cinema.Utils;

namespace Cinema.UserInterface;

/// <summary>
/// Abstract base class for all menu implementations using the Template Method pattern.
/// Provides common menu display and input handling functionality.
/// Derived classes define their specific commands and configuration.
/// </summary>
public abstract class Menu
{
    protected readonly UserInputHandler InputHandler;
    
    /// <summary>
    /// Gets the commands available in this menu (Command pattern).
    /// </summary>
    protected abstract Dictionary<int, ICommand> Commands { get; }
    
    /// <summary>
    /// Gets the maximum number of menu options.
    /// </summary>
    protected abstract int MaxOptions { get;  }
    
    /// <summary>
    /// Gets the header text displayed at the top of the menu.
    /// </summary>
    protected abstract string MenuHeader { get;  }

    /// <summary>
    /// Initializes the menu with an input handler.
    /// </summary>
    /// <param name="inputHandler">Handler for user input operations</param>
    protected Menu(UserInputHandler inputHandler)
    {
        InputHandler = inputHandler;
    }

    /// <summary>
    /// Displays the menu to the console with formatted borders and options.
    /// </summary>
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
            string paddedName = kvp.Value.Name.PadLeft(remainingSpace);
            sb.Append($"{optionText}{paddedName} {AppConfig.BorderChar}\n");
        }
        
        sb.Append(headerLine);
        Console.Write("{0}\n", sb);
    }

    /// <summary>
    /// Handles the user's menu choice by executing the corresponding command.
    /// </summary>
    /// <param name="choice">The menu option chosen by the user</param>
    /// <returns>
    /// True to continue showing this menu,
    /// False to exit this menu,
    /// Null to exit the entire application
    /// </returns>
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

    /// <summary>
    /// Runs the menu loop: clear screen, display menu, get input, handle choice.
    /// Template Method pattern
    /// </summary>
    /// <returns>
    /// True to continue running,
    /// False to exit menu,
    /// Null to exit application
    /// </returns>
    public bool? Run()
    {
        InputHandler.Clear();
        Display();
        int choice = InputHandler.GetMenuChoice(1, MaxOptions);
        return HandleChoice(choice);
    }
}