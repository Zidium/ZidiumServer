using Microsoft.Extensions.Logging;

namespace Zidium.Storage
{
    public interface IStorage
    {
        IAccountSettingRepository AccountSettings { get; }

        IArchivedStatusRepository ArchivedStatuses { get; }

        IBulbRepository Bulbs { get; }

        IComponentRepository Components { get; }

        IComponentPropertyRepository ComponentProperties { get; }

        IComponentTypeRepository ComponentTypes { get; }

        IDefectRepository Defects { get; }

        IDefectChangeRepository DefectChanges { get; }

        IEventRepository Events { get; }

        IEventPropertyRepository EventProperties { get; }

        IEventTypeRepository EventTypes { get; }

        IHttpRequestUnitTestRepository HttpRequestUnitTests { get; }

        IHttpRequestUnitTestRuleRepository HttpRequestUnitTestRules { get; }

        IHttpRequestUnitTestRuleDataRepository HttpRequestUnitTestRuleDatas { get; }

        ILastComponentNotificationRepository LastComponentNotifications { get; }

        ILimitDataRepository LimitData { get; }

        ILimitDataForUnitTestRepository LimitDataForUnitTest { get; }

        ILogRepository Logs { get; }

        ILogConfigRepository LogConfigs { get; }

        ILogPropertyRepository LogProperties { get; }

        IMetricRepository Metrics { get; }

        IMetricHistoryRepository MetricHistory { get; }

        IMetricTypeRepository MetricTypes { get; }

        INotificationRepository Notifications { get; }

        INotificationHttpRepository NotificationsHttp { get; }

        IRoleRepository Roles { get; }

        ISendEmailCommandRepository SendEmailCommands { get; }

        ISendMessageCommandRepository SendMessageCommands { get; }

        ISendSmsCommandRepository SendSmsCommands { get; }

        ISubscriptionRepository Subscriptions { get; }

        ITimeZoneRepository TimeZones { get; }

        ITokenRepository Tokens { get; }

        IUnitTestPingRuleRepository UnitTestPingRules { get; }

        IUnitTestRepository UnitTests { get; }

        IUnitTestTypeRepository UnitTestTypes { get; }

        IUnitTestDomainNamePaymentPeriodRuleRepository DomainNamePaymentPeriodRules { get; }

        IUnitTestSqlRuleRepository UnitTestSqlRules { get; }

        IUnitTestSslCertificateExpirationDateRuleRepository UnitTestSslCertificateExpirationDateRules { get; }

        IUnitTestTcpPortRuleRepository UnitTestTcpPortRules { get; }

        IUnitTestVirusTotalRuleRepository UnitTestVirusTotalRules { get; }

        IUserRepository Users { get; }

        IUserContactRepository UserContacts { get; }

        IUserRoleRepository UserRoles { get; }

        IUserSettingRepository UserSettings { get; }

        IGuiRepository Gui { get; }

        void Check();

        ITransaction BeginTransaction();

        int Migrate();

    }
}
