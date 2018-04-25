using System;
using System.Text;

namespace Zidium.Api.Dto
{
    public class JsonSerializer : ISerializer
    {
        public JsonSerializer()
        {
            Init();
        }

        private void Init()
        {
            JsonClasses.JSON.Parameters.UseEscapedUnicode = false;
            JsonClasses.JSON.Parameters.UseExtensions = false;
            JsonClasses.JSON.Parameters.UseFastGuid = false;
            JsonClasses.JSON.Parameters.SerializeNullValues = false;
            JsonClasses.JSON.Parameters.EnableAnonymousTypes = true;
            JsonClasses.JSON.Parameters.ShowReadOnlyProperties = false;
            JsonClasses.JSON.RegisterCustomType(typeof(ExtentionPropertyCollection), SerializeProperty, ParseProperty);
        }

        public byte[] GetBytes(object obj)
        {
            string json = JsonClasses.JSON.ToJSON(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public string GetString(object obj)
        {
            string json = JsonClasses.JSON.ToJSON(obj);
            return json;
        }

        public object GetObject(Type type, byte[] bytes)
        {
            string json = Encoding.UTF8.GetString(bytes);
            return JsonClasses.JSON.ToObject(json, type);
        }

        public object GetObject(string json)
        {
            return JsonClasses.JSON.ToObject(json);
        }

        public T GetObject<T>(string json)
        {
            return JsonClasses.JSON.ToObject<T>(json);
        }

        public string Format
        {
            get { return "json"; }
        }

        private string SerializeProperty(object collection)
        {
            return "ttt";
        }

        private object ParseProperty(string json)
        {
            return new ExtentionPropertyCollection();
        }
    }
}
