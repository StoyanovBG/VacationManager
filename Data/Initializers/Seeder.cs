using Data.Entities;

namespace Data.Initializers
{
    public static class Seeder
    {
        public static void Seed(OvmDbContext context)
        {
            context.Teams.Add(new Team
            {
                Name = "BDE Team"
            });

            context.Projects.Add(new Project
            {
                Name = "BDE Project",
                Description = "Initial project"
            });

            context.Users.Add(new User
            {
                FirstName = "CEO",
                LastName = "CEO",
                Password = "ceo",
                Username = "ceo",
                Role = new Role("CEO"),
            });

            context.Users.Add(new User()
            {
                FirstName = "Dev",
                LastName = "Dev",
                Username = "dev",
                Password = "dev",
                Role = new Role("Developer")
            });

            context.Users.Add(new User()
            {
                FirstName = "Team",
                LastName = "Lead",
                Username = "teamlead",
                Password = "teamlead",
                Role = new Role("Team Lead")
            });

            context.Roles.Add(new Role("Unassigned"));

            context.SaveChanges();
        }
    }
}
