using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kitty
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Function 1: Create a Kitty (Expense Group)
            var newKitty = AddKitty.CreateKitty();


            // Function 2: Pick The kitty and pick yourself (sort of login into the Kitty)
            // Function 3: Add Transactions to a Kitty
            // Function 4: Leave a Comment in the group
            // Function 5: Display Financial Overview / Expenses/ Comments
            var startScreen = new StartScreen();
           

        }
    }
}
