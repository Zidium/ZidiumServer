using System;
using System.IO;
using System.Text;

namespace Zidium.Api.Dto
{
    public class XmlSerializer : ISerializer
    {
        public byte[] GetBytes(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            var serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            using (var textWriter = new Utf8StringWriter())
            {
                serializer.Serialize(textWriter, obj);
                var xml = textWriter.ToString();
                return Encoding.UTF8.GetBytes(xml);
            }
        }

        public string GetString(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            var serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            using (var textWriter = new Utf8StringWriter())
            {
                serializer.Serialize(textWriter, obj);
                var xml = textWriter.ToString();
                return xml;
            }
        }

        public string LastXml { get; protected set; }

        public object GetObject(Type type, byte[] bytes)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(type);
            var xml = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            LastXml = xml;
            using (var reader = new StringReader(xml))
            {
                var result = serializer.Deserialize(reader);
                return result;
            }
        }

        public string Format
        {
            get { return "xml"; }
        }
    }
}
