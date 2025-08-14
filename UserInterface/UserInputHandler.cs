namespace Cinema.UserInterface;

public class UserInputHandler
{
    public UserInputHandler()
    {
        
    }

    public int GetMenuChoice(int min, int max)
    {
        bool validInput = false;
        int choice = 0;

        while (!validInput)
        {
            Console.Write("Enter Choice:\n> ");
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                if (choice >= min && choice <= max)
                {
                    validInput = true;
                }
                else
                {
                    Console.Write("Invalid option please try again\n");
                }
            }
            else
            {
                Console.Write("Invalid input. Please enter a whole number\n");
            }
        }
        
        return choice;
    }

    public void Clear()
    {
        Console.Clear();
    }
}