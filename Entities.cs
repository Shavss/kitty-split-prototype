using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kitty
{
    public class Currency
    {
        [Key]
        public int CurrencyID { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }

        private Currency() { }

        public Currency(string currencyName, string currencyCode)
        {
            this.CurrencyName = currencyName;
            this.CurrencyCode = currencyCode;
        }
    }

    public class Kitty
    {
        [Key]
        public int KittyID { get; set; }
        public string EventName { get; set; }
        public bool SuperKitty { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyID { get; set; }
        public Currency Currency { get; set; }

        public List<Transaction> Transactions { get; set; }
        public List<KittyMember> KittyMembers { get; set; }
        public List<Comment> Comments { get; set; }

        public Kitty() { }

        public Kitty(string eventName, int currencyID, bool superKitty)
        {
            this.EventName = eventName;
            this.CurrencyID = currencyID;
            this.SuperKitty = superKitty;

        }
    }

    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public bool Seen { get; set; }

        public User() { }

        public User(string userName, string userEmail, bool seen)
        {
            this.UserName = userName;
            this.UserEmail = userEmail;
            this.Seen = seen;
        }

        public List<Transaction> Transactions { get; set; }
        public List<KittyMember> KittyMembers { get; set; }
        public List<Comment> Comments { get; set; }
    }

    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public string ChosenCurrency { get; set; }
        public float Amount { get; set; }
        public string OwedBy { get; set; }
        public float AmountSplit { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        [ForeignKey("Kitty")]
        public int KittyID { get; set; }
        public Kitty Kitty { get; set; }


        public Transaction() { }

        public Transaction(int kittyID, string label, int userID, string description, string chosenCurrency, float amount, string owedBy, int amountSplit, DateTime date)
        {
            this.KittyID = kittyID;
            this.Label = label;
            this.UserID = userID;
            this.Description = description;
            this.ChosenCurrency = chosenCurrency;
            this.Amount = amount;
            this.OwedBy = owedBy;
            this.AmountSplit = amountSplit;
            this.Date = date;

        }
    }


    public class KittyMember
    {
        [ForeignKey("Kitty")]
        public int KittyID { get; set; }
        public Kitty Kitty { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }
        public KittyMember() { }

        public KittyMember(int kittyId, int userId)
        {
            KittyID = kittyId;
            UserID = userId;
        }

    }

    public class Comment
    {
        [Key]
        public int CommentID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        [ForeignKey("Kitty")]
        public int KittyID { get; set; }
        public Kitty Kitty { get; set; }

        public Comment() { }

        public Comment(int userID, int kittyID, string content)
        {
            UserID = userID;
            KittyID = kittyID;
            Content = content;
            CreatedAt = DateTime.Now;
        }
    }
}
