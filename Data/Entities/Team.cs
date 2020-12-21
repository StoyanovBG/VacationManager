using System.Collections.Generic;

namespace Data.Entities
{
    public class Team : BaseEntity
    {
        public Team()
        {
            this.Developers = new HashSet<User>();
        }

        public string Name { get; set; }

        public int? ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public int? TeamLeadId { get; set; }

        public virtual User TeamLead { get; set; }

        public virtual ICollection<User> Developers { get; set; }
    }
}
