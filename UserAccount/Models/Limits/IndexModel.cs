using System;
using System.Collections.Generic;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models.Limits
{
    public class IndexModel
    {
        public GetAccountLimitsResponseData Limits { get; set; }

        public GetLogicSettingsResponseData LogicSettings { get; set; }

        public Dictionary<Guid, UnitTestInfo> UnitTests { get; set; }

        public class UnitTestInfo
        {
            public Guid Id;

            public UnitTestTypeInfo Type;

            public string DisplayName;

            public ComponentInfo Component;
        }

        public class UnitTestTypeInfo
        {
            public Guid Id;

            public string DisplayName;

            public override bool Equals(object obj)
            {
                return Id == (obj as UnitTestTypeInfo)?.Id;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }
        }

        public class ComponentInfo
        {
            public string DisplayName;
        }
    }
}