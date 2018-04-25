using System;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount
{
    public class UserInfo
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public string AccountName { get; set; }

        public string Login { get; set; }

        public string Name { get; set; }

        public UserInfoRole[] Roles { get; set; }

        public bool IsSwitched { get; set; }

        private UserSettingsHelper _settings = null;
        public UserSettingsHelper Settings
        {
            get
            {
                if (_settings == null)
                {
                    var accountDbContext = FullRequestContext.Current.AccountDbContext;
                    var repository = accountDbContext.GetUserSettingRepository();
                    _settings = new UserSettingsHelper(AccountId, Id, repository);
                }
                return _settings;
            }
        }
    }
}