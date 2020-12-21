using System.ComponentModel;
using API.Models.ViewModels.Base;

namespace API.Models.ViewModels.Roles
{
    public class RolesVM : BaseVM
    {
        public string Name { get; set; }

        [DisplayName("Assigned users")]
        public int AssignedUsersCount { get; set; }
    }
}