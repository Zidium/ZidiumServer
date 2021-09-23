using System;
using Zidium.Common;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class AccountSettingService : IAccountSettingService
    {
        private const string VIRUS_TOTAL_API_KEY = "VirusTotal_ApiKey";

        public AccountSettingService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        private string GetValue(string name)
        {
            var setting = _storage.AccountSettings.GetOneOrNullByName(name);
            return setting?.Value;
        }

        private void SetValue(string name, string value)
        {
            var setting = _storage.AccountSettings.GetOneOrNullByName(name);
            if (setting == null)
            {
                _storage.AccountSettings.Add(new AccountSettingForAdd()
                {
                    Id = Ulid.NewUlid(),
                    Name = name,
                    Value = value
                });
            }
            else
            {
                var settingForUpdate = setting.GetForUpdate();
                settingForUpdate.Value.Set(value);
                _storage.AccountSettings.Update(settingForUpdate);
            }
        }

        public string VirusTotalApiKey
        {
            get => GetValue(VIRUS_TOTAL_API_KEY);
            set => SetValue(VIRUS_TOTAL_API_KEY, value);
        }
    }
}
