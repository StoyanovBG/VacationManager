using Data.Entities;
using Data.Initializers;
using System.Data.Entity;
using Data.Entities.TimeOffs;

namespace Data
{
    public class OvmDbContext : DbContext
    {
        public OvmDbContext()
            : base("name=OvmDbContext")
        {
            Database.SetInitializer(new OvmDbDropCreateIfModelChanges());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOptional(u => u.Team)
                .WithMany(t => t.Developers)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasOptional(t => t.TeamLead)
                .WithMany(u => u.LedTeams)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Teams)
                .WithOptional(t => t.Project)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithRequired(u => u.Role)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Team> Teams { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<PaidTimeOff> PaidTimeOffs { get; set; }

        public virtual DbSet<UnpaidTimeOff> UnpaidTimeOffs { get; set; }

        public virtual DbSet<SickTimeOff> SickTimeOffs { get; set; }
    }
}