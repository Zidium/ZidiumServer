using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class UserSettingRepository : IUserSettingRepository
    {
        protected AccountDbContext Context;

        public UserSettingRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public string GetValue(Guid userId, string name)
        {
            var setting = Context.UserSettings.SingleOrDefault(t => t.UserId == userId && t.Name == name);
            return setting != null ? setting.Value : null;
        }

        public void SetValue(Guid userId, string name, string value)
        {
            var setting = Context.UserSettings.SingleOrDefault(t => t.UserId == userId && t.Name == name);
            if (setting == null)
            {
                setting = new UserSetting()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = name
                };
                Context.UserSettings.Add(setting);
            }
            setting.Value = value;
        }
    }
}
