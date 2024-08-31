using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kitty
{
    public class AddTransaction
    {
        public static void CreateTransaction(int kittyId)
        {
            using (var dbContext = new KittySplitDbContext())

            {

                var kitty = dbContext.Kitties
                    .Include(k => k.Currency) // Include Currency entity
                    .FirstOrDefault(k => k.KittyID == kittyId);

                Console.WriteLine($"Debug: kittyId = {kittyId}");

                string chosenCurrency = kitty.Currency.CurrencyCode;
                string label = GetTransactionLabel();
                int userId = GetUserId(kittyId);
                string description = UserInput.GetInput("What for?", s => s);
                KittyUpgrade(kittyId, GetKittyUsers(kittyId), userId);
                float amount = UserInput.GetInput("How much?: ", float.Parse);
                DateTime date = UserInput.GetInput("When?: ", DateTime.Parse);
                TransactionSplit(label, userId, description,chosenCurrency, kittyId, amount, date);
                Console.WriteLine("Expense has been created successfully!");



            }
        }

        private static string GetTransactionLabel()
        {
            Console.WriteLine("What do you want to add?");
            Console.WriteLine("1. Expense");
            Console.WriteLine("2. Money Given");
            Console.WriteLine("3. Income");

            int labelId = int.Parse(Console.ReadLine());
            string label = "None";
            if (labelId == 1)
            {
                label = "Expense";
            }
            if (labelId == 2)
            {
                label = "Money Given";
            }
            else if (labelId == 3)
            {
                label = "Income";
            }

            return label;
        }

        private static int GetUserId(int kittyId)
        {
            Console.WriteLine("Who wants to add something?");
            using (var dbContext = new KittySplitDbContext())
            {

                var kittyUsers = GetKittyUsers(kittyId);
                //var users = dbContext.Users.ToList();
                IDropdown<User> userSelector = new AppDropdown<User>();
                User selectedUser = userSelector.ChooseSingleItem(kittyUsers, u => u.UserID, u => $"{u.UserName}");

                return selectedUser.UserID;
            }
        }
        private static void KittyUpgrade(int kittyId, List<User> kittyUsers, int userId)
        {
            using (var dbContext = new KittySplitDbContext())
            {
                var kitty = dbContext.Kitties
                    .Include(k => k.Currency) 
                    .FirstOrDefault(k => k.KittyID == kittyId);

                Console.WriteLine($"Current currency code: {kitty.Currency.CurrencyCode}");
                Console.WriteLine("To use multiple currencies, upgrade this kitty.\nDo you want to upgrade for 3€? [Y/N]");
                string decision = Console.ReadLine();

                if (decision.ToUpper() == "Y")
                {
                    kitty.SuperKitty = true;
                    dbContext.SaveChanges();

                    string label = "Expense";
                    string description = "SuperKitty upgrade";
                    string chosenCurrency = kitty.Currency.CurrencyCode;
                    // Amount for SuperKitty upgrade
                    float amount = 3.0f;
                    DateTime date = DateTime.Now;

                    // Split equally among kitty members
                    SplitEqually(kittyUsers, kittyId, label, description, chosenCurrency, amount, date, userId);

                    Console.WriteLine("Kitty upgraded successfully!");
                }
                else
                {
                    Console.WriteLine("Kitty upgrade declined.");
                }

            }
        }

        private static void TransactionSplit(string label, int userId, string description, string chosenCurrency, int kittyId, float amount, DateTime date)
        {
            using (var dbContext = new KittySplitDbContext())

            {
                var kittyUsers = GetKittyUsers(kittyId);
                if (label == "Expense" || label == "Income")

                {

                    Console.WriteLine("1. Split equally between everyone");
                    Console.WriteLine("2. Split differently");

                    int chosenOption = UserInput.GetInput("Choose an option: ", int.Parse);


                    if (chosenOption == 1)
                    {
                        SplitEqually(kittyUsers, kittyId, label, description, chosenCurrency, amount, date, userId);
                    }
                    else if (chosenOption == 2)
                    {
                        SplitDifferently(kittyUsers, kittyId, label, description, chosenCurrency, amount, date, userId);
                    }
                }

                else if (label == "Money Given")
                {
                    Console.WriteLine("Select a person to give money to:");
                    var potentialRecipients = kittyUsers.Where(u => u.UserID != userId).ToList();

                    IDropdown<User> userSelector = new AppDropdown<User>();
                    var selectedUser = userSelector.ChooseSingleItem(kittyUsers, u => u.UserID, u => $"{u.UserID}. {u.UserName}");
                    CreateTransaction(label, userId, description, chosenCurrency, amount, selectedUser.UserName, amount, date, kittyId);
                    dbContext.SaveChanges();


                }
            }

        }

        private static void SplitEqually(List<User> kittyUsers, int kittyId, string label, string description, string chosenCurrency, float amount, DateTime date, int userId)
        {
            using (var dbContext = new KittySplitDbContext())
            {

                int count = kittyUsers.Count;
                float amountSplit = amount / count;

                foreach (var user in kittyUsers)
                {
                    CreateTransaction(label, userId, description, chosenCurrency, amount, user.UserName, amountSplit, date, kittyId);
                }

                dbContext.SaveChanges();
            }
        }

        private static void SplitDifferently(List<User> kittyUsers, int kittyId, string label, string description, string chosenCurrency, float amount, DateTime date, int userId)
        {
            using (var dbContext = new KittySplitDbContext())

            {
                Console.WriteLine("1. Equally");
                Console.WriteLine("2. Different Amounts");
                Console.WriteLine("3. Weight in %");

                int chosenOption2 = UserInput.GetInput("Choose an option: ", int.Parse);

                switch (chosenOption2)
                {
                    case 1:
                        Console.WriteLine("Select participants:");
                        IDropdown<User> userSelector = new AppDropdown<User>();
                        var selectedUsers = userSelector.ChooseMultipleItems(kittyUsers, u => u.UserID, u => $"{u.UserID}. {u.UserName}");
                        SplitEqually(selectedUsers, kittyId, label, description, chosenCurrency, amount, date, userId);
                        break;
                    case 2:
                        SplitDifferentlyAmounts(kittyUsers, kittyId, label, description, chosenCurrency, amount, date, userId);
                        break;
                    case 3:
                        SplitDifferentlyAmounts(kittyUsers, kittyId, label, description, chosenCurrency, amount, date, userId);
                        break;
                }

            }
            
        }

        private static void SplitDifferentlyAmounts(List<User> kittyUsers, int kittyId, string label, string description, string chosenCurrency, float amount, DateTime date, int userId)
        {
            using (var dbContext = new KittySplitDbContext())


            {
                Console.WriteLine("Select participants:");
                IDropdown<User> userSelector = new AppDropdown<User>();
                var selectedUsers = userSelector.ChooseMultipleItems(kittyUsers, u => u.UserID, u => $"{u.UserID}. {u.UserName}");

                foreach (var user in selectedUsers)
                {
                    Console.WriteLine($"Enter custom amount for {user.UserName}: ");
                    float customAmount = float.Parse(Console.ReadLine());
                    CreateTransaction(label, userId, description, chosenCurrency, amount, user.UserName, customAmount, date, kittyId);
                }

                dbContext.SaveChanges();

            }

          
        }

        private static void CreateTransaction(string label, int userId, string description, string chosenCurrency, float amount, string owedBy, float amountSplit, DateTime date, int kittyId)
        {
            using (var dbContext = new KittySplitDbContext())

            {
                // Determine the sign based on the label
                float signedAmount = (label == "Expense" || label == "Money Given") ? amountSplit * (-1) : amountSplit;

                var newTransaction = new Transaction
                {
                    Label = label,
                    UserID = userId,
                    Description = description,
                    ChosenCurrency = chosenCurrency,
                    Amount = amount,
                    OwedBy = owedBy,
                    AmountSplit = signedAmount,
                    Date = date,
                    KittyID = kittyId
                };

                dbContext.Transactions.Add(newTransaction);
                dbContext.SaveChanges();

            }
        }

        private static List<User> GetKittyUsers(int kittyId)
        {
            using (var dbContext = new KittySplitDbContext())
            {
                return dbContext.KittyMembers
                     .Where(km => km.KittyID == kittyId)
                     .Select(km => km.User)
                     .ToList();
            }

        }

    }
}








         