using System;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Helpers
{
    public class UserSettingsHelper
    {
        private Guid _accountId;
        private Guid _userId;
        private IUserSettingRepository _repository;

        public UserSettingsHelper(Guid accauntId, Guid userId, IUserSettingRepository repository)
        {
            _accountId = accauntId;
            _userId = userId;
            _repository = repository;
        }

        private string GetStringOrNull(string key)
        {
            return _repository.GetValue(_userId, key);
        }

        private void SetValue(string key, string value)
        {
            _repository.SetValue(_userId, key, value);
        }

        public void SetBool(string key, bool value)
        {
            var valueToString = value ? "true" : "false";
            _repository.SetValue(_userId, key, valueToString);  
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