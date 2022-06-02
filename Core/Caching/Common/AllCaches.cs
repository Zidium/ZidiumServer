using System;
using Zidium.Api;
using Zidium.Core.InternalLogger;

namespace Zidium.Core.Caching
{
    // Refactor to non-static service
    public static class AllCaches
    {
        public static EventCacheStorage Events { get; private set; }

        public static BulbCacheStorage StatusDatas { get; private set; }

        public static UnitTestCacheStorage UnitTests { get; private set; }

        public static ComponentCacheStorage Components { get; private set; }

        public static MetricCacheStorage Metrics { get; private set; }

        public static MetricTypeCacheStorage MetricTypes { get; private set; }

        public static UnitTestTypeCacheStorage UnitTestTypes { get; private set; }

        private static IComponentControl GetCacheControl(IComponentControl control, string name)
        {
            var result = control.GetOrCreateChildComponentControl("Cache", name);
            result.IsFake();
            return result;
        }

        public static void SetControl(IComponentControl control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            var mapping = DependencyInjection.GetServicePersistent<InternalLoggerComponentMapping>();

            Events.ComponentControl = GetCacheControl(control, "EventsCache");
            mapping.MapLoggerToComponent("EventsCache", Events.ComponentControl.Info.Id);
            Events.Logger = DependencyInjection.GetLogger("EventsCache");

            StatusDatas.ComponentControl = GetCacheControl(control, "StatusDatasCache");
            mapping.MapLoggerToComponent("StatusDatasCache", StatusDatas.ComponentControl.Info.Id);
            StatusDatas.Logger = DependencyInjection.GetLogger("StatusDatasCache");

            UnitTests.ComponentControl = GetCacheControl(control, "UnitTestsCache");
            mapping.MapLoggerToComponent("UnitTestsCache", UnitTests.ComponentControl.Info.Id);
            UnitTests.Logger = DependencyInjection.GetLogger("UnitTestsCache");

            Components.ComponentControl = GetCacheControl(control, "ComponentsCache");
            mapping.MapLoggerToComponent("ComponentsCache", Components.ComponentControl.Info.Id);
            Components.Logger = DependencyInjection.GetLogger("ComponentsCache");
        }

        public static void Init()
        {
            if (Events == null)
            {
                Events = new EventCacheStorage();
                Events.MaxCount = 300 * 1000;
                Events.BeginUnloadCount = 200 * 1000;
                Events.StopUnloadCount = 100 * 1000;

                StatusDatas = new BulbCacheStorage();
                StatusDatas.MaxCount = 300 * 1000;
                StatusDatas.BeginUnloadCount = 200 * 1000;
                StatusDatas.StopUnloadCount = 100 * 1000;

                UnitTests = new UnitTestCacheStorage();
                UnitTests.MaxCount = 300 * 1000;
                UnitTests.BeginUnloadCount = 200 * 1000;
                UnitTests.StopUnloadCount = 100 * 1000;

                Components = new ComponentCacheStorage();
                Components.MaxCount = 300 * 1000;
                Components.BeginUnloadCount = 200 * 1000;
                Components.StopUnloadCount = 100 * 1000;

                Metrics = new MetricCacheStorage();
                Metrics.MaxCount = 300 * 1000;
                Metrics.BeginUnloadCount = 200 * 1000;
                Metrics.StopUnloadCount = 100 * 1000;

                MetricTypes = new MetricTypeCacheStorage();
                MetricTypes.MaxCount = 300 * 1000;
                MetricTypes.BeginUnloadCount = 200 * 1000;
                MetricTypes.StopUnloadCount = 100 * 1000;

                UnitTestTypes = new UnitTestTypeCacheStorage();
                UnitTestTypes.MaxCount = 300 * 1000;
                UnitTestTypes.BeginUnloadCount = 200 * 1000;
                UnitTestTypes.StopUnloadCount = 100 * 1000;
            }
        }

        internal static void SaveChanges()
        {
            foreach (var cacheStorage in All)
            {
                cacheStorage.SaveChanges();
            }
        }

        public static ICacheStorage[] All
        {
            get
            {
                return new ICacheStorage[]
                {
                    Events,
                    StatusDatas,
                    UnitTests,
                    Components,
                    Metrics,
                    MetricTypes,
                    UnitTestTypes
                };
            }
        }

        public static void Stop()
        {
            foreach (var cache in All)
            {
                cache.Stop();
            }
        }

        static AllCaches()
        {
            Init();
        }
    }
}
