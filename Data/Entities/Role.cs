using System.Collections.Generic;

namespace Data.Entities
{
    public class Role : BaseEntity
    {
        public Role()
        {
            this.Users = new HashSet<User>();
        }

        public Role(string name) : this()
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
