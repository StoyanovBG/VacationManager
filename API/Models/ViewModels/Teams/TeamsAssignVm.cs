using System.Collections.Generic;
using API.Models.ViewModels.Base;
using API.Models.ViewModels.Users;

namespace API.Models.ViewModels.Teams
{
    public class TeamsAssignVM : BaseAssignVM
    {
        public int UserId { get; set; }

        public List<UsersPair> Users { get; set; }
    }
}