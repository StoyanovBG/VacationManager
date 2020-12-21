using System.Collections.Generic;
using API.Models.ViewModels.Base;
using API.Models.ViewModels.Projects;
using API.Models.ViewModels.Users;

namespace API.Models.ViewModels.Teams
{
    public class TeamsDetailsVM : BaseDetailsVM
    {
        public string Name { get; set; }

        public ProjectsPair Project { get; set; }

        public UsersPair TeamLead { get; set; }

        public List<UsersVM> Developers { get; set; }
    }
}