using System;

namespace Zidium.Core.Common
{
    public static class VersionHelper
    {
        public static long? FromString(string version)
        {
            if (string.IsNullOrEmpty(version))
                return null;

            var parts = version.Split('.');
            if (parts.Length != 4)
                return null;

            UInt16 a;
            UInt16 b;
            UInt16 c;
            UInt16 d;

            if (!UInt16.TryParse(parts[0], out a))
                return null;
            if (!UInt16.TryParse(parts[1], out b))
                return null;
            if (!UInt16.TryParse(parts[2], out c))
                return null;
            if (!UInt16.TryParse(parts[3], out d))
                return null;

            if (a > (1 << 15) - 1)
                a = (1 << 15) - 1; // Иначе результат переполнится и станет отрицательным

            var result = ((long)a << 48) + ((long)b << 32) + ((long)c << 16) + d;
            return result;
        }
    }
}
