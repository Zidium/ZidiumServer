using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Zidium.Core.AccountsDb.Classes;
using Zidium.Core.AccountsDb.Classes.UnitTests.HttpTests;
using Zidium.Core.AccountsDb.Mapping;
using Zidium.Core.AccountsDb.Repositories;
using Zidium.Core.AccountsDb.Services.AccountSettings;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;

namespace Zidium.Core.AccountsDb
{
    public abstract class AccountDbContext : MyDataContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Configurations.Add(new ComponentMapping());
            modelBuilder.Configurations.Add(new ComponentTypeMapping());
            modelBuilder.Configurations.Add(new MetricTypeMapping());
            modelBuilder.Configurations.Add(new EventTypeMapping());
            modelBuilder.Configurations.Add(new TariffLimitMapping());
            modelBuilder.Configurations.Add(new UserMapping());
            modelBuilder.Configurations.Add(new UserContactMapping());
            modelBuilder.Configurations.Add(new SubscriptionMapping());
            modelBuilder.Configurations.Add(new RoleMapping());
            modelBuilder.Configurations.Add(new UnitTestMapping());
            modelBuilder.Configurations.Add(new UnitTestTypeMapping());
            modelBuilder.Configurations.Add(new LogConfigMapping());
            modelBuilder.Configurations.Add(new StatusDataMapping());
            modelBuilder.Configurations.Add(new HttpRequestUnitTestMapping());
            modelBuilder.Configurations.Add(new HttpRequestUnitTestRuleMapping());
            modelBuilder.Configurations.Add(new UserSettingMapping());
            modelBuilder.Configurations.Add(new AccountSettingMapping());
            modelBuilder.Configurations.Add(new HttpRequestUnitTestRuleDataMapping());
            modelBuilder.Configurations.Add(new UnitTestPropertyMapping());
            modelBuilder.Configurations.Add(new UnitTestPingRuleMapping());
            modelBuilder.Configurations.Add(new UnitTestVirusTotalRuleMapping());
            modelBuilder.Configurations.Add(new UnitTestTcpPortRuleMapping());
            modelBuilder.Configurations.Add(new UnitTestSqlRuleMapping());
            modelBuilder.Configurations.Add(new UnitTestDomainNamePaymentPeriodRuleMapping());
            modelBuilder.Configurations.Add(new UnitTestSslCertificateExpirationDateRuleMapping());
            modelBuilder.Configurations.Add(new AccountTariffMapping());
            modelBuilder.Configurations.Add(new LimitDataMapping());
            modelBuilder.Configurations.Add(new LimitDataForUnitTestMapping());
            modelBuilder.Configurations.Add(new MetricMapping());
            modelBuilder.Configurations.Add(new UserRoleMapping());
            modelBuilder.Configurations.Add(new LastComponentNotificationMapping());
            modelBuilder.Configurations.Add(new DefectMapping());
            modelBuilder.Configurations.Add(new DefectChangeMapping());
            modelBuilder.Configurations.Add(new EventMapping());
            modelBuilder.Configurations.Add(new EventParameterMapping());
            modelBuilder.Configurations.Add(new NotificationMapping());
            modelBuilder.Configurations.Add(new MetricHistoryMapping());
            modelBuilder.Configurations.Add(new SendEmailCommandMapping());
            modelBuilder.Configurations.Add(new SendSmsCommandMapping());
            modelBuilder.Configurations.Add(new SendMessageCommandMapping());
            modelBuilder.Configurations.Add(new LogMapping());
            modelBuilder.Configurations.Add(new LogParameterMapping());
            modelBuilder.Configurations.Add(new NotificationHttpMapping());
            modelBuilder.Configurations.Add(new ArchivedStatusMapping());
            modelBuilder.Configurations.Add(new TokenMapping());
            modelBuilder.Configurations.Add(new TimeZoneMapping());
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        /// <summary>
        /// Ограничения по тарифам
        /// </summary>
        public DbSet<TariffLimit> TariffLimits { get; set; }

        /// <summary>
        /// Компоненты
        /// </summary>
        public DbSet<Component> Components { get; set; }

        /// <summary>
        /// Последние уведомления о статусе компонента
        /// </summary>
        public DbSet<LastComponentNotification> LastComponentNotifications { get; set; }

        /// <summary>
        /// Типы компонентов
        /// </summary>
        public DbSet<ComponentType> ComponentTypes { get; set; }

        /// <summary>
        /// Типы событий
        /// </summary>
        public DbSet<EventType> EventTypes { get; set; }

        /// <summary>
        /// Типы метрик
        /// </summary>
        public DbSet<MetricType> MetricTypes { get; set; }

        /// <summary>
        /// Пользователи
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Контакты пользователей
        /// </summary>
        public DbSet<UserContact> UserContacts { get; set; }

        /// <summary>
        /// Подписки
        /// </summary>
        public DbSet<Subscription> Subscriptions { get; set; }

