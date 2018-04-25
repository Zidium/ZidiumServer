//using System;
//using System.IO;
//using System.Xml;

//namespace Zidium.Core.Api
//{
//    public class XmlSerializer : ISerializer
//    {
//        public string GetString(object obj)
//        {
//            if (obj == null)
//            {
//                throw new ArgumentNullException("obj");
//            }
//            var serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
//            using (var textWriter = new Utf8StringWriter())
//            {
//                serializer.Serialize(textWriter, obj);
//                return textWriter.ToString();
//            }
//        }

//        public object GetObject(Type type, string text)
//        {
//            var serializer = new System.Xml.Serialization.XmlSerializer(type);
//            using (var reader = new StringReader(text))
//            {
//                var result = serializer.Deserialize(reader);
//                return result;
//            }
//        }

//        public string Format
//        {
//            get { return "xml"; }
//        }
//    }
//}
