using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Zidium.Storage.Ef.Mapping;

namespace Zidium.Storage.Ef
{
    internal abstract class AccountDbContext : DataContextBase
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Configurations.Add(new ComponentMapping());
            modelBuilder.Configurations.Add(new ComponentPropertyMapping());
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
        public DbSet<DbTariffLimit> TariffLimits { get; set; }

        /// <summary>
        /// Компоненты
        /// </summary>
        public DbSet<DbComponent> Components { get; set; }

        /// <summary>
        /// Последние уведомления о статусе компонента
        /// </summary>
        public DbSet<DbLastComponentNotification> LastComponentNotifications { get; set; }

        /// <summary>
        /// Типы компонентов
        /// </summary>
        public DbSet<DbComponentType> ComponentTypes { get; set; }

        /// <summary>
        /// Типы событий
        /// </summary>
        public DbSet<DbEventType> EventTypes { get; set; }

        /// <summary>
        /// Типы метрик
        /// </summary>
        public DbSet<DbMetricType> MetricTypes { get; set; }

        /// <summary>
        /// Пользователи
        /// </summary>
        public DbSet<DbUser> Users { get; set; }

        /// <summary>
        /// Контакты пользователей
        /// </summary>
        public DbSet<DbUserContact> UserContacts { get; set; }

        /// <summary>
        /// Подписки
        /// </summary>
        public DbSet<DbSubscription> Subscriptions { get; set; }

        /// <summary>
        /// Роли пользователей
        /// </summary>
        public DbSet<DbRole> Roles { get; set; }

        /// <summary>
        /// Роли пользователей
        /// </summary>
        public DbSet<DbUserRole> UserRoles { get; set; }

        /// <summary>
        /// Расширенные свойства компонентов
        /// </summary>
        public DbSet<DbComponentProperty> ComponentProperties { get; set; }

        /// <summary>
        /// Настройки лога
        /// </summary>
        public DbSet<DbLogConfig> LogConfigs { get; set; }

        /// <summary>
        /// Типы проверок
        /// </summary>
        public DbSet<DbUnitTestType> UnitTestTypes { get; set; }

        /// <summary>
        /// Проверки
        /// </summary>
        public DbSet<DbUnitTest> UnitTests { get; set; }

        /// <summary>
        /// Проверки http
        /// </summary>
        public DbSet<DbHttpRequestUnitTest> HttpRequestUnitTests { get; set; }

        /// <summary>
        /// Правила проверок http
        /// </summary>
        public DbSet<DbHttpRequestUnitTestRule> HttpRequestUnitTestRules { get; set; }

        /// <summary>
        /// Данные прави проверок http
        /// </summary>
        public DbSet<DbHttpRequestUnitTestRuleData> HttpRequestUnitTestRuleDatas { get; set; }

        public DbSet<DbUnitTestDomainNamePaymentPeriodRule> UnitTestDomainNamePaymentPeriodRules { get; set; }

        public DbSet<DbUnitTestPingRule> UnitTestPingRules { get; set; }

        public DbSet<DbUnitTestSqlRule> UnitTestSqlRules { get; set; }

        public DbSet<DbUnitTestSslCertificateExpirationDateRule> UnitTestSslCertificateExpirationDateRules { get; set; }

        public DbSet<DbUnitTestTcpPortRule> UnitTestTcpPortRules { get; set; }

        public DbSet<DbUnitTestVirusTotalRule> UnitTestVirusTotalRules { get; set; }

        /// <summary>
        /// Состояния компонентов
        /// </summary>
        public DbSet<DbBulb> Bulbs { get; set; }

        /// <summary>
        /// Настройки пользователей
        /// </summary>
        public DbSet<DbUserSetting> UserSettings { get; set; }

        /// <summary>
        /// Настройки аккаунта
        /// </summary>
        public DbSet<DbAccountSetting> AccountSettings { get; set; }

        /// <summary>
        /// Статистика по лимитам
        /// </summary>
        public DbSet<DbLimitData> LimitDatas { get; set; }

        /// <summary>
        /// Детализация статистики лимитов по проверкам
        /// </summary>
        public DbSet<DbLimitDataForUnitTest> LimitDatasForUnitTests { get; set; }

        /// <summary>
        /// Метрики
        /// </summary>
        public DbSet<DbMetric> Metrics { get; set; }

        public DbSet<DbDefect> Defects { get; set; }

        public DbSet<DbDefectChange> DefectChanges { get; set; }

        public DbSet<DbEvent> Events { get; set; }

        public DbSet<DbEventProperty> EventProperties { get; set; }

        public DbSet<DbMetricHistory> MetricHistories { get; set; }

        public DbSet<DbLog> Logs { get; set; }

        public DbSet<DbLogProperty> LogProperties { get; set; }

        public DbSet<DbSendEmailCommand> SendEmailCommands { get; set; }

        public DbSet<DbSendSmsCommand> SendSmsCommands { get; set; }

        public DbSet<DbSendMessageCommand> SendMessageCommands { get; set; }

        /// <summary>
        /// Уведомления
        /// </summary>
        public DbSet<DbNotification> Notifications { get; set; }

        public DbSet<DbNotificationHttp> NotificationsHttp { get; set; }

        /// <summary>
        /// Старые статусы, по которым нужно проверить уведомления
        /// </summary>
        public DbSet<DbArchivedStatus> ArchivedStatuses { get; set; }

        /// <summary>
        /// Токены на любые одноразовые действия
        /// </summary>
        public DbSet<DbToken> Tokens { get; set; }

        /// <summary>
        /// Часовые пояса
        /// </summary>
        public DbSet<DbTimeZone> TimeZones { get; set; }

        protected AccountDbContext(DbConnection connection) : base(connection, true)
        {
            IncActiveCount();
        }

        protected AccountDbContext(string connectionString) : base(connectionString)
        {
            IncActiveCount();
        }

        public static AccountDbContext CreateFromConnectionString(string connectionString)
        {
            var provider = Provider.Current();
            return provider.DbContext(connectionString);
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
