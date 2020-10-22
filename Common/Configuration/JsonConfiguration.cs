using System.IO;
using Zidium.Api;
using Zidium.Api.Dto;

namespace Zidium.Common
{
    public abstract class JsonConfiguration<T>
    {
        protected JsonConfiguration(string filename = null)
        {
            _filename = filename ?? "appsettings";
            _jsonSerializer = new JsonSerializer();
        }

        private readonly string _filename;

        private readonly JsonSerializer _jsonSerializer;

        private T _data;

        protected T Get()
        {
            if (_data == null)
                _data = Load();
            return _data;
        }

        private T Load()
        {
            var jsonFileName = Path.Combine(Tools.GetApplicationDir(), _filename + ".user.json");
            if (!File.Exists(jsonFileName))
                jsonFileName = Path.Combine(Tools.GetApplicationDir(), _filename + ".json");
            var json = File.ReadAllText(jsonFileName);
            return _jsonSerializer.GetObject<T>(json);
        }

    }
}
