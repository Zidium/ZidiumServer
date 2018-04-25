using System;
using System.Security.Cryptography;
using System.Text;

namespace Zidium.Core.Common
{
    /// <summary>
    /// Хэлпер для вычисления хэшей
    /// </summary>
    public class HashHelper
    {
        protected static Encoding Encoding = Encoding.UTF8;

        /// <summary>
        /// Возвращает хэш MD5 = 128 бит = 16 байт
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] GetMD5(string text)
        {
            if (text == null)
            {
                text = "empty string";
            }
            byte[] data = Encoding.GetBytes(text);
            return MD5.Create().ComputeHash(data);
        }

        public static int GetInt32(Guid value)
        {
            return value.GetHashCode();
        }

        public static int GetInt32(string text)
        {
            byte[] data = GetMD5(text);
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
            byte[] data = GetMD5(text);
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

        public static string BytesToString(byte[] data)
        {
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();            
        }
    }
}