        /// <summary>
        /// Роли пользователей
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Роли пользователей
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Расширенные свойства компонентов
        /// </summary>
        public DbSet<ComponentProperty> ComponentProperties { get; set; }

        /// <summary>
        /// Настройки лога
        /// </summary>
        public DbSet<LogConfig> LogConfigs { get; set; }

        /// <summary>
        /// Типы юнит-тестов
        /// </summary>
        public DbSet<UnitTestType> UnitTestTypes { get; set; }

        /// <summary>
        /// Юнит-тесты
        /// </summary>
        public DbSet<UnitTest> UnitTests { get; set; }

        /// <summary>
        /// Правила http-юнит-тестов
        /// </summary>
        public DbSet<HttpRequestUnitTestRule> HttpRequestUnitTestRules { get; set; }

        /// <summary>
        /// Правила http-юнит-тестов
        /// </summary>
        public DbSet<HttpRequestUnitTestRuleData> HttpRequestUnitTestRuleDatas { get; set; }

        /// <summary>
        /// Состояния компонентов
        /// </summary>
        public DbSet<Bulb> Bulbs { get; set; }

        /// <summary>
        /// Настройки пользователей
        /// </summary>
        public DbSet<UserSetting> UserSettings { get; set; }

        /// <summary>
        /// Настройки аккаунта
        /// </summary>
        public DbSet<AccountSetting> AccountSettings { get; set; }

        /// <summary>
        /// Тарифы аккаунтов
        /// </summary>
        public DbSet<AccountTariff> AccountTariffs { get; set; }

        /// <summary>
        /// Статистика по лимитам
        /// </summary>
        public DbSet<LimitData> LimitDatas { get; set; }

        /// <summary>
        /// Детализация статистики лимитов по проверкам
        /// </summary>
        public DbSet<LimitDataForUnitTest> LimitDatasForUnitTests { get; set; }

        /// <summary>
        /// Метрики
        /// </summary>
        public DbSet<Metric> Metrics { get; set; }

        public DbSet<Defect> Defects { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventProperty> EventProperties { get; set; }

        public DbSet<MetricHistory> MetricHistories { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<LogProperty> LogProperties { get; set; }

        public DbSet<SendEmailCommand> SendEmailCommands { get; set; }

        public DbSet<SendSmsCommand> SendSmsCommands { get; set; }

        public DbSet<SendMessageCommand> SendMessageCommands { get; set; }

        /// <summary>
        /// Уведомления
        /// </summary>
        public DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// Старые статусы, по которым нужно проверить уведомления
        /// </summary>
        public DbSet<ArchivedStatus> ArchivedStatuses { get; set; }

        /// <summary>
        /// Токены на любые одноразовые действия
        /// </summary>
        public DbSet<Token> Tokens { get; set; }

        /// <summary>
        /// Часовые пояса
        /// </summary>
        public DbSet<TimeZone> TimeZones { get; set; }

        // Skokov: без этой строки не работает построитель миграций, не удаляйте её!
        protected AccountDbContext()
            : base("AccountDbContext")
        {
            IncActiveCount();
        }

        protected AccountDbContext(DbConnection connection)
            : base(connection, true)
        {
            IncActiveCount();
        }

        public static AccountDbContext CreateFromConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }
            var provider = Provider.Current();
            return provider.DbContext(connectionString);
        }

        public static AccountDbContext CreateFromDatabaseId(Guid databaseId)
        {
            var client = DispatcherHelper.GetDispatcherClient();
            var database = client.GetDatabaseById(new GetDatabaseByIdRequestData() { Id = databaseId }).Data;
            var context = CreateFromConnectionString(database.ConnectionString);
            context.DatabaseName = database.SystemName;
            return context;
        }

        public static AccountDbContext CreateFromAccountId(Guid accountId)
        {
            var client = DispatcherHelper.GetDispatcherClient();
            var account = client.GetAccountById(new GetAccountByIdRequestData() { Id = accountId }).Data;
            var database = client.GetDatabaseById(new GetDatabaseByIdRequestData() { Id = account.AccountDatabaseId }).Data;
            var context = CreateFromConnectionString(database.ConnectionString);
            context.DatabaseName = database.SystemName;
            return context;
        }

        public static AccountDbContext CreateFromAccountIdLocalCache(Guid accountId)
        {
            var account = ConfigDbServicesHelper.GetAccountService().GetOneById(accountId);
            var database = ConfigDbServicesHelper.GetDatabaseService().GetOneById(account.AccountDatabaseId);
            var context = CreateFromConnectionString(database.ConnectionString);
            context.DatabaseName = database.SystemName;
            return context;
        }

        public IComponentTypeRepository GetComponentTypeRepository()
        {
            return new ComponentTypeRepository(this);
        }

        public IComponentRepository GetComponentRepository()
        {
            return new ComponentRepository(this);
        }

