using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class AccountSettingRepository : IAccountSettingRepository
    {
        protected AccountDbContext Context;

        public AccountSettingRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public string GetValue(string name)
        {
            var setting = Context.AccountSettings.SingleOrDefault(t => t.Name == name);
            return setting != null ? setting.Value : null;
        }

        public void SetValue(string name, string value)
        {
            var setting = Context.AccountSettings.SingleOrDefault(t => t.Name == name);
            if (setting == null)
            {
                setting = new AccountSetting()
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };
                Context.AccountSettings.Add(setting);
            }
            setting.Value = value;
        }
    }
}
