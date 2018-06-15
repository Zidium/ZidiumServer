using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Класс заполняет в БД аккаунтов все необходимые справочники
    /// </summary>
    public class AccountDbDataRegistator
    {
        protected AccountDbContext Context { get; set; }

        public AccountDbDataRegistator(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        protected void RegisterComponentType(List<ComponentType> typesFromDb, ComponentType componentType)
        {
            var typeFromDb = typesFromDb.SingleOrDefault(x => x.Id == componentType.Id);
            if (typeFromDb == null)
            {
                Context.ComponentTypes.Add(componentType);
            }
            else
            {
                if (typeFromDb.SystemName != componentType.SystemName
                    || typeFromDb.DisplayName != componentType.DisplayName)
                {
                    typeFromDb.DisplayName = componentType.DisplayName;
                    typeFromDb.SystemName = componentType.SystemName;
                }
            }
        }

        /// <summary>
        /// Регистрирует в БД справочник системных типов компонентов
        /// </summary>
        protected void RegisterComponentTypes()
        {
            var typesFromDb = Context.ComponentTypes.Where(x => x.IsSystem).ToList();

            RegisterComponentType(typesFromDb, SystemComponentTypes.Root);
            RegisterComponentType(typesFromDb, SystemComponentTypes.Folder);
            RegisterComponentType(typesFromDb, SystemComponentTypes.WebSite);
            RegisterComponentType(typesFromDb, SystemComponentTypes.Others);
            RegisterComponentType(typesFromDb, SystemComponentTypes.DataBase);
            RegisterComponentType(typesFromDb, SystemComponentTypes.Domain);
        }

        /// <summary>
        /// Регистрируем в БД системные типы событий
        /// </summary>
        protected void RegisterEventTypes()
        {
            var eventTypeRepository = new EventTypeRepository(Context);

            var types = new List<SystemEventType>();

            types.Add(SystemEventType.ComponentEventsStatus);
            types.Add(SystemEventType.ComponentUnitTestsStatus);
            types.Add(SystemEventType.ComponentMetricsStatus);
            types.Add(SystemEventType.ComponentChildsStatus);

            types.Add(SystemEventType.ComponentInternalStatus);
            types.Add(SystemEventType.ComponentExternalStatus);

            types.Add(SystemEventType.UnitTestStatus);
            types.Add(SystemEventType.UnitTestResult);

            types.Add(SystemEventType.MetricStatus);

            foreach (var type in types)
            {
                var typeObj = type.GetEventType();
                var fromDb = eventTypeRepository.GetOrCreate(typeObj);
                fromDb.DisplayName = typeObj.DisplayName;
            }
        }

        protected void RegisterUserRoles()
        {
            var repository = new RoleRepository(Context);

            // Администраторы аккаунта
            var accountAdminRole = new Role()
            {
                Id = RoleId.AccountAdministrators,
                SystemName = "AccountAdministrators",
                DisplayName = "Администраторы аккаунта"
            };
            repository.GetOrCreateOne(accountAdminRole);

            // Пользователи
            var userRole = new Role()
            {
                Id = RoleId.Users,
                SystemName = "Users",
                DisplayName = "Пользователи"
            };
            repository.GetOrCreateOne(userRole);

            // Наблюдатели
            var viewerRole = new Role()
            {
                Id = RoleId.Viewers,
                SystemName = "Viewers",
                DisplayName = "Наблюдатели"
            };
            repository.GetOrCreateOne(viewerRole);
        }

        protected void RegisterUnitTestType(UnitTestType unitTestType)
        {
            var fromDb = Context.UnitTestTypes.SingleOrDefault(x => x.Id == unitTestType.Id);
            if (fromDb == null)
            {
                Context.UnitTestTypes.Add(unitTestType);
                Context.SaveChanges();
            }
            else
            {
                fromDb.SystemName = unitTestType.SystemName;
                fromDb.DisplayName = unitTestType.DisplayName;
                fromDb.IsSystem = unitTestType.IsSystem;
                Context.SaveChanges();
            }
        }

        protected void RegisterUnitTestTypes()
        {
            // Http
            RegisterUnitTestType(SystemUnitTestTypes.HttpUnitTestType);

            // Ping
            RegisterUnitTestType(SystemUnitTestTypes.PingTestType);

            // Sql
            RegisterUnitTestType(SystemUnitTestTypes.SqlTestType);

            // срок оплаты доменного имени
            RegisterUnitTestType(SystemUnitTestTypes.DomainNameTestType);

            // срок годности ssl-сертификата
            RegisterUnitTestType(SystemUnitTestTypes.SslTestType);

        }

        protected void RegisterTariffLimit(TariffLimit limit)
        {
            var limitRepository = Context.GetTariffLimitRepository();
            var entity = limitRepository.GetByIdOrNull(limit.Id);
            if (entity == null)
            {
                limitRepository.Add(limit);
            }
            else
            {
                // Ради эксперимента - обновление записи через detach & attach
                // Так можно не переприсваивать все поля
                // Работает!
                Context.Entry(entity).State = System.Data.Entity.EntityState.Detached;
                Context.TariffLimits.Attach(limit);
                Context.Entry(limit).State = System.Data.Entity.EntityState.Modified;
                limitRepository.Update(limit);
            }
        }

        protected void RegisterTariffLimits()
        {
            RegisterTariffLimit(SystemTariffLimits.Friend);
            RegisterTariffLimit(SystemTariffLimits.TestBonus);
        }

        public void RegisterAll()
        {
            RegisterComponentTypes();
            RegisterEventTypes();
            RegisterUserRoles();
            RegisterUnitTestTypes();
            RegisterTariffLimits();

            Context.SaveChanges();
        }
    }
}
