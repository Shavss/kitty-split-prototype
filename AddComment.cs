using System;
namespace Kitty
{
    // Add comment function that inputs the data into Comments table

    public class AddComment
    {
        public void CreateComment(int userId, int kittyId)
        {
            using (var dbContext = new KittySplitDbContext())
            {
                Console.WriteLine("Enter your comment:");
                string commentText = Console.ReadLine();

                var comment = new Comment(userId, kittyId, commentText);

                dbContext.Comments.Add(comment);
                dbContext.SaveChanges();
            }
        }

    }
}
