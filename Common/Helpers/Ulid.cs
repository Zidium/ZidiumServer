using System;

namespace Zidium.Common
{
    /// <summary>
    /// ULID - Последовательный Guid
    /// Не совсем соответствует стандарту, но для наших целей сойдёт
    /// </summary>
    public static class Ulid
    {
        public static Guid NewUlid()
        {
            lock (_lockObject)
            {
                var unixDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var datePart = new byte[8];
                var b = unixDate;

                datePart[7] = (byte)(b & 0xFF);
                b = b >> 8;
                datePart[6] = (byte)(b & 0xFF);
                b = b >> 8;
                datePart[5] = (byte)(b & 0xFF);
                b = b >> 8;
                datePart[4] = (byte)(b & 0xFF);
                b = b >> 8;
                datePart[3] = (byte)(b & 0xFF);
                b = b >> 8;
                datePart[2] = (byte)(b & 0xFF);
                b = b >> 8;
                datePart[1] = (byte)(b & 0xFF);
                b = b >> 8;
                datePart[0] = (byte)(b & 0xFF);

                var randomPart = new byte[8];
                _random.NextBytes(randomPart);
                var result = new byte[16];
                Buffer.BlockCopy(datePart, 0, result, 0, 8);
                Buffer.BlockCopy(randomPart, 0, result, 8, 8);
                return new Guid(result);
            }
        }

        private static Random _random = new Random();

        private static object _lockObject = new object();
    }
}
