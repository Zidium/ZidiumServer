using System.IO;
using System.Xml;

namespace Zidium.Api
{
    internal static class XmlDocumentExtentions
    {
        public static void Load(this XmlDocument document, string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                document.Load(stream);
            }
        }
    }
}
