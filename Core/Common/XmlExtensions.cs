using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Zidium.Core
{
    public static class XmlExtensions
    {
        public static string Serialize<T>(this T value) where T: class
        {
            if (value == null) 
                return string.Empty;

            var xmlserializer = new XmlSerializer(typeof(T));

            using (var stringWriter = new StringWriter())
            {
                using (var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
        }
    }
}
