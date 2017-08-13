using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Runtime.CompilerServices;

namespace huypq.Crypto
{
    public static class PasswordHash
    {
        const int Pbkdf2IterCount = 1000;
        const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
        const int SaltSize = 128 / 8; // 128 bits
        const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA256;
        
        public static string HashedBase64String(string password)
        {
            return Convert.ToBase64String(HashedBytes(password));
        }

        public static byte[] HashedBytes(string password)
        {
            byte[] salt = new byte[SaltSize];
            RandomNumberGenerator.Create().GetBytes(salt);
            byte[] subkey = KeyDerivation.Pbkdf2(
                password, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);

            var outputBytes = new byte[salt.Length + subkey.Length];
            Buffer.BlockCopy(salt, 0, outputBytes, 0, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, SaltSize, Pbkdf2SubkeyLength);
            return outputBytes;
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);
            return VerifyHashedPassword(decodedHashedPassword, password);
        }

        public static bool VerifyHashedPassword(byte[] hashedPassword, string password)
        {            
            // We know ahead of time the exact length of a valid hashed password payload.
            if (hashedPassword.Length != SaltSize + Pbkdf2SubkeyLength)
            {
                return false; // bad size
            }

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(hashedPassword, 0, salt, 0, salt.Length);

            byte[] expectedSubkey = new byte[Pbkdf2SubkeyLength];
            Buffer.BlockCopy(hashedPassword, salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the incoming password and verify it
            byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);
            return ByteArraysEqual(actualSubkey, expectedSubkey);
        }

        // Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        // If a "smart" compiler turned that function into something that returns false as soon as a mismatch was found, it could make code using this function vulnerable to a "timing attack"---an attacker could conceivably figure out where the first mismatch in a string was based on how long the function took to return.
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
    }
}
