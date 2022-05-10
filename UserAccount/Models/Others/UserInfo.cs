using System;

namespace Zidium.UserAccount
{
    public class UserInfo
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public string Name { get; set; }

        public UserInfoRole[] Roles { get; set; }

        // TODO Remove
        public bool IsSwitched { get; set; }

        public int TimeZoneOffsetMinutes { get; set; }
    }
}