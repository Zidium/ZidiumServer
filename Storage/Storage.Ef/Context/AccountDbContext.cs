﻿using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Zidium.Storage.Ef
{
    internal abstract class AccountDbContext : DataContextBase
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountDbContext).Assembly);

            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

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

        // For migrations
        protected AccountDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public static AccountDbContext CreateFromConnectionString(string connectionString)
        {
            var provider = Provider.Current();
            return provider.DbContext(connectionString);
        }

        // For unittests
        internal static AccountDbContext CreateFromConnectionString(string connectionString, Action<DbContextOptionsBuilder> optionsBuilderAction)
        {
            var result = CreateFromConnectionString(connectionString);
            result._optionsBuilderAction = optionsBuilderAction;
            return result;
        }

        private Action<DbContextOptionsBuilder> _optionsBuilderAction;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (_optionsBuilderAction != null)
                _optionsBuilderAction.Invoke(optionsBuilder);
        }

        /// <summary>
        /// Проверка доступности базы
        /// </summary>
        public override void Check()
        {
            Database.ExecuteSqlRaw("SELECT NULL;");
        }

        public abstract AccountDbContext Clone();

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

        public override void Dispose()
        {
            base.Dispose();
            lock (LockObject)
            {
                ActiveCount--;
            }
        }

        #endregion

    }

}