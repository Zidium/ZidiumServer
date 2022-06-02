using System;
using System.Collections.Concurrent;
using System.Linq;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Core.AccountDb
{
    internal class ApiKeyService : IApiKeyService
    {
        public ApiKeyService(ITimeService timeService)
        {
            _timeService = timeService;
        }

        private readonly ITimeService _timeService;
        private ConcurrentDictionary<string, ApiKeyForRead> _apiKeys;

        public event IApiKeyService.OnKeyChangeHandler OnKeyChange;

        public void Load(IStorage storage)
        {
            var apiKeys = storage.ApiKeys.GetAll();
            var apiKeysDict = apiKeys.ToDictionary(t => t.Value);
            _apiKeys = new ConcurrentDictionary<string, ApiKeyForRead>(apiKeysDict);
        }

        public bool HasKey(string key)
        {
            return _apiKeys.ContainsKey(key);
        }

        public ApiKeyForRead[] GetAll()
        {
            return _apiKeys.Values.ToArray();
        }

        public ApiKeyForRead GetById(Guid id)
        {
            var apiKey = _apiKeys.Values.FirstOrDefault(t => t.Id == id);

            if (apiKey != null)
                return apiKey;

            throw new ObjectNotFoundException("Api key with id " + id + " not found");
        }

        public void Add(IStorage storage, AddApiKeyRequestData data)
        {
            bool added = false;
            var newApiKey = _apiKeys.GetOrAdd(data.Value, (key) =>
            {
                var apiKey = new ApiKeyForAdd()
                {
                    Id = data.Id,
                    Name = data.Name,
                    Value = data.Value,
                    UpdatedAt = _timeService.Now(),
                    UserId = data.UserId
                };

                storage.ApiKeys.Add(apiKey);
                added = true;

                return storage.ApiKeys.GetOneById(apiKey.Id);
            });

            if (added)
            {
                OnKeyChange?.Invoke(newApiKey.Value);
                return;
            }

            throw new UserFriendlyException("Api key " + data.Value + " already exists");
        }

        public void Update(IStorage storage, UpdateApiKeyRequestData data)
        {
            var existingKeyValue = GetById(data.Id).Value;
            _apiKeys.AddOrUpdate(existingKeyValue,
                (_) =>
                {
                    throw new ObjectNotFoundException("Api key " + existingKeyValue + " not found");
                },
                (_, existingApiKey) =>
                {
                    var apiKey = existingApiKey.GetForUpdate();
                    apiKey.Name.Set(data.Name);
                    apiKey.UserId.Set(data.UserId);
                    apiKey.UpdatedAt.Set(_timeService.Now());
                    storage.ApiKeys.Update(apiKey);

                    return storage.ApiKeys.GetOneById(apiKey.Id);
                });

            OnKeyChange?.Invoke(existingKeyValue);
        }

        public void Delete(IStorage storage, Guid id)
        {
            var existingKeyValue = GetById(id).Value;
            if (_apiKeys.TryRemove(existingKeyValue, out _))
            {
                storage.ApiKeys.Delete(id);
                OnKeyChange?.Invoke(existingKeyValue);
            }
        }

        internal void FireOnKeyChange(string key)
        {
            OnKeyChange?.Invoke(key);
        }
    }
}
