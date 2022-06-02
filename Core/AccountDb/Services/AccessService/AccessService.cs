using System.Collections.Concurrent;

namespace Zidium.Core.AccountDb
{
    internal class AccessService : IAccessService
    {
        public AccessService(
            IAccessConfiguration accessConfiguration,
            IApiKeyService apiKeyService)
        {
            _secretKey = accessConfiguration.SecretKey;
            _apiKeyService = apiKeyService;
            _apiKeyService.OnKeyChange += (key) => Invalidate(key);
        }

        private readonly string _secretKey;
        private readonly IApiKeyService _apiKeyService;
        private readonly ConcurrentDictionary<string, bool> _apiAccess = new ConcurrentDictionary<string, bool>();

        public bool HasApiAccess(string key)
        {
            return _apiAccess.GetOrAdd(key, key =>
            {
                return HasSystemAccess(key) || _apiKeyService.HasKey(key);
            });
        }

        public bool HasSystemAccess(string key)
        {
            return _secretKey.Equals(key);
        }

        internal virtual void Invalidate(string key)
        {
            _apiAccess.TryRemove(key, out _);
        }
    }
}
