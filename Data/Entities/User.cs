using System.Collections.Generic;
using Data.Entities.TimeOffs;

namespace Data.Entities
{
    public class User : BaseEntity
    {
        public User()
        {
            this.LedTeams = new HashSet<Team>();
            this.PaidTimeOffRequests = new HashSet<PaidTimeOff>();
            this.UnpaidTimeOffRequests = new HashSet<UnpaidTimeOff>();
            this.SickTimeOffRequests = new HashSet<SickTimeOff>();
        }
        public string Username { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        public int? TeamId { get; set; }

        public virtual Team Team { get; set; }

        public virtual ICollection<Team> LedTeams { get; set; }

        public virtual ICollection<PaidTimeOff> PaidTimeOffRequests { get; set; }

        public virtual ICollection<UnpaidTimeOff> UnpaidTimeOffRequests { get; set; }

        public virtual ICollection<SickTimeOff> SickTimeOffRequests { get; set; }
    }
}
