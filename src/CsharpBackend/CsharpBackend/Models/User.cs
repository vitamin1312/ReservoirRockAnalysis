using System.Security.Cryptography;
using System.Text;
using CsharpBackend.Utils;

namespace CsharpBackend.Models
{
    public class User
    {

        public int Id { get; set; }
        public string Login { get; set; }

        private byte[] password;

        public string Password
        {
            get
            {
                return Hasher.HashPassword(password);
            }
            set { password = Encoding.UTF8.GetBytes(value); }
        }

        public bool IsAdmin => Login == "admin";

        public bool CheckPassword(string password) => password == Password;
    }
}
