using Main.Models;

namespace Main.Tests
{
    public static class Seeding
    {
        public static List<ApplicationUser> GetSeedingUsers()
        {
            List<ApplicationUser> users = new List<ApplicationUser>();

            // Creating 10 users
            for (int i = 1; i <= 10; i++)
            {
                var user = new ApplicationUser
                {
                    Id = $"user{i}Id",
                    FirstName = $"User{i}FirstName",
                    LastName = $"User{i}LastName",
                    Email = $"user{i}@mail.com",
                    NormalizedEmail = $"USER{i}@MAIL.COM"
                };

                users.Add(user);
            }

            // Return the list of users
            return users;
        }
    }
}