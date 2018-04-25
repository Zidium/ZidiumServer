using System.Text.RegularExpressions;

namespace Zidium.Core.Common
{
    public static class ValidationHelper
    {
        public const string InvalidFormatMessage = "Неверный формат";

        public const string NotAEmail = "Это непохоже на email";

        public const string EmailRegex = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

        public static bool IsRegexValid(string pattern, string value)
        {
            if (value == null)
            {
                return false;
            }
            return Regex.IsMatch(value, pattern);
        }

        public static bool IsEmail(string value)
        {
            return IsRegexValid(EmailRegex, value);
        }
    }
}