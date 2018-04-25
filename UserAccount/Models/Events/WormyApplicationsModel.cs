using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;

namespace Zidium.UserAccount.Models.Events
{
    public class WormyApplicationsModel
    {
        public class ApplicationData
        {
            public Component Component { get; set; }

            public long Errors { get; set; }
        }

        public int Top { get; set; }

        public ReportPeriodRange PeriodRange { get; set; }

        public Guid? ComponentTypeId { get; set; }

        public ApplicationData[] Applications { get; set; }

        public void Load(Guid accountId, AccountDbContext accountDbContext)
        {
            var eventQuery = accountDbContext.GetEventRepository()
                .GetErrorsByPeriod(PeriodRange.From, PeriodRange.To);

            if (ComponentTypeId.HasValue)
            {
                var componentArrayId = accountDbContext.GetComponentRepository().QueryAll()
                    .Where(x => x.ComponentTypeId == ComponentTypeId.Value)
                    .Select(x => x.Id)
                    .ToArray();

                eventQuery = eventQuery.Where(x => componentArrayId.Any(id => id == x.OwnerId));
            }
            var events = eventQuery.GroupBy(x => x.OwnerId)
                .Select(x => new
                {
                    componentId = x.Key,
                    errors = x.Sum(y => y.Count)
                })
                .OrderByDescending(x=>x.errors)
                .Take(Top)
                .ToArray();

            var appList = new List<ApplicationData>();
            foreach (var @event in events)
            {
                var component = accountDbContext.GetComponentRepository().GetById(@event.componentId);
                if (component.IsDeleted)
                {
                    continue;
                }
                var appData = new ApplicationData()
                {
                    Errors = @event.errors,
                    Component = component
                };
                appList.Add(appData);
            }
            Applications = appList.ToArray();
        }
    }
}