using System.Text.RegularExpressions;
using Cinema.Entities;

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

    public string[] GetGuestInformation()
    {
        string guestName = "";
        string guestEmail = "";
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        string[] result = new string[2];
        bool validInput = false;

        while (!validInput)
        {
            Console.Write("Please enter your your name\n> ");
            guestName = Console.ReadLine() ?? string.Empty;

            if (guestName.Length == 0)
            {
                Console.Write("Invalid input. Please enter your name number\n");
            }
            else if (guestName.Length > 20)
            {
                Console.Write("Invalid input. input too long. Please try again\n");
            }
            else
            {
                validInput = true;
            }
        }
        validInput = false;

        while (!validInput)
        {
            Console.Write("Please enter your email\n> ");
            guestEmail = Console.ReadLine() ?? string.Empty;

            if (Regex.IsMatch(guestEmail, pattern))
            {
                validInput = true;
            }
            else
            {
                Console.Write("Invalid input. Please enter a valid email address\n");
            }
        }
        result[0] = guestName;
        result[1] = guestEmail;
        
        return result;
    }
    
    public int[] GetSeatSelection(Hall hall)
    {
        int row;
        int seat;
        int[] result = new int[2];
        
        do
        {
            Console.Write("\nEnter row number (1-{0}): ", hall.Rows);
        } while (!int.TryParse(Console.ReadLine(), out row) || row < 1 || row > hall.Rows);

        do
        {
            Console.Write("Enter seat number (1-{0}): ", hall.SeatsPerRow);
        } while (!int.TryParse(Console.ReadLine(), out seat) || seat < 1 || seat > hall.SeatsPerRow);
        result[0] = row;
        result[1] = seat;
        
        return result;
    }
    
    public void Clear()
    {
        Console.Clear();
    }
}