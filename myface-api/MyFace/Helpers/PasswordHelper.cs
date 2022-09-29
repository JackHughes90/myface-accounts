using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MyFace.Helpers
{
    public static class PasswordHelper
    {
        public static byte[] SaltGenerator()
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            return salt;
        }

        public static string HashGenerator(string password, byte[] salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

            return hashed;
        }

        public static (string Username, string Password) DecodeAuthHeader(string authorization)
        {
            string encodedUsernamePassword = authorization.Substring("Basic ".Length);
            string usernamePassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
            string[] usernamePasswordArray = usernamePassword.Split(':');
            string username = usernamePasswordArray[0];
            string password = usernamePasswordArray[1];  

            return (username, password);          

        }
    }
}