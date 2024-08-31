# kitty-split-prototype
Database design and app for assignment

5 main functions:
1. Create a Kitty - Expense Group:
  • Description: Handles the creation of a new Kitty, captures essential data, and puts the details in the database.

2. Navigation between the Kitties and logging into one of them.
  • Description: Enables naviga-on between different Kittes and facilitates user login. Marks the selected user as "seen" in the Users table upon login to track user engagement.
  
3. Add Transac4ons to a Kitty:
  • Description: Manages adding various transactions to a Kitty, accommodting different transacion types and spliting methods. Each transaction is associated with a label (Expense, Money Given, etc.), and the splitting function determines the number of rows entered in the transaction table. We also can upgrade the kitty, which I included in the program. (Super Kitty column in the Kitty table)
   
4. Leave comments in the Group
  • Description: Allows users to leave comments within a Kitty group.

5. Display Expenses, Comments, Overview...
  • Descripion: Presentation of expenses, comments, and a financial overview of the Kitty. Offers insights into financial transactions, user interacions, and an overall summary of Kitty's financial state.
    