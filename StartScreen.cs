using System;
using Microsoft.EntityFrameworkCore;

namespace Kitty
{
    public class StartScreen
    {

        public int SelectedKittyId { get; private set; }
        public int SelectedUserId { get; private set; }


        public StartScreen()
        {
            using (var dbContext = new KittySplitDbContext())
            {
                Console.WriteLine("Available kitties");
                var kittyDropdown = new AppDropdown<Kitty>();

                // Select a Kitty
                var selectedKitty = kittyDropdown.ChooseSingleItem(
                    dbContext.Kitties.ToList(),
                    k => k.KittyID,
                    k => $"{k.EventName}"
                );

                // Store selected Kitty ID
                SelectedKittyId = selectedKitty.KittyID;

                // Filter users based on the selected Kitty
                var usersForSelectedKitty = dbContext.KittyMembers
                    .Where(km => km.KittyID == SelectedKittyId)
                    .Select(km => km.User)
                    .ToList();

                  var kittyCreator = dbContext.Users.FirstOrDefault(u => !string.IsNullOrEmpty(u.UserEmail));


                Console.WriteLine($"Your friend {kittyCreator.UserName} created a kitty for the event {selectedKitty.EventName}. \n Tell us who you are and you'll see all the expenses entered so far.");

                Console.WriteLine("Who are you?");
                var userDropdown = new AppDropdown<User>();

                // Select a User from the filtered list
                var selectedUser = userDropdown.ChooseSingleItem(
                    usersForSelectedKitty,
                    u => u.UserID,
                    u => $"{u.UserName}"
                );

                // Store selected User ID
                SelectedUserId = selectedUser.UserID;
                selectedUser.Seen = true;
                DisplayFinancialOverview();
                AskForAction();
                dbContext.SaveChanges();
            }
        }



        private void CreateComment()
        {
            var addComment = new AddComment();
            addComment.CreateComment(SelectedUserId, SelectedKittyId);
        }

        private void DisplayFinancialOverview()
        {
            using (var dbContext = new KittySplitDbContext())
            {
                var expenses = dbContext.Transactions
                    .Where(t => t.KittyID == SelectedKittyId)
                    .ToList();

                if (expenses.Any())
                {
                    // Assuming all transactions have the same currency, take the currency from the first transaction
                    string chosenCurrencyCode = expenses.First().ChosenCurrency;

                    float totalGroupExpense = expenses.Sum(t => t.AmountSplit);

                    Console.WriteLine("Financial Overview:");
                    Console.WriteLine($"This event cost the group: {chosenCurrencyCode} {totalGroupExpense:F2}");
                }
                else
                {
                    Console.WriteLine("No transactions found for the selected kitty.");
                }
            }
        }

        private void AskForAction()
        {

            Console.WriteLine("What do you want to do?");
            Console.WriteLine("1. Add Expense");
            Console.WriteLine("2. Leave a Comment");
            Console.WriteLine("3. View All Expenses");
            Console.WriteLine("4. View Comments");

            int choice = UserInput.GetInput("Enter your choice:", int.Parse);

            switch (choice)
            {
                case 1:
                    // Call a method to add an expense
                    AddTransaction.CreateTransaction(SelectedKittyId);
                    break;
                case 2:
                    // Call a method to leave a comment
                    CreateComment();
                    break;

                case 3:
                    ShowTransactions.ShowKittyTransactions(SelectedKittyId);
                    break;

                case 4:
                    ShowComments(SelectedKittyId);
                    // Call a method to leave a comment

                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    AskForAction();
                    break;
            }
        }

        private static void ShowComments(int kittyId)
        {
            using (var dbContext = new KittySplitDbContext())
            {
                var comments = dbContext.Comments
                    .Include(c => c.User) // Include User entity to avoid null reference issues
                    .Where(c => c.KittyID == kittyId)
                    .ToList();

                if (comments.Any())
                {
                    Console.WriteLine("All Comments:");
                    foreach (var comment in comments)
                    {
                        if (comment.User != null) // Check if User is not null
                        {
                            Console.WriteLine($"{comment.User.UserName} - {comment.CreatedAt}");
                            Console.WriteLine($"Comment: {comment.Content}");
                            Console.WriteLine("------------------------------/n");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No comments found for the selected kitty.");
                }
            }
        }
    }
}
