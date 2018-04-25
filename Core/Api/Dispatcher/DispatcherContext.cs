using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;

namespace Zidium.Core.DispatcherLayer
{
    /// <summary>
    /// Данный класс отвечает за создание и закрытие всех ресурсов (контекстов EF)
    /// </summary>
    public class DispatcherContext : IDisposable
    {
        private DatabasesContext _dbContext;

        public IEventService EventService { get; protected set; }

        public IBulbService BulbService { get; protected set; }

        public IComponentService ComponentService { get; protected set; }

        public IEventTypeService EventTypeService { get; protected set; }

        public IComponentTypeService ComponentTypeService { get; protected set; }

        public ILogService LogService { get; protected set; }

        public IMetricService MetricService { get; protected set; }

        public IUnitTestTypeService UnitTestTypeService { get; protected set; }

        public IUnitTestService UnitTestService { get; protected set; }

        public ISubscriptionService SubscriptionService { get; protected set; }

        public IPaymentService PaymentService { get; protected set; }

        public IUserService UserService { get; protected set; }

        public IAccountService AccountService { get; }

        public IAccountManagementService AccountManagementService { get; }

        internal IDatabaseService DatabaseService { get; }

        public DatabasesContext DbContext
        {
            get { return _dbContext; }
        }

        protected DispatcherContext(string configDbConnectionString = null)
        {
            _dbContext = new DatabasesContext
            {
                IsInternalDispatcherContext = true,
                ConfigDbConnectionString = configDbConnectionString
            };

            EventService = new EventService(this);
            EventTypeService = new EventTypeService(this);
            BulbService = new BulbService(this);
            ComponentTypeService = new ComponentTypeService(this);
            LogService = new LogService(this);
            MetricService = new MetricService(this);
            UnitTestTypeService = new UnitTestTypeService(this);
            UnitTestService = new UnitTestService(this);
            SubscriptionService = new SubscriptionService(DbContext);
            PaymentService = ConfigDbServicesHelper.GetPaymentService(DbContext);
            ComponentService = new ComponentService(this);
            UserService = new UserService(DbContext);
            AccountService = ConfigDbServicesHelper.GetAccountService();
            AccountManagementService = ConfigDbServicesHelper.GetAccountManagementService(this);
            DatabaseService = ConfigDbServicesHelper.GetDatabaseService();
        }

        public AccountDbContext GetAccountDbContext(Guid accountId)
        {
            return DbContext.GetAccountDbContext(accountId);
        }

        public Component GetRoot(Guid accontId)
        {
            var accountDbContext = GetAccountDbContext(accontId);
            var repository = accountDbContext.GetComponentRepository();
            var root = repository.GetRoot();
            return root;
        }

        public Component GetComponent(Guid accountId, Guid componentId)
        {
            return GetAccountDbContext(accountId).GetComponentRepository().GetById(componentId);
        }

        public void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
                _dbContext = null;
            }
        }

        public static DispatcherContext Create(string configDbConnectionString = null)
        {
            return new DispatcherContext(configDbConnectionString);
        }

        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }

        static DispatcherContext()
        {
            AllCaches.Init();
        }
    }
}
