using System;
using System.Linq;

namespace Zidium.Core.Common.Helpers
{
    public static class PhoneHelper
    {
        public static string NormalizePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                throw new ArgumentNullException("phone");

            // Оставим только цифры
            phone = new string(phone.Where(char.IsDigit).ToArray());

            // Должно быть хотя бы 11 символов
            if (phone.Length < 11)
                throw new Exception("Некорректная длина телефона: " + phone);

            return phone;
        }
    }
}
