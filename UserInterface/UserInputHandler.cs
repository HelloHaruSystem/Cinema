using System.Text.RegularExpressions;
using Cinema.Entities;

namespace Cinema.UserInterface;

/// <summary>
/// Handles all user input operations with validation.
/// Provides safe methods for getting different types of input from the console.
/// </summary>
public class UserInputHandler
{
    public UserInputHandler()
    {
        
    }

    /// <summary>
    /// Gets a menu choice from the user within a specified range.
    /// </summary>
    /// <param name="min">Minimum valid choice</param>
    /// <param name="max">Maximum valid choice</param>
    /// <returns>Valid menu choice within the specified range</returns>
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
    
    /// <summary>
    /// Gets a menu choice with option to cancel (return 0).
    /// </summary>
    /// <param name="min">Minimum valid choice</param>
    /// <param name="max">Maximum valid choice</param>
    /// <returns>Valid choice or null if user cancelled</returns>
    public int? GetMenuChoiceWithCancel(int min, int max)
    {
        bool validInput = false;
        int choice = 0;

        while (!validInput)
        {
            Console.Write("Enter Choice:\n> ");
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                if (choice == 0)
                {
                    return null; // User cancelled
                }
                else if (choice >= min && choice <= max)
                {
                    validInput = true;
                }
                else
                {
                    Console.Write("Invalid option. Please enter 0-{0}\n", max);
                }
            }
            else
            {
                Console.Write("Invalid input. Please enter a whole number\n");
            }
        }
        
        return choice;
    }

    /// <summary>
    /// Gets guest information for booking (name and email).
    /// </summary>
    /// <returns>Array with [0] = name, [1] = email</returns>
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
    
    /// <summary>
    /// Gets seat selection from user for a specific hall.
    /// </summary>
    /// <param name="hall">The hall to select seats from</param>
    /// <returns>Array with [0] = row number, [1] = seat number</returns>
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
    
    /// <summary>
    /// Gets a non-empty string from the user.
    /// </summary>
    /// <returns>Non-empty string input</returns>
    public string GetString()
    {
        bool validInput = false;
        string result = "";

        while (!validInput)
        {
            result = Console.ReadLine() ?? string.Empty;

            if (result.Length == 0)
            {
                Console.Write("Invalid input. you need to input something\n> ");
            }
            else
            {
                validInput = true;
            }
        }
        
        return result;
    }

    /// <summary>
    /// Gets a password with hidden input (shows * characters).
    /// </summary>
    /// <returns>Password string</returns>
    public string GetPassword()
    {
        bool validInput = false;
        string result = "";

        while (!validInput)
        {
            result = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    result += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && result.Length > 0)
                {
                    result = result.Substring(0, result.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            Console.Write("\n");

            if (result.Length == 0)
            {
                Console.Write("Invalid input. you need to input something\n> ");
            }
            else
            {
                validInput = true;
            }
        }
    
        return result;
    }
    
    /// <summary>
    /// Clears the console screen.
    /// </summary>
    public void Clear()
    {
        Console.Clear();
    }
}