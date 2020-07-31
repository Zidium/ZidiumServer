using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Справочник системных типов юнит-тестов
    /// </summary>
    public class SystemUnitTestType
    {
        public Guid Id;

        public string SystemName;

        public string DisplayName;

        public static readonly SystemUnitTestType HttpUnitTestType = new SystemUnitTestType()
        {
            Id = new Guid("44F601AD-2946-1578-9904-517A6D3C6D64"),
            DisplayName = "Http",
            SystemName = "System.UnitTests.Http"
        };

        public static readonly SystemUnitTestType PingTestType = new SystemUnitTestType()
        {
            Id = new Guid("C4B764F0-AF19-42B0-8089-1D01FC4CAD40"),
            DisplayName = "Ping",
            SystemName = "System.UnitTests.Ping"
        };

        public static readonly SystemUnitTestType TcpPortTestType = new SystemUnitTestType()
        {
            Id = new Guid("159E359D-BD27-4F72-BA76-CA8410EBF5B5"),
            DisplayName = "TcpPort",
            SystemName = "System.UnitTests.TcpPort"
        };

        public static readonly SystemUnitTestType SqlTestType = new SystemUnitTestType()
        {
            Id = new Guid("CB35631D-8C1F-491C-A2C9-243641D72FBF"),
            DisplayName = "SQL",
            SystemName = "System.UnitTests.Sql"
        };

        public static readonly SystemUnitTestType DomainNameTestType = new SystemUnitTestType()
        {
            Id = new Guid("73A92501-0B33-426F-986B-4E3C34CF44B2"),
            DisplayName = "Domain",
            SystemName = "System.UnitTests.DomainName"
        };

        public static readonly SystemUnitTestType SslTestType = new SystemUnitTestType()
        {
            Id = new Guid("8B314C56-12B9-44CE-8F20-38C767EE379A"),
            DisplayName = "SSL",
            SystemName = "System.UnitTests.Ssl"
        };

        public static readonly SystemUnitTestType VirusTotalTestType = new SystemUnitTestType()
        {
            Id = new Guid("87F2A4E6-9D23-4724-AA1F-0517DB060D03"),
            DisplayName = "VirusTotal",
            SystemName = "System.UnitTests.VirusTotal"
        };

        public static readonly Guid[] AllSystemTypesIds =
        {
            HttpUnitTestType.Id,
            PingTestType.Id,
            TcpPortTestType.Id,
            SqlTestType.Id,
            DomainNameTestType.Id,
            SslTestType.Id,
            VirusTotalTestType.Id
        };

        public static bool IsSystem(Guid unitTestTypeId)
        {
            return AllSystemTypesIds.Contains(unitTestTypeId);
        }

        public static bool IsCustom(Guid unitTestTypeId)
        {
            return !IsSystem(unitTestTypeId);
        }

        public static bool CanEditPeriod(Guid unitTestTypeId)
        {
            return unitTestTypeId != DomainNameTestType.Id && unitTestTypeId != SslTestType.Id;
        }
    }
}
