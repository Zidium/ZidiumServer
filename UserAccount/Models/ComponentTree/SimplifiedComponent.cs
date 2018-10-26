using System;
using System.Collections.Generic;

namespace Zidium.UserAccount.Models.ComponentTree
{
    public class SimplifiedComponent
    {
        public Guid Id;

        public string DisplayName;

        public string SystemName;

        public SimplifiedStatusData ExternalStatus;

        public SimplifiedStatusData EventsStatus;

        public SimplifiedStatusData UnitTestsStatus;

        public SimplifiedStatusData MetricsStatus;

        public Guid ComponentTypeId;

        public Guid? ParentId;

        public SimplifiedComponent Parent;

        public List<SimplifiedComponent> Childs;

        public IEnumerable<SimplifiedUnittest> Unittests;

        public IEnumerable<SimplifiedMetric> Metrics;

        public bool ItemModelInternalDataLoaded;

        public SimplifiedComponent()
        {
            Childs = new List<SimplifiedComponent>();
        }
    }
}