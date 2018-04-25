using System.IO;
using System.Text;

namespace Zidium.Api
{
    internal static class FileHelper
    {
        public static void AppendAllText(string path, string contents, Encoding encoding)
        {
            using (var stream = new FileStream(path, FileMode.Append))
            {
                using (var streamWriter = new StreamWriter(stream, encoding))
                {
                    streamWriter.Write(contents);
                }
            }
        }

        public static string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
        }
    }
}
