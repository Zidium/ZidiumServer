using System;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models.ComponentTreeDetails
{
    public class ComponentDetailsUnittestsModel
    {
        public Guid Id { get; set; }

        public bool CanEdit { get; set; }

        public SystemUnittest[] SystemUnittests { get; set; }

        public UserUnittest[] UserUnittests { get; set; }

        public class SystemUnittest
        {
            public Guid Id { get; set; }

            public MonitoringStatus Status { get; set; }

            public TimeSpan StatusDuration { get; set; }

            public string Name { get; set; }

            public string TypeName { get; set; }

            public DateTime LastResultDate { get; set; }

            public string LastResult { get; set; }

            public TimeSpan Interval { get; set; }

            public bool IsEnabled { get; set; }
        }

        public class UserUnittest
        {
            public Guid Id { get; set; }

            public MonitoringStatus Status { get; set; }

            public TimeSpan StatusDuration { get; set; }

            public string Name { get; set; }

            public DateTime LastResultDate { get; set; }

            public string LastResult { get; set; }

            public DateTime ActualDate { get; set; }

            public TimeSpan ActualInterval { get; set; }

            public bool IsEnabled { get; set; }
        }
    }
}