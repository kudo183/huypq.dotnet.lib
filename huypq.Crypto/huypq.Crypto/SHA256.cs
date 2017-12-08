using System.Security.Cryptography;
using System.Text;

namespace huypq.Crypto
{
    public static class SHA256Utils
    {
        public static string ComputeBase64UrlEncodeHash(string text)
        {
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return Base64UrlEncoder.Encode(challengeBytes);
            }
        }
    }
}
