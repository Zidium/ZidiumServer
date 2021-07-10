using System;

namespace Zidium.Core.Common
{
    /// <summary>
    /// Класс возвращает объект блокировки по хэшу строкового имени блокировки.
    /// Хэш имени блокировки используется, чтобы ограничить количество хранимых объектов блокировки.
    /// Т.е. теоретически 2 разных имени объекта блокровки могут использовать один и тот же объект блокировки,
    /// но вероятность этого мала = 1 из 10 000.
    /// </summary>
    public class LockObject
    {
        private static readonly int _locksCount = 10000;

        private readonly object[] _locks;

        public LockObject(int size)
        {
            _locks = new object[size];
            for (int i = 0; i < size; i++)
            {
                _locks[i] = new object();
            }
        }

        private static readonly LockObject _accountLocks = new LockObject(_locksCount);
        private static readonly LockObject _componentLocks = new LockObject(_locksCount);
        private static readonly LockObject _eventTypeLocks = new LockObject(_locksCount);
        private static readonly LockObject _eventLocks = new LockObject(_locksCount);
        private static readonly LockObject _metricLocks = new LockObject(_locksCount);
        private static readonly LockObject _metricTypeLocks = new LockObject(_locksCount);
        private static readonly LockObject _unitTestLocks = new LockObject(_locksCount);
        private static readonly LockObject _joinEventLocks = new LockObject(_locksCount);
        private static readonly LockObject _unitTestTypesLocks = new LockObject(_locksCount);
        private static readonly LockObject _subscriptionLocks = new LockObject(_locksCount);
        private static readonly LockObject _unitTestLimitDataLocks = new LockObject(_locksCount);

        public static object ForComponent(string systemName)
        {
            return _componentLocks.Get(systemName.ToLowerInvariant());
        }

        public static object ForComponent(Guid componentId)
        {
            return _componentLocks.Get(componentId);
        }

        // TODO Refactor to simple object
        public static object ForAccount()
        {
            return _accountLocks.Get("Account");
        }

        /// <summary>
        /// Возвращает объект блокировки для типа события
        /// </summary>
        public static object ForEventType(string systemName)
        {
            var key = systemName;
            return _eventTypeLocks.Get(key);
        }

        /// <summary>
        /// Возвращает объект блокировки для события
        /// </summary>
        public static object ForProcessSimpleEvent(Guid componentId, Guid eventTypeId, long joinKey)
        {
            return _eventLocks.Get(componentId.ToString() + eventTypeId + joinKey);
        }

        /// <summary>
        /// Возвращает объект блокировки для метрики
        /// </summary>
        public static object ForMetric(Guid id)
        {
            return _metricLocks.Get(id);
        }

        /// <summary>
        /// Возвращает объект блокировки для типа метрики
        /// </summary>
        public static object ForMetricType(Guid accountId, string name)
        {
            return _metricTypeLocks.Get(accountId + name);
        }

        public static object ForUnitTest(Guid unitTestId)
        {
            return _unitTestLocks.Get(unitTestId);
        }

        public static object ForUnitTestType(Guid unitTestTypeId)
        {
            return _unitTestTypesLocks.Get(unitTestTypeId);
        }

        public static object ForUnitTest(Guid componentId, string systemName)
        {
            return _unitTestLocks.Get(componentId + systemName);
        }

        public static object ForJoinEvent(Guid ownerId)
        {
            return _joinEventLocks.Get(ownerId);
        }

        public static object ForSubscription(Guid subscriptionId)
        {
            return _subscriptionLocks.Get(subscriptionId);
        }

        public static object ForUnitTestLimitData(Guid limitDataId, Guid unitTestId)
        {
            return _unitTestLimitDataLocks.Get(unitTestId.ToString() + unitTestId.ToString());
        }

        /// <summary>
        /// Возвращает блокировку для произвольного объекта, описанного строкой
        /// </summary>
        public object Get(string key)
        {
            int hash = HashHelper.GetInt32(key);
            int index = Math.Abs(hash % _locks.Length);
            return _locks[index];
        }

        public object Get(Guid id)
        {
            int hash = id.GetHashCode();
            int index = Math.Abs(hash % _locks.Length);
            return _locks[index];
        }

        public object Get(int value)
        {
            int index = Math.Abs(value % _locks.Length);
            return _locks[index];
        }
    }
}
