using System.Security.Cryptography;
using System.Text;

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
                var sb = new StringBuilder();
                foreach (var b in MD5.Create().ComputeHash(password))
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
            set { password = Encoding.UTF8.GetBytes(value); }
        }

        public bool IsAdmin => Login == "admin";

        public bool CheckPassword(string password) => password == Password;
    }
}
