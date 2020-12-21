using API.Models.ViewModels.Base;

namespace API.Models.ViewModels.Users
{
    public class UsersVM : BaseVM
    {
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string RoleName { get; set; }
    }
}