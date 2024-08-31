using System;
using System.Collections.Generic;
using System.Linq;

namespace Kitty
{
    public class AddKitty
    {

        // Create a Kitty (Expense Group)

        public static Kitty CreateKitty()
        {
            using (var dbContext = new KittySplitDbContext())

            {


                Console.WriteLine("Creating KittySplit...");

                string eventName = GetEventName();
                int homeCurrencyId = GetHomeCurrencyId();
                List<User> participants = GetParticipants();

                var newKitty = SaveKittyToDatabase(eventName, homeCurrencyId, participants);

                Console.WriteLine($"Kitty '{newKitty.EventName}' created successfully!");

                return newKitty;
            }

        }


        //  1.    Event name field
        private static string GetEventName()
        {
            return UserInput.GetInput("Enter event name", s => s);
        }

        //  2.    Home currency field
        private static int GetHomeCurrencyId()
        {
            using (var dbContext = new KittySplitDbContext())
            {
                var currencies = dbContext.Currencies.ToList();

                IDropdown<Currency> currencySelector = new AppDropdown<Currency>();
                Currency selectedCurrency = currencySelector.ChooseSingleItem(currencies, c => c.CurrencyID, c => $"{c.CurrencyID}. {c.CurrencyCode} - {c.CurrencyName}");

                return selectedCurrency.CurrencyID;
            }
        }


        //  3. Participants field
        private static List<User> GetParticipants()
        {
            Console.Write("Enter the number of participants (including yourself): ");
            var participants = new List<User>();

            using (var dbContext = new KittySplitDbContext())
            {
                int numberOfParticipants;
                while (true)
                {
                    if (int.TryParse(Console.ReadLine(), out numberOfParticipants) && numberOfParticipants > 0)
                    {
                        break;
                    }
                    Console.WriteLine("Invalid input. Please enter a valid number greater than 0.");
                }

                for (int i = 1; i <= numberOfParticipants; i++)
                {
                    Console.Write($"Enter the name for participant {i}: ");
                    var userName = Console.ReadLine();

                    string userEmail = "";
                    if (i == 1)
                    {
                        Console.Write("Enter your email: ");
                        userEmail = Console.ReadLine();
                    }

                    userEmail ??= "unknown";

                    var newUser = new User(userName, userEmail, false);
                    participants.Add(newUser);
                    dbContext.Users.Add(newUser);
                }
                dbContext.SaveChanges();
            }
            return participants;
        }

        // Save function

        private static Kitty SaveKittyToDatabase(string eventName, int homeCurrencyId, List<User> participants)
        {
            var newKitty = new Kitty(eventName, homeCurrencyId, false);
            using (var dbContext = new KittySplitDbContext())
            {
                dbContext.Kitties.Add(newKitty);
                dbContext.SaveChanges();

                // Create associations in KittyMember table
                foreach (var participant in participants)
                {
                    var kittyMember = new KittyMember(newKitty.KittyID, participant.UserID);
                    dbContext.KittyMembers.Add(kittyMember);
                }

                dbContext.SaveChanges();
            }

            return newKitty;
        }
    }
}
