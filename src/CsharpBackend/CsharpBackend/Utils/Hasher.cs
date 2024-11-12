using System.Security.Cryptography;
using System.Text;

namespace CsharpBackend.Utils
{
    public class Hasher
    {
        static public string HashPassword(string password)
        {
            var sb = new StringBuilder();
            foreach (var b in MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)))
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        static public string HashPassword(byte[] password)
        {
            var sb = new StringBuilder();
            foreach (var b in MD5.Create().ComputeHash(password))
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

    }
}
