using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Helpers
{
    public class UserSettingsHelper
    {
        public UserSettingsHelper(Guid userId, IStorage storage)
        {
            _userId = userId;
            _storage = storage;
        }

        private readonly Guid _userId;

        private readonly IStorage _storage;

        private string GetStringOrNull(string key)
        {
            return _storage.UserSettings.GetValue(_userId, key);
        }

        private void SetValue(string key, string value)
        {
            _storage.UserSettings.SetValue(_userId, key, value);
        }

        public void SetBool(string key, bool value)
        {
            var valueToString = value ? "true" : "false";
            _storage.UserSettings.SetValue(_userId, key, valueToString);  
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            var value = GetStringOrNull(key);
            if (value == null)
            {
                return defaultValue;
            }
            return value == "true";
        }
    }
}