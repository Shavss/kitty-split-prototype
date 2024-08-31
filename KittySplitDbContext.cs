using System;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Kitty
{
    public class KittySplitDbContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Kitty> Kitties { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<KittyMember> KittyMembers { get; set; }
        public DbSet<Comment> Comments { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = mydatabase.db");
        }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KittyMember>()
                .HasKey(km => new { km.KittyID, km.UserID });

            // Currency to Kitty relationship
            modelBuilder.Entity<Kitty>()
                .HasOne(k => k.Currency)
                .WithMany()
                .HasForeignKey(k => k.CurrencyID);

            // User to Transaction relationship
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserID);

            // Kitty to Transaction relationship
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Kitty)
                .WithMany(k => k.Transactions)
                .HasForeignKey(t => t.KittyID);

            // Kitty to User (Members of a Kitty) relationship (Many-to-Many)
            modelBuilder.Entity<KittyMember>()
                .HasOne(km => km.Kitty)
                .WithMany(k => k.KittyMembers)
                .HasForeignKey(km => km.KittyID);

            modelBuilder.Entity<KittyMember>()
                .HasOne(km => km.User)
                .WithMany(u => u.KittyMembers)
                .HasForeignKey(km => km.UserID);

            // Comment to User relationship
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserID);

            // Comment to Kitty relationship
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Kitty)
                .WithMany(k => k.Comments)
                .HasForeignKey(c => c.KittyID);

            base.OnModelCreating(modelBuilder);
        }
    }
}

