using System.Collections.Generic;
using API.Models.ViewModels.Base;
using System.ComponentModel;
using API.Models.ViewModels.Roles;

namespace API.Models.ViewModels.Users
{
    public class UsersEditVM : BaseEditVM
    {
        public string Username { get; set; }

        public string Password { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        public int RoleId { get; set; }

        public List<RolesPair> Roles { get; set; }
    }
}