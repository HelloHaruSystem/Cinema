using Cinema.UserInterface;

namespace Cinema.Commands;

public class SubMenuCommand : BaseCommand
{
    private readonly Menu _subMenu;
    
    public SubMenuCommand(UserInputHandler inputHandler, Menu subMenu, string name = "sub-menu", string description = "sub-menu") : base(inputHandler)
    {
        _subMenu = subMenu;
        Name = name;
        Description = description;
    }
    
    public override string Name { get; }
    public override string Description { get; }
    
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