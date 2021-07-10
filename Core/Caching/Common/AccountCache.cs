namespace Zidium.Core.Caching
{
    // TODO Rename class
    public class AccountCache
    {
        private AccountCacheRepository<ComponentCacheResponse, IComponentCacheReadObject, ComponentCacheWriteObject> _components;

        public AccountCacheRepository<ComponentCacheResponse, IComponentCacheReadObject, ComponentCacheWriteObject> Components
        {
            get
            {
                if (_components == null)
                {
                    _components = new AccountCacheRepository<ComponentCacheResponse, IComponentCacheReadObject, ComponentCacheWriteObject>(
                        AllCaches.Components);
                }
                return _components;
            }
        }

        private AccountCacheRepository<MetricCacheResponse, IMetricCacheReadObject, MetricCacheWriteObject> _metrics;

        public AccountCacheRepository<MetricCacheResponse, IMetricCacheReadObject, MetricCacheWriteObject> Metrics
        {
            get
            {
                if (_metrics == null)
                {
                    _metrics = new AccountCacheRepository<MetricCacheResponse, IMetricCacheReadObject, MetricCacheWriteObject>(
                        AllCaches.Metrics);
                }
                return _metrics;
            }
        }

        private AccountCacheRepository<UnitTestCacheResponse, IUnitTestCacheReadObject, UnitTestCacheWriteObject> _unitTests;

        public AccountCacheRepository<UnitTestCacheResponse, IUnitTestCacheReadObject, UnitTestCacheWriteObject> UnitTests
        {
            get
            {
                if (_unitTests == null)
                {
                    _unitTests = new AccountCacheRepository<UnitTestCacheResponse, IUnitTestCacheReadObject, UnitTestCacheWriteObject>(
                        AllCaches.UnitTests);
                }
                return _unitTests;
            }
        }

        private AccountCacheRepository<BulbCacheResponse, IBulbCacheReadObject, BulbCacheWriteObject> _statusDatas;

        public AccountCacheRepository<BulbCacheResponse, IBulbCacheReadObject, BulbCacheWriteObject> StatusDatas
        {
            get
            {
                if (_statusDatas == null)
                {
                    _statusDatas = new AccountCacheRepository<BulbCacheResponse, IBulbCacheReadObject, BulbCacheWriteObject>(
                        AllCaches.StatusDatas);
                }
                return _statusDatas;
            }
        }

        private AccountCacheRepository<EventCacheResponse, IEventCacheReadObject, EventCacheWriteObject> _events;

        public AccountCacheRepository<EventCacheResponse, IEventCacheReadObject, EventCacheWriteObject> Events
        {
            get
            {
                if (_events == null)
                {
                    _events = new AccountCacheRepository<EventCacheResponse, IEventCacheReadObject, EventCacheWriteObject>(
                        AllCaches.Events);
                }
                return _events;
            }
        }

        private AccountCacheRepository<MetricTypeCacheResponse, IMetricTypeCacheReadObject, MetricTypeCacheWriteObject> _metricTypes;

        public AccountCacheRepository<MetricTypeCacheResponse, IMetricTypeCacheReadObject, MetricTypeCacheWriteObject> MetricTypes
        {
            get
            {
                if (_metricTypes == null)
                {
                    _metricTypes = new AccountCacheRepository<MetricTypeCacheResponse, IMetricTypeCacheReadObject, MetricTypeCacheWriteObject>(
                        AllCaches.MetricTypes);
                }
                return _metricTypes;
            }
        }

        private AccountCacheRepository<UnitTestTypeCacheResponse, IUnitTestTypeCacheReadObject, UnitTestTypeCacheWriteObject> _unitTestTypes;

        public AccountCacheRepository<UnitTestTypeCacheResponse, IUnitTestTypeCacheReadObject, UnitTestTypeCacheWriteObject> UnitTestTypes
        {
            get
            {
                if (_unitTestTypes == null)
                {
                    _unitTestTypes = new AccountCacheRepository<UnitTestTypeCacheResponse, IUnitTestTypeCacheReadObject, UnitTestTypeCacheWriteObject>(
                        AllCaches.UnitTestTypes);
                }
                return _unitTestTypes;
            }
        }

    }
}
