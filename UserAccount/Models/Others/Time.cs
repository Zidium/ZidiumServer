using System;
using System.Globalization;

namespace Zidium.UserAccount
{
    public struct Time
    {
        public int Hour { get; set; }

        public int Minute { get; set; }

        public override string ToString()
        {
            return Hour.ToString("00", CultureInfo.InvariantCulture) + ":" + Minute.ToString("00", CultureInfo.InvariantCulture);
        }

        public static Time Parse(string value)
        {
            if (value.Length != 5)
                throw new FormatException("Length of time string must be equal 5");

            if (value[2] != ':')
                throw new FormatException("Time string must have : at position 2");

            var result = new Time
            {
                Hour = int.Parse(value.Substring(0, 2)),
                Minute = int.Parse(value.Substring(3, 2))
            };

            return result;
        }
    }
}