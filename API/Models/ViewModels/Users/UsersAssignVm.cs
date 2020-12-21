using System.Collections.Generic;
using API.Models.ViewModels.Base;
using API.Models.ViewModels.Teams;

namespace API.Models.ViewModels.Users
{
    public class UsersAssignVM : BaseAssignVM
    {
        public int TeamId { get; set; }

        public List<TeamsPair> Teams { get; set; }
    }
}