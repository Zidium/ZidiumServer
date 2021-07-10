using System.Globalization;

namespace Zidium.Core
{
    public static class CultureHelper
    {
        public static readonly CultureInfo Russian;

        static CultureHelper()
        {
            Russian = new CultureInfo("ru-RU")
            {
                DateTimeFormat =
                {
                    ShortDatePattern = "dd.MM.yyyy",
                    DateSeparator = "."
                },
                NumberFormat =
                {
                    NumberDecimalSeparator = ".",
                    NumberGroupSeparator = string.Empty
                }
            };
        }
    }
}
