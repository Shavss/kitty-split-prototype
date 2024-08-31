using System;
using Microsoft.EntityFrameworkCore;

namespace Kitty
{
    public class ShowTransactions
    {
        public static void ShowKittyTransactions(int kittyId)
        {
            using (var dbContext = new KittySplitDbContext())
            {
                var transactions = dbContext.Transactions
                    .Include(t => t.User)
                    .Include(t => t.Kitty)
                    .ThenInclude(k => k.KittyMembers)
                    .ThenInclude(km => km.User)
                    .Where(t => t.KittyID == kittyId)
                    .GroupBy(t => new { t.Description, t.UserID, t.Amount})
                    .Select(group => group.First())
                    .ToList();


                foreach (var transaction in transactions)
                {
                    string action = GetTransactionAction(transaction.Label);
                    string shareInfo = GetShareInfo(transaction.Label, transaction.AmountSplit, transaction);

                    Console.WriteLine($"{transaction.User.UserName} {action} {transaction.ChosenCurrency} {Math.Abs(transaction.Amount)} {transaction.Description}");
                    Console.WriteLine($"{shareInfo}");
                    Console.WriteLine($"People involved: {GetInvolvedPeople(transaction.Kitty.KittyMembers)}, {transaction.Date:MM/dd/yyyy}\n");
                }
            }
        }

        private static string GetTransactionAction(string label)
        {
            switch (label)
            {
                case "Income":
                    return "received";
                case "Expense":
                    return "paid";
                case "Money Given":
                    return "gave";
                default:
                    return "performed";
            }
        }

        private static string GetShareInfo(string label, float shareAmount, Transaction transaction)
        {
            if (label == "Money Given" || label == "Expense")
            {
                return $"Your share: {transaction.ChosenCurrency} {shareAmount}";
            }
            else
            {
                return string.Empty;
            }
        }

        private static string GetInvolvedPeople(List<KittyMember> members)
        {
            return string.Join(", ", members.Select(m => m.User.UserName));
        }
    }
}
