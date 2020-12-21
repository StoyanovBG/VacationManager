using Data.Entities;
using Data.Repository;

namespace Data.Services
{
    public class AuthenticationService
    {
        public User LoggedUser { get; private set; }
        public void AuthenticateUser(string username, string password)
        {
            UserRepository userRepo = new UserRepository(new OvmDbContext());
            this.LoggedUser = userRepo.GetOne(u => u.Username == username && u.Password == password);
        }
    }
}
