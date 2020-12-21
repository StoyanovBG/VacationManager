using System.Collections.Generic;
using API.Models.Filters;
using API.Models.ViewModels.Base;

namespace API.Models.ViewModels.Roles
{
    public class RolesIndexVM : BaseIndexVM
    {
        public List<RolesVM> Items { get; set; }

        public RolesFilterVM FilterVm { get; set; }
    }
}