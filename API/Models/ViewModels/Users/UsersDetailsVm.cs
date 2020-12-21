using System.ComponentModel;
using API.Models.ViewModels.Base;

namespace API.Models.ViewModels.Users
{
    public class UsersDetailsVM : BaseDetailsVM
    {
        public string Username { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        [DisplayName("Role")]
        public string RoleName { get; set; }

        [DisplayName("Team")]
        public string TeamName { get; set; }

        [DisplayName("Led teams")]
        public int LedTeams { get; set; }
    }
}