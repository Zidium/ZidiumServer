using System;
using Zidium.Core.Api;
using Zidium.Storage;

namespace Zidium.Core.AccountDb
{
    /// <summary>
    /// Кеширует и управляет ключами доступа к Api
    /// </summary>
    internal interface IApiKeyService
    {
        public void Load(IStorage storage);

        public bool HasKey(string key);

        public ApiKeyForRead[] GetAll();

        public ApiKeyForRead GetById(Guid id);

        public void Add(IStorage storage, AddApiKeyRequestData data);

        public void Update(IStorage storage, UpdateApiKeyRequestData data);

        public void Delete(IStorage storage, Guid id);

        public delegate void OnKeyChangeHandler(string key);

        public event OnKeyChangeHandler OnKeyChange;
    }
}
