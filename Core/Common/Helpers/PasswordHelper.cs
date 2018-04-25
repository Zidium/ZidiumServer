using System;
using System.Text;
using Microsoft.AspNet.Identity;

namespace Zidium.Core.Common
{
    public static class PasswordHelper
    {
        public static string GetRandomPassword(int length)
        {
            var charArray = new char[]
            {
                // спец символы
                '$','#','!',
                // цифры
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                // маленькие буквы
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                // большие буквы
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
            };
            var random = new Random();
            var result = new byte[length];
            for (int index = 0; index < length; index++)
            {
                int pos = random.Next(0, charArray.Length - 1);
                result[index] = (byte)charArray[pos];
            }
            return Encoding.ASCII.GetString(result);
        }

        public static string GetPasswordHashString(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            return new PasswordHasher().HashPassword(password);
        }

        public static bool VerifyHashedPassword(string hash, string password)
        {
            var flag = new PasswordHasher().VerifyHashedPassword(hash, password);
            return flag == PasswordVerificationResult.Success;
        }

    }

}
