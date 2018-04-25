using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Zidium.Api.Dto
{
    public class XmlDeflateSerializer: ISerializer
    {
        protected ISerializer xmlSerializer = new XmlSerializer();

        public byte[] GetBytes(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            var xmlBytes = xmlSerializer.GetBytes(obj);
            var xml = Encoding.UTF8.GetString(xmlBytes, 0, xmlBytes.Length);
            using (var output = new MemoryStream())
            {
                using (var gzip = new DeflateStream(output, CompressionMode.Compress))
                {
                    using (var writer = new StreamWriter(gzip, Encoding.UTF8))
                    {
                        writer.Write(xml);
                    }
                }
                return output.ToArray();
            }
        }

        public string GetString(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            var xml = xmlSerializer.GetString(obj);
            return xml;
        }

        public object GetObject(Type type, byte[] bytes)
        {
            using (var inputStream = new MemoryStream(bytes))
            {
                using (var gzip = new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(gzip, Encoding.UTF8))
                    {
                        string xml = reader.ReadToEnd();
                        var xmlBytes = Encoding.UTF8.GetBytes(xml);
                        return xmlSerializer.GetObject(type, xmlBytes);
                    }
                }
            }
        }

        public string Format
        {
            get { return "octet-stream"; }
        }
    }
}
