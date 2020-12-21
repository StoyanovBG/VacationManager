using API.Models.Filters;
using API.Models.ViewModels.Base;
using System.Collections.Generic;

namespace API.Models.ViewModels.Users
{
    public class UsersIndexVM : BaseIndexVM
    {
        public List<UsersVM> Items { get; set; }

        public UsersFilterVM Filter { get; set; }
    }
}