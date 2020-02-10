using System;

namespace Zidium.Core.AccountsDb
{
    public interface IUserSettingService
    {
        bool ShowComponentsAsList(Guid userId);

        void ShowComponentsAsList(Guid userId, bool value);

        bool SendMeNews(Guid userId);

        void SendMeNews(Guid userId, bool value);

        int? ComponentHistoryInterval(Guid userId);

        void ComponentHistoryInterval(Guid userId, int value);

        int TimeZoneOffsetMinutes(Guid userId);

        void TimeZoneOffsetMinutes(Guid userId, int value);
    }
}
