using System;
using System.Linq;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Класс заполняет в БД аккаунтов все необходимые справочники
    /// </summary>
    public class AccountDbDataRegistator
    {
        private readonly IStorage _storage;

        public AccountDbDataRegistator(IStorage storage)
        {
            _storage = storage;
        }

        public void RegisterAll()
        {
            RegisterComponentTypes();
            RegisterEventTypes();
            RegisterUserRoles();
            RegisterUnitTestTypes();
            RegisterTariffLimits();
            RegisterTimeZones();
        }

        /// <summary>
        /// Регистрирует в БД справочник системных типов компонентов
        /// </summary>
        private void RegisterComponentTypes()
        {
            RegisterComponentType(SystemComponentType.Root);
            RegisterComponentType(SystemComponentType.Folder);
            RegisterComponentType(SystemComponentType.WebSite);
            RegisterComponentType(SystemComponentType.Others);
            RegisterComponentType(SystemComponentType.Database);
            RegisterComponentType(SystemComponentType.Domain);
        }

        private void RegisterComponentType(SystemComponentType componentType)
        {
            var existingComponentType = _storage.ComponentTypes.GetOneOrNullById(componentType.Id);

            if (existingComponentType == null)
            {
                _storage.ComponentTypes.Add(new ComponentTypeForAdd()
                {
                    Id = componentType.Id,
                    SystemName = componentType.SystemName,
                    DisplayName = componentType.DisplayName,
                    IsSystem = true
                });
            }
        }

        /// <summary>
        /// Регистрируем в БД системные типы событий
        /// </summary>
        private void RegisterEventTypes()
        {
            RegisterEventType(SystemEventType.ComponentEventsStatus);
            RegisterEventType(SystemEventType.ComponentUnitTestsStatus);
            RegisterEventType(SystemEventType.ComponentMetricsStatus);
            RegisterEventType(SystemEventType.ComponentChildsStatus);
            RegisterEventType(SystemEventType.ComponentInternalStatus);
            RegisterEventType(SystemEventType.ComponentExternalStatus);
            RegisterEventType(SystemEventType.UnitTestStatus);
            RegisterEventType(SystemEventType.UnitTestResult);
            RegisterEventType(SystemEventType.MetricStatus);
        }

        private void RegisterEventType(SystemEventType eventType)
        {
            var existingEventType = _storage.EventTypes.GetOneOrNullById(eventType.Id);

            if (existingEventType == null)
            {
                _storage.EventTypes.Add(new EventTypeForAdd()
                {
                    Id = eventType.Id,
                    Category = eventType.Category,
                    SystemName = eventType.SystemName,
                    DisplayName = eventType.DisplayName,
                    JoinIntervalSeconds = (int?)eventType.JoinInterval?.TotalSeconds,
                    ImportanceForNew = eventType.Importance
                });
            }
        }

        private void RegisterUserRoles()
        {
            RegisterRole(SystemRole.AccountAdministrators);
            RegisterRole(SystemRole.Users);
            RegisterRole(SystemRole.Viewers);
        }

        private void RegisterRole(SystemRole role)
        {
            var existingRole = _storage.Roles.GetOneOrNullById(role.Id);

            if (existingRole == null)
            {
                _storage.Roles.Add(new RoleForAdd()
                {
                    Id = role.Id,
                    SystemName = role.SystemName,
                    DisplayName = role.DisplayName
                });
            }
        }

        private void RegisterUnitTestTypes()
        {
            // Http
            RegisterUnitTestType(SystemUnitTestType.HttpUnitTestType);

            // Ping
            RegisterUnitTestType(SystemUnitTestType.PingTestType);

            // TcpPort
            RegisterUnitTestType(SystemUnitTestType.TcpPortTestType);

            // Sql
            RegisterUnitTestType(SystemUnitTestType.SqlTestType);

            // срок оплаты доменного имени
            RegisterUnitTestType(SystemUnitTestType.DomainNameTestType);

            // срок годности ssl-сертификата
            RegisterUnitTestType(SystemUnitTestType.SslTestType);

            // VirusTotal
            RegisterUnitTestType(SystemUnitTestType.VirusTotalTestType);
        }

        private void RegisterUnitTestType(SystemUnitTestType unitTestType)
        {
            var existingUnitTestType = _storage.UnitTestTypes.GetOneOrNullById(unitTestType.Id);

            if (existingUnitTestType == null)
            {
                _storage.UnitTestTypes.Add(new UnitTestTypeForAdd()
                {
                    Id = unitTestType.Id,
                    SystemName = unitTestType.SystemName,
                    DisplayName = unitTestType.DisplayName,
                    IsSystem = true
                });
            }
        }

        private void RegisterTariffLimits()
        {
            RegisterTariffLimit(SystemTariffLimit.Friend);
            RegisterTariffLimit(SystemTariffLimit.TestBonus);
        }

        private void RegisterTariffLimit(TariffLimitForAdd limit)
        {
            var existingLimit = _storage.TariffLimits.GetOneOrNullById(limit.Id);

            if (existingLimit == null)
            {
                _storage.TariffLimits.Add(limit);
            }
        }

        private void RegisterTimeZones()
        {
            if (!_storage.TimeZones.GetAll().Any())
            {
                var offsets = TimeZoneInfo.GetSystemTimeZones()
                    .Select(t => t.BaseUtcOffset)
                    .Distinct();
                foreach (var offset in offsets)
                {
                    var timezone = new TimeZoneForAdd()
                    {
                        OffsetMinutes = (int)offset.TotalMinutes,
                        Name = "UTC " + (offset < TimeSpan.Zero ? "-" : "+") + offset.ToString(@"hh\:mm")
                    };

                    _storage.TimeZones.Add(timezone);
                }
            }
        }

    }
}
