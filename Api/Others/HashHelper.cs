using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Zidium.Api
{
    /// <summary>
    /// Хэлпер для вычисления хэшей
    /// </summary>
    public static class HashHelper
    {
        private static MD5 md5 = MD5.Create();
        private static Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// Возвращает хэш MD5 = 128 бит = 16 байт
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] GetMd5(string text)
        {
            if (text == null)
            {
                text = "empty string";
            }
            byte[] data = encoding.GetBytes(text);
            lock (typeof(HashHelper))
            {
                return md5.ComputeHash(data);
            }
        }

        public static int GetInt32(string text)
        {
            byte[] data = GetMd5(text);
            int count = data.Length / 4; // int = 4 байта
            int result = 0;
            for (int i = 0; i < count; i++)
            {
                int integer = BitConverter.ToInt32(data, i * 4);
                result = result ^ integer;
            }
            return result;
        }

        public static long GetInt64(string text)
        {
            if (text == null)
            {
                return 20;
            }
            if (text == string.Empty)
            {
                return 30;
            }
            byte[] data = GetMd5(text);
            int count = data.Length / 8; // long = 8 байт
            long result = 0;
            for (int i = 0; i < count; i++)
            {
                long integer = BitConverter.ToInt64(data, i * 8);
                result = result ^ integer;
            }
            return result;
        }

        public static long GetInt64(params string[] values)
        {
            string key = string.Join(":", values);
            return GetInt64(key);
        }

        public static string GetInt32Dig5(string text)
        {
            var result = GetInt32(text);
            return Math.Abs(result % 100000).ToString(CultureInfo.InvariantCulture).PadLeft(5, '0');
        }
    }
}
