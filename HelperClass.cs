using System;
using System.Collections.Generic;

namespace Kitty
{

    // Data entry handling. Dropdowns and field filling


    //Choosing from the list

    public interface IDropdown<T>
    {
        T ChooseSingleItem(List<T> items, Func<T, int> idFunction, Func<T, string> displayFunction);
        List<T> ChooseMultipleItems(List<T> items, Func<T, int> idFunction, Func<T, string> displayFunction);
    }

    public class AppDropdown<T> : IDropdown<T>
    {
        public T ChooseSingleItem(List<T> items, Func<T, int> idFunction, Func<T, string> displayFunction)
        {

            foreach (var item in items)
            {
                Console.WriteLine($"{idFunction(item)}. {displayFunction(item)}");
            }

            while (true)
            {
                Console.Write("Enter item ID: ");
                if (int.TryParse(Console.ReadLine(), out int selectedItemId) &&
                    items.Any(item => idFunction(item) == selectedItemId))
                {
                    return items.First(item => idFunction(item) == selectedItemId);
                }
                Console.WriteLine("Invalid item ID. Please try again.");
            }
        }

        public List<T> ChooseMultipleItems(List<T> items, Func<T, int> idFunction, Func<T, string> displayFunction)
        {

            foreach (var item in items)
            {
                Console.WriteLine($"{idFunction(item)}. {displayFunction(item)}");
            }

            List<T> selectedItems = new List<T>();

            while (true)
            {
                Console.Write("Enter item ID (0 to finish): ");
                if (int.TryParse(Console.ReadLine(), out int selectedItemId) && selectedItemId != 0)
                {
                    if (items.Any(item => idFunction(item) == selectedItemId))
                    {
                        selectedItems.Add(items.First(item => idFunction(item) == selectedItemId));
                    }
                    else
                    {
                        Console.WriteLine("Invalid item ID. Please try again.");
                    }
                }
                else
                {
                    break;
                }
            }

            return selectedItems;
        }
    }

    // Filling fields

    public static class UserInput
    {
        public static T GetInput<T>(string prompt, Func<string, T> parser)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();
                try
                {
                    return parser(input);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Invalid input. Please enter a valid {typeof(T).Name}.");
                }
            }
        }
    }
}
