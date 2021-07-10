namespace Zidium.Core
{
    public static class DatabaseHelper
    {
        /// <summary>
        /// Убирает некорректные символы из строки
        /// </summary>
        public static string FixStringSymbols(this string value)
        {
            if (value == null)
                return null;

            if (value.Contains("\x00"))
                return value.Replace("\x00", " ");

            return value;
        }
    }
}
