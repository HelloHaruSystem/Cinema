using Cinema.UserInterface;

namespace Cinema.Commands;

/// <summary>
/// Command for navigating to sub-menus.
/// Implements the Command pattern to encapsulate menu navigation logic.
/// </summary>
public class SubMenuCommand : BaseCommand
{
    private readonly Menu _subMenu;
    
    /// <summary>
    /// Initializes the sub-menu command with a target menu.
    /// </summary>
    /// <param name="inputHandler">Handler for user input</param>
    /// <param name="subMenu">The sub-menu to navigate to</param>
    /// <param name="name">Display name for this command (default: "sub-menu")</param>
    /// <param name="description">Description for this command (default: "sub-menu")</param>
    public SubMenuCommand(UserInputHandler inputHandler, Menu subMenu, string name = "sub-menu", string description = "sub-menu") : base(inputHandler)
    {
        _subMenu = subMenu;
        Name = name;
        Description = description;
    }
    
    /// <summary>
    /// Gets the command display name.
    /// </summary>
    public override string Name { get; }
    
    /// <summary>
    /// Gets the command description.
    /// </summary>
    public override string Description { get; }
    
    /// <summary>
    /// Executes the sub-menu navigation, running the sub-menu until user exits.
    /// </summary>
    /// <returns>
    /// True to continue current menu after sub-menu exits,
    /// Null to exit entire application if sub-menu requests it
    /// </returns>
    public override bool? Execute()
    {
        bool? continueRunningSubMenu = true;

        while (continueRunningSubMenu == true)
        {
            continueRunningSubMenu = _subMenu.Run();
        
            // if submenu returned null quit the app
            if (continueRunningSubMenu == null)
            {
                return null;
            }
        }
    

        return true;
    }
}