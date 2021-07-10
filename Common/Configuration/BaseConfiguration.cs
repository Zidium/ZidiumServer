using Microsoft.Extensions.Configuration;

namespace Zidium.Common
{
    public abstract class BaseConfiguration<T> where T : new()
    {
        protected BaseConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        private T _data;

        protected T Get()
        {
            if (_data == null)
                _data = Load();
            return _data;
        }

        private T Load()
        {
            var result = new T();
            _configuration.Bind(result);
            return result;
        }

    }
}
