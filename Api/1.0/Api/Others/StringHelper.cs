using System;
using System.Text;

namespace Zidium.Api.Others
{
    public class StringHelper
    {
        public static bool Contains(string source, string findText, StringComparison comparison)
        {
            int index = source.IndexOf(findText, comparison);
            return index >= 0;
        }

        public static string ReplaceComparison(string source, string oldString, string newString, StringComparison comparison)
        {
            if (source.Length == 0 || oldString.Length == 0)
                return source;

            var result = new StringBuilder();
            int startingPos = 0;
            int nextMatch;
            while ((nextMatch = source.IndexOf(oldString, startingPos, comparison)) > -1)
            {
                result.Append(source, startingPos, nextMatch - startingPos);
                result.Append(newString);
                startingPos = nextMatch + oldString.Length;
            }
            result.Append(source, startingPos, source.Length - startingPos);

            return result.ToString();
        }

        public static string TrimLenght(string text, int maxLength)
        {
            if (text == null)
            {
                return null;
            }
            if (text.Length > maxLength)
            {
                return text.Substring(0, maxLength);
            }
            return text;
        }

        public static void SetMaxLength(ref string text, int maxLength)
        {
            if (text != null && text.Length > maxLength)
            {
                text = text.Substring(0, maxLength);
            }
        }

        public static int GetLength(string text)
        {
            if (text == null)
            {
                return 0;
            }
            return text.Length;
        }

        /// <summary>
        /// Размер строки в памяти
        /// По статье https://codeblog.jonskeet.uk/2011/04/05/of-memory-and-strings/
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int GetLengthInMemory(string text)
        {
            var result = 2 + IntPtr.Size;
            result += text != null ? text.Length * 2 : 0;
            return result;
        }

        // Почему этот метод в StringHelper, если он считает размер свойства, а не строки?
        // Почему именно такой алгоритм?
        public static int GetPropertySize(string value)
        {
            if (value == null)
            {
                return IntPtr.Size; // размер ссылки
            }
            return IntPtr.Size + Encoding.Unicode.GetByteCount(value);
        }
    }
}
