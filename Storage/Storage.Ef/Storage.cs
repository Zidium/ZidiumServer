using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Zidium.Storage.Ef
{
    internal class Storage : IStorage
    {
        public Storage(string connectionString)
        {
            _connectionString = connectionString;
        }

        private readonly string _connectionString;

        private AccountDbContext CreateContext()
        {
            return AccountDbContext.CreateFromConnectionString(_connectionString);
        }

        private AccountDbContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = CreateContext();
                }
                return _context;
            }
        }

        private AccountDbContext _context;

        private int _usageCount;

        public ContextWrapper GetContextWrapper()
        {
            Interlocked.Increment(ref _usageCount);
            return new ContextWrapper(Context, OnWrapperDispose);
        }

        private void OnWrapperDispose()
        {
            var newCount = Interlocked.Decrement(ref _usageCount);
            if (newCount == 0)
            {
                _context.Dispose();
                _context = null;
            }
        }

        public IAccountSettingRepository AccountSettings
        {
            get { return new AccountSettingRepository(this); }
        }

        public IArchivedStatusRepository ArchivedStatuses
        {
            get { return new ArchivedStatusRepository(this); }
        }

        public IBulbRepository Bulbs
        {
            get { return new BulbRepository(this); }
        }

        public IComponentRepository Components
        {
            get { return new ComponentRepository(this); }
        }

        public IComponentPropertyRepository ComponentProperties
        {
            get { return new ComponentPropertyRepository(this); }
        }

        public IComponentTypeRepository ComponentTypes
        {
            get { return new ComponentTypeRepository(this); }
        }

        public IDefectRepository Defects
        {
            get { return new DefectRepository(this); }
        }

        public IDefectChangeRepository DefectChanges
        {
            get { return new DefectChangeRepository(this); }
        }

        public IEventRepository Events
        {
            get { return new EventRepository(this); }
        }

        public IEventPropertyRepository EventProperties
        {
            get { return new EventPropertyRepository(this); }
        }

        public IEventTypeRepository EventTypes
        {
            get { return new EventTypeRepository(this); }
        }

        public IHttpRequestUnitTestRepository HttpRequestUnitTests
        {
            get { return new HttpRequestUnitTestRepository(this); }
        }

        public IHttpRequestUnitTestRuleRepository HttpRequestUnitTestRules
        {
            get { return new HttpRequestUnitTestRuleRepository(this); }
        }

        public IHttpRequestUnitTestRuleDataRepository HttpRequestUnitTestRuleDatas
        {
            get { return new HttpRequestUnitTestRuleDataRepository(this); }
        }

        public ILastComponentNotificationRepository LastComponentNotifications
        {
            get { return new LastComponentNotificationRepository(this); }
        }

        public ILimitDataRepository LimitData
        {
            get { return new LimitDataRepository(this); }
        }

        public ILimitDataForUnitTestRepository LimitDataForUnitTest
        {
            get { return new LimitDataForUnitTestRepository(this); }
        }

        public ILogRepository Logs
        {
            get { return new LogRepository(this); }
        }

        public ILogConfigRepository LogConfigs
        {
            get { return new LogConfigRepository(this); }
        }

        public ILogPropertyRepository LogProperties
        {
            get { return new LogPropertyRepository(this); }
        }

        public IMetricRepository Metrics
        {
            get { return new MetricRepository(this); }
        }

        public IMetricHistoryRepository MetricHistory
        {
            get { return new MetricHistoryRepository(this); }
        }

        public IMetricTypeRepository MetricTypes
        {
            get { return new MetricTypeRepository(this); }
        }

        public INotificationRepository Notifications
        {
            get { return new NotificationRepository(this); }
        }

        public INotificationHttpRepository NotificationsHttp
        {
            get { return new NotificationHttpRepository(this); }
        }

        public IRoleRepository Roles
        {
            get { return new RoleRepository(this); }
        }

        public ISendEmailCommandRepository SendEmailCommands
        {
            get { return new SendEmailCommandRepository(this); }
        }

        public ISendMessageCommandRepository SendMessageCommands
        {
            get { return new SendMessageCommandRepository(this); }
        }

        public ISendSmsCommandRepository SendSmsCommands
        {
            get { return new SendSmsCommandRepository(this); }
        }

        public ISubscriptionRepository Subscriptions
        {
            get { return new SubscriptionRepository(this); }
        }

        public ITimeZoneRepository TimeZones
        {
            get { return new TimeZoneRepository(this); }
        }

        public ITokenRepository Tokens
        {
            get { return new TokenRepository(this); }
        }

        public IUnitTestPingRuleRepository UnitTestPingRules
        {
            get { return new UnitTestPingRuleRepository(this); }
        }

        public IUnitTestRepository UnitTests
        {
            get { return new UnitTestRepository(this); }
        }

        public IUnitTestTypeRepository UnitTestTypes
        {
            get { return new UnitTestTypeRepository(this); }
        }

        public IUnitTestDomainNamePaymentPeriodRuleRepository DomainNamePaymentPeriodRules
        {
            get { return new UnitTestDomainNamePaymentPeriodRuleRepository(this); }
        }

        public IUnitTestSqlRuleRepository UnitTestSqlRules
        {
            get { return new UnitTestSqlRuleRepository(this); }
        }

        public IUnitTestSslCertificateExpirationDateRuleRepository UnitTestSslCertificateExpirationDateRules
        {
            get { return new UnitTestSslCertificateExpirationDateRuleRepository(this); }
        }

        public IUnitTestTcpPortRuleRepository UnitTestTcpPortRules
        {
            get { return new UnitTestTcpPortRuleRepository(this); }
        }

        public IUnitTestVirusTotalRuleRepository UnitTestVirusTotalRules
        {
            get { return new UnitTestVirusTotalRuleRepository(this); }
        }

        public IUserRepository Users
        {
            get { return new UserRepository(this); }
        }

        public IUserContactRepository UserContacts
        {
            get { return new UserContactRepository(this); }
        }

        public IUserRoleRepository UserRoles
        {
            get { return new UserRoleRepository(this); }
        }

        public IUserSettingRepository UserSettings
        {
            get { return new UserSettingRepository(this); }
        }

        public IGuiRepository Gui
        {
            get { return new GuiRepository(this); }
        }

        public void Check()
        {
            using (var context = CreateContext())
            {
                context.Check();
            }
        }

        public ITransaction BeginTransaction()
        {
            return new Transaction(GetContextWrapper());
        }

        public int Migrate()
        {
            var logger = DependencyInjection.GetLogger<Storage>();
            logger.LogInformation("Checking database...");
            using (var context = CreateContext())
            {
                var count = context.Database.GetPendingMigrations().Count();

                if (count == 0)
                {
                    logger.LogInformation("No pending migrations, database is up to date");
                }
                else
                {
                    logger.LogInformation($"Installing {count} pending migrations...");
                    context.Database.Migrate();
                    logger.LogInformation("Migrations intalled, database is up to date");
                }

                return count;
            }
        }
    }
}
