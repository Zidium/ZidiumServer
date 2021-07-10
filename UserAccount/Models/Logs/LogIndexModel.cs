using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class LogIndexModel : LogFiltersModel
    {
        public ComponentInfo Component { get; set; }

        public class ComponentInfo
        {
            public Guid Id;

            public string DisplayName;

            public LogConfigForRead LogConfig;
        }
    }
}