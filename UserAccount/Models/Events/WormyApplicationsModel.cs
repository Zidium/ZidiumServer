using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.Events
{
    public class WormyApplicationsModel
    {
        public class ApplicationData
        {
            public ComponentForRead Component { get; set; }

            public long Errors { get; set; }
        }

        public int Top { get; set; }

        public ReportPeriodRange PeriodRange { get; set; }

        public Guid? ComponentTypeId { get; set; }

        public ApplicationData[] Applications { get; set; }

        public IStorage Storage { get; set; }

        // TODO move to controller
        public void Load(IStorage storage)
        {
            Storage = storage;

            Guid[] componentIds = null;
            if (ComponentTypeId.HasValue)
            {
                componentIds = storage.Components.GetByComponentTypeId(ComponentTypeId.Value).Select(x => x.Id).ToArray();
            }

            var eventQuery = storage.Events.GetErrorsByPeriod(componentIds, PeriodRange.From, PeriodRange.To);

            var events = eventQuery.GroupBy(x => x.OwnerId)
                .Select(x => new
                {
                    ComponentId = x.Key,
                    Errors = x.Sum(y => y.Count)
                })
                .OrderByDescending(x => x.Errors)
                .Take(Top)
                .ToArray();

            var components = storage.Components.GetMany(events.Select(t => t.ComponentId).Distinct().ToArray()).ToDictionary(a => a.Id, b => b);

            var appList = new List<ApplicationData>();
            foreach (var eventObj in events)
            {
                var component = components[eventObj.ComponentId];
                if (component.IsDeleted)
                {
                    continue;
                }
                var appData = new ApplicationData()
                {
                    Errors = eventObj.Errors,
                    Component = component
                };
                appList.Add(appData);
            }
            Applications = appList.ToArray();
        }
    }
}