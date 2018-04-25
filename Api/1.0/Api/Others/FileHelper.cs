using System.IO;
using System.Text;

namespace Zidium.Api
{
    internal static class FileHelper
    {
        public static void AppendAllText(string path, string contents, Encoding encoding)
        {
            using (var streamWriter = new StreamWriter(path, true, encoding))
            {
                streamWriter.Write(contents);
            }
        }

        public static string[] GetFiles(string path, string searchPattern)
        {
            #if !PocketPC
            return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
            #else
            return Directory.GetFiles(path, searchPattern);
            #endif
        }
    }
}
