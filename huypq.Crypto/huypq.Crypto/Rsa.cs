using System.Security.Cryptography;

namespace huypq.Crypto
{
    public static class Rsa
    {
        static HashAlgorithmName hashName = new HashAlgorithmName("SHA256");

        public static byte[] SignSHA256(byte[] data, RSAParameters key)
        {
            var rsa = RSA.Create();
            rsa.ImportParameters(key);
            return rsa.SignData(data, hashName, RSASignaturePadding.Pkcs1);
        }

        public static bool VerifySignSHA256(byte[] data, byte[] sign, RSAParameters key)
        {
            var rsa = RSA.Create();
            rsa.ImportParameters(key);
            return rsa.VerifyData(data, sign, hashName, RSASignaturePadding.Pkcs1);
        }
    }
}
