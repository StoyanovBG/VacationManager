using System.Collections.Generic;
using API.Models.ViewModels.Base;
using API.Models.ViewModels.Users;

namespace API.Models.ViewModels.Roles
{
    public class RolesDetailsVM : BaseDetailsVM
    {
        public string Name { get; set; }

        public List<UsersVM> Users { get; set; }
    }
}