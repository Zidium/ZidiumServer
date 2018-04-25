using System;
using System.Globalization;

namespace Zidium.Core.AccountsDb
{
    public class UserSettingService : IUserSettingService
    {
        protected AccountDbContext Context { get; set; }

        public UserSettingService(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public bool ShowComponentsAsList(Guid userId)
        {
            var repository = Context.GetUserSettingRepository();
            var value = repository.GetValue(userId, ShowComponentsAsListSettingName);
            return value == "1";
        }

        public void ShowComponentsAsList(Guid userId, bool value)
        {
            var repository = Context.GetUserSettingRepository();
            repository.SetValue(userId, ShowComponentsAsListSettingName, value ? "1" : "0");
        }

        public static string ShowComponentsAsListSettingName = "ShowComponentsAsList";

        public bool SendMeNews(Guid userId)
        {
            var repository = Context.GetUserSettingRepository();
            var value = repository.GetValue(userId, SendMeNewsSettingName);
            return value == "1";
        }

        public void SendMeNews(Guid userId, bool value)
        {
            var repository = Context.GetUserSettingRepository();
            repository.SetValue(userId, SendMeNewsSettingName, value ? "1" : "0");
        }

        public static string SendMeNewsSettingName = "SendMeNews";

        public int? ComponentHistoryInterval(Guid userId)
        {
            var repository = Context.GetUserSettingRepository();
            var valueStr = repository.GetValue(userId, ComponentHistoryIntervalSettingName);
            int result;
            if (int.TryParse(valueStr, NumberStyles.None, CultureInfo.InvariantCulture, out result))
                return result;
            return null;
        }

        public void ComponentHistoryInterval(Guid userId, int value)
        {
            var repository = Context.GetUserSettingRepository();
            repository.SetValue(userId, ComponentHistoryIntervalSettingName, value.ToString(CultureInfo.InvariantCulture));
        }

        public static string ComponentHistoryIntervalSettingName = "ComponentHistoryInterval";
    }
}
