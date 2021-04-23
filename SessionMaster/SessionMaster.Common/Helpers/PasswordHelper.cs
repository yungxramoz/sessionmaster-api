using System;
using System.Security.Cryptography;
using System.Text;

namespace SessionMaster.Common.Helpers
{
    public static class PasswordHelper
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            BaseParamValidation(password);

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            BaseParamValidation(password);

            if (passwordHash.Length != 64)
            {
                throw new ArgumentException("Invalid password hash (64 bytes expected)", "passwordHash");
            }
            if (passwordSalt.Length != 128)
            {
                throw new ArgumentException("Invalid password salt(128 bytes expected)", "passwordSalt");
            }

            using (var hmac = new HMACSHA512(passwordSalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void BaseParamValidation(string password)
        {
            if (password == null)
            {
                throw new ArgumentException("Value cannot be null.", "password");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            }
        }
    }
}
