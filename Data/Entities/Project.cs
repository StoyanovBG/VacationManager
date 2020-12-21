using System.Collections.Generic;

namespace Data.Entities
{
    public class Project : BaseEntity
    {
        public Project()
        {
            this.Teams = new HashSet<Team>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Team> Teams { get; set; }
    }
}
