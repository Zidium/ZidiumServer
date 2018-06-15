using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Справочник ИД системных типов юнит-тестов
    /// </summary>
    public static class SystemUnitTestTypes
    {
        public static readonly UnitTestType HttpUnitTestType = new UnitTestType()
        {
            Id = new Guid("44F601AD-2946-1578-9904-517A6D3C6D64"),
            DisplayName = "Http",
            SystemName = "System.UnitTests.Http",
            IsSystem = true
        };

        public static readonly UnitTestType PingTestType = new UnitTestType()
        {
            Id = new Guid("C4B764F0-AF19-42B0-8089-1D01FC4CAD40"),
            DisplayName = "Ping",
            SystemName = "System.UnitTests.Ping",
            IsSystem = true
        };

        public static readonly UnitTestType SqlTestType = new UnitTestType()
        {
            Id = new Guid("CB35631D-8C1F-491C-A2C9-243641D72FBF"),
            DisplayName = "SQL",
            SystemName = "System.UnitTests.Sql",
            IsSystem = true
        };

        public static readonly UnitTestType DomainNameTestType = new UnitTestType()
        {
            Id = new Guid("73A92501-0B33-426F-986B-4E3C34CF44B2"),
            DisplayName = "Domain",
            SystemName = "System.UnitTests.DomainName",
            IsSystem = true
        };

        public static readonly UnitTestType SslTestType = new UnitTestType()
        {
            Id = new Guid("8B314C56-12B9-44CE-8F20-38C767EE379A"),
            DisplayName = "SSL",
            SystemName = "System.UnitTests.Ssl",
            IsSystem = true
        };

        public static readonly Guid[] AllSystemTypesIds =
        {
            HttpUnitTestType.Id,
            PingTestType.Id,
            SqlTestType.Id,
            DomainNameTestType.Id,
            SslTestType.Id
        };

        public static bool IsSystem(Guid unitTestTypeId)
        {
            return AllSystemTypesIds.Contains(unitTestTypeId);
        }

        public static bool CanEditPeriod(Guid unitTestTypeId)
        {
            return unitTestTypeId != DomainNameTestType.Id && unitTestTypeId != SslTestType.Id;
        }
    }
}
