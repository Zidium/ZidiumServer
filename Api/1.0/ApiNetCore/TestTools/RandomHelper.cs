using System;
using System.Collections.Generic;

namespace Zidium.Core.Common
{
    public class RandomHelper
    {
        private static Random _random = new Random();

        /// <summary>
        /// Возвращает случайный элемент из списка
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandomItemFromList<T>(List<T> list)
        {
            lock (_random)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                if (list.Count == 0)
                {
                    throw new Exception("Нельзя получить случайный элемент из пустого списка");
                }
                var index = _random.Next(0, list.Count);
                return list[index];
            }
        }

        public static bool GetRandomBool()
        {
            lock (_random)
            {
                return _random.Next(0, 2) == 0;
            }
        }

        public static byte[] GetRandomBytes(int length)
        {
            var result = new byte[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = GetRandomByte();
            }
            return result;
        }

        public static int GetRandomInt32(int minValue, int maxValue)
        {
            lock (_random)
            {
                var buf = new byte[4];
                _random.NextBytes(buf);
                var longRand = BitConverter.ToInt32(buf, 0);
                return (Math.Abs(longRand % (maxValue - minValue)) + minValue);
            }
        }

        public static byte GetRandomByte()
        {
            lock (_random)
            {
                return (byte)(_random.Next() % 256);
            }
        }

        public static DateTime GetRandomDate(DateTime min, DateTime max)
        {
            var seconds = (long)(max - min).TotalSeconds;
            var val = GetRandomInt64(0, seconds);
            return min + TimeSpan.FromSeconds(val);
        }

        public static long GetRandomInt64(long minValue, long maxValue)
        {
            lock (_random)
            {
                var buf = new byte[8];
                _random.NextBytes(buf);
                var longRand = BitConverter.ToInt64(buf, 0);
                return (Math.Abs(longRand % (maxValue - minValue)) + minValue);
            }
        }

        public static string GetRandomStringAZ(int minLength, int maxLength)
        {
            var chars = new char[]
            {
                // маленькие буквы
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                // большие буквы
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
            };
            return GetRandomString(chars, minLength, maxLength);
        }

        public static string GetRandomStringAZ09(int minLength, int maxLength)
        {
            var chars = new char[]
            {
                // маленькие буквы
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                // большие буквы
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                // цифры
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            return GetRandomString(chars, minLength, maxLength);
        }

        public static string GetRandomString09(int minLength, int maxLength)
        {
            var chars = new char[]
            {
                // цифры
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            return GetRandomString(chars, minLength, maxLength);
        }

        public static string GetRandomString(char[] chars, int minLength, int maxLength)
        {
            lock (_random)
            {
                var length = minLength;
                if (maxLength > minLength)
                {
                    length = length + GetRandomInt32(0, maxLength - minLength);
                }

                var result = new char[length];
                for (var index = 0; index < length; index++)
                {
                    var pos = _random.Next(0, chars.Length - 1);
                    result[index] = chars[pos];
                }

                return new string(result);
            }
        }
    }
}