        public IUserRepository GetUserRepository()
        {
            return new UserRepository(this);
        }

        public IRoleRepository GetUserRoleRepository()
        {
            return new RoleRepository(this);
        }

        public IMetricTypeRepository GetMetricTypeRepository()
        {
            return new MetricTypeRepository(this);
        }

        public IMetricRepository GetMetricRepository()
        {
            return new MetricRepository(this);
        }

        public IEventTypeRepository GetEventTypeRepository()
        {
            return new EventTypeRepository(this);
        }

        public ISubscriptionRepository GetSubscriptionRepository()
        {
            return new SubscriptionRepository(this);
        }

        public ILogConfigRepository GetLogConfigRepository()
        {
            return new LogConfigRepository(this);
        }

        public IUnitTestTypeRepository GetUnitTestTypeRepository()
        {
            return new UnitTestTypeRepository(this);
        }

        public IUnitTestRepository GetUnitTestRepository()
        {
            return new UnitTestRepository(this);
        }

        public IComponentPropertyRepository GetComponentPropertyRepository()
        {
            return new ComponentPropertyRepository(this);
        }

        public IHttpRequestUnitTestRuleRepository GetHttpRequestUnitTestRuleRepository()
        {
            return new HttpRequestUnitTestRuleRepository(this);
        }

        public IUserSettingRepository GetUserSettingRepository()
        {
            return new UserSettingRepository(this);
        }

        public IUserSettingService GetUserSettingService()
        {
            return new UserSettingService(this);
        }

        public IAccountSettingRepository GetAccountSettingRepository()
        {
            return new AccountSettingRepository(this);
        }

        public IAccountSettingService GetAccountSettingService()
        {
            return new AccountSettingService(this);
        }

        public ITariffLimitRepository GetTariffLimitRepository()
        {
            return new TariffLimitRepository(this);
        }

        public IAccountTariffRepository GetAccountTariffRepository()
        {
            return new AccountTariffRepository(this);
        }

        public ILimitDataRepository GetLimitDataRepository()
        {
            return new LimitDataRepository(this);
        }

        public ILimitDataForUnitTestRepository GetLimitDataForUnitTestRepository()
        {
            return new LimitDataForUnitTestRepository(this);
        }

        public IStatusDataRepository GetStatusDataRepository()
        {
            return new StatusDataRepository(this);
        }

        public IDefectRepository GetDefectRepository()
        {
            return new DefectRepository(this);
        }

        public IDefectService GetDefectService()
        {
            return new DefectService(this);
        }

        /// <summary>
        /// Возвращает репозиторий работы с данными метрик
        /// </summary>
        /// <returns></returns>
        public IMetricHistoryRepository GetMetricHistoryRepository()
        {
            return new MetricHistoryRepository(this);
        }

        public IEventRepository GetEventRepository()
        {
            return new EventRepository(this);
        }

        public INotificationRepository GetNotificationRepository()
        {
            return new NotificationRepository(this);
        }

        /// <summary>
        /// Возвращает репозиторий для работы с командами отправки email
        /// </summary>
        /// <returns></returns>
        public ISendEmailCommandRepository GetSendEmailCommandRepository()
        {
            return new SendEmailCommandRepository(this);
        }

        public ILogRepository GetLogRepository()
        {
            return new LogRepository(this);
        }

        public IArchivedStatusRepository GetArchivedStatusRepository()
        {
            return new ArchivedStatusRepository(this);
        }

        public ISendSmsCommandRepository GetSendSmsCommandRepository()
        {
            return new SendSmsCommandRepository(this);
        }

        public ITokenRepository GetTokenRepository()
        {
            return new TokenRepository(this);
        }

        public ISendMessageCommandRepository GetSendMessageCommandRepository()
        {
            return new SendMessageCommandRepository(this);
        }

        public ITimeZoneRepository GetTimeZoneRepository()
        {
            return new TimeZoneRepository(this);
        }

        /// <summary>
        /// Проверка доступности базы
        /// </summary>
        public override void Check()
        {
            Database.ExecuteSqlCommand("SELECT NULL;");
        }

        public abstract AccountDbContext Clone();

        public static void DisableMigrations()
        {
            Database.SetInitializer(new NullDatabaseInitializer<MsSqlAccountDbContext>());
            Database.SetInitializer(new NullDatabaseInitializer<MySqlAccountDbContext>());
            Database.SetInitializer(new NullDatabaseInitializer<PostgreSqlAccountDbContext>());
        }

        #region Статистика использования

        public static object LockObject = new object();

        public static int ActiveCount;

        public static int MaxActiveCount;

        protected void IncActiveCount()
        {
            lock (LockObject)
            {
                ActiveCount++;
                if (ActiveCount > MaxActiveCount)
                    MaxActiveCount = ActiveCount;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            lock (LockObject)
            {
                ActiveCount--;
            }
        }

        #endregion
    }

}
