using System.Globalization;

namespace Zidium.Core.Common.Helpers
{
    public static class NumbersHelper
    {
        public static string Money(decimal? value)
        {
            if (!value.HasValue)
                return string.Empty;

            return value.Value.ToString("#0.00", CultureInfo.InvariantCulture);
        }

        public static string Amount(long? value)
        {
            if (!value.HasValue)
                return string.Empty;

            return value.Value.ToString("#,0", AmountCultureInfo);
        }

        public static string Amount(double? value)
        {
            if (!value.HasValue)
                return string.Empty;

            return value.Value.ToString("#,0.##", AmountCultureInfo);
        }

        internal static CultureInfo AmountCultureInfo;

        static NumbersHelper()
        {
            AmountCultureInfo = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            AmountCultureInfo.NumberFormat.NumberGroupSeparator = " ";
            AmountCultureInfo.NumberFormat.NumberGroupSizes = new[] { 3 };
            AmountCultureInfo.NumberFormat.NumberDecimalSeparator = ".";
        }
    }
}