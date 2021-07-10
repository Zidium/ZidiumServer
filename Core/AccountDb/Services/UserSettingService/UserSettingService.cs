using System;
using System.Globalization;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class UserSettingService : IUserSettingService
    {
        public UserSettingService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        public bool ShowComponentsAsList(Guid userId)
        {
            var value = _storage.UserSettings.GetValue(userId, ShowComponentsAsListSettingName);
            return value == "1";
        }

        public void ShowComponentsAsList(Guid userId, bool value)
        {
            _storage.UserSettings.SetValue(userId, ShowComponentsAsListSettingName, value ? "1" : "0");
        }

        public static string ShowComponentsAsListSettingName = "ShowComponentsAsList";

        public bool SendMeNews(Guid userId)
        {
            var value = _storage.UserSettings.GetValue(userId, SendMeNewsSettingName);
            return value == "1";
        }

        public void SendMeNews(Guid userId, bool value)
        {
            _storage.UserSettings.SetValue(userId, SendMeNewsSettingName, value ? "1" : "0");
        }

        public static string SendMeNewsSettingName = "SendMeNews";

        public int? ComponentHistoryInterval(Guid userId)
        {
            var valueStr = _storage.UserSettings.GetValue(userId, ComponentHistoryIntervalSettingName);
            if (int.TryParse(valueStr, NumberStyles.None, CultureInfo.InvariantCulture, out var result))
                return result;
            return null;
        }

        public void ComponentHistoryInterval(Guid userId, int value)
        {
            _storage.UserSettings.SetValue(userId, ComponentHistoryIntervalSettingName, value.ToString(CultureInfo.InvariantCulture));
        }

        public static string ComponentHistoryIntervalSettingName = "ComponentHistoryInterval";

        /// <summary>
        /// Смещение часового пояса относительно UTC, в минутах
        /// </summary>
        public int TimeZoneOffsetMinutes(Guid userId)
        {
            var valueStr = _storage.UserSettings.GetValue(userId, TimeZoneOffsetMinutesSettingName);
            if (int.TryParse(valueStr, NumberStyles.None, CultureInfo.InvariantCulture, out var result))
                return result;
            return 3 * 60; // +03:00 UTC
        }

        /// <summary>
        /// Смещение часового пояса относительно UTC, в минутах
        /// </summary>
        public void TimeZoneOffsetMinutes(Guid userId, int value)
        {
            _storage.UserSettings.SetValue(userId, TimeZoneOffsetMinutesSettingName, value.ToString(CultureInfo.InvariantCulture));
        }

        public static string TimeZoneOffsetMinutesSettingName = "TimeZoneOffsetMinutes";
    }
}
