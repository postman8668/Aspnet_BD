using KursDB.Models;

namespace KursDB.Services
{
    public class AuthService
    {
        private readonly User _adminUser = new()
        {
            Username = "admin",
            Password = "admin123"
        };

        public bool Authenticate(string username, string password)
        {
            return _adminUser.Username == username &&
                   _adminUser.Password == password;
        }
    }
}