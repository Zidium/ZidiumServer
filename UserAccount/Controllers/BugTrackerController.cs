using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class BugTrackerController : ContextController
    {
        public ActionResult Index(Guid? componentId = null)
        {
            return RedirectToAction("Index", "Problems");

            //var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            //Guid[] components;
            //if (componentId.HasValue)
            //    components = new Guid[] { componentId.Value };
            //else
            //{
            //    components = componentRepository.QueryAll(CurrentUser.AccountId)
            //        .Where(t => t.IsDeleted == false && t.ComponentTypeId != SystemComponentTypes.Root.Id && t.ComponentTypeId != SystemComponentTypes.Folder.Id)
            //        .Select(t => t.Id).ToArray();
            //}
            //var allComponents = componentRepository.QueryAll(CurrentUser.AccountId);

            //var eventTypeRepository = CurrentAccountDbContext.GetEventTypeRepository();
            //var events = new List<BugTrackerEventWithVersionModel>();
            //var listLockObject = new Object();
            
            //        var eventRepository = data.StorageDbContext.GetEventRepository();
            //        var eventsList = eventRepository.QueryAllByComponentId(components)
            //            .Where(t => t.Category == EventCategory.ApplicationError && t.IsUserHandled == false)
            //            .Select(t => new BugTrackerEventWithVersionModel()
            //            {
            //                Id = t.Id,
            //                ComponentId = t.OwnerId,
            //                EventTypeId = t.EventTypeId,
            //                StartDate = t.StartDate,
            //                Version = t.Version,
            //                VersionLong = t.VersionLong ?? 0
            //            }).ToArray();

            //        lock (listLockObject)
            //        {
            //            events.AddRange(eventsList);
            //        }
                

            //var result = events
            //    .Join(allComponents, a => a.ComponentId, b => b.Id, (x, y) => new { Event = x, Component = y})
            //    .GroupBy(t => t.Event.EventTypeId)
            //    .Select(t => new
            //    {
            //        EventTypeId = t.Key, 
            //        Count = t.Count(),
            //        Events = t.OrderByDescending(m => m.Event.VersionLong).ThenByDescending(m => m.Event.StartDate).Select(m => new BugTrackerEventsInnerModel()
            //        {
            //            Id = m.Event.Id,
            //            ComponentId = m.Component.Id,
            //            StartDate = m.Event.StartDate, 
            //            Component = m.Component.DisplayName,
            //            Version = m.Event.Version
            //        }).ToArray()
            //    })
            //    .Join(eventTypeRepository.QueryAll(CurrentUser.AccountId), a => a.EventTypeId, b => b.Id, (x, y) => new BugTrackerEventModel()
            //    {
            //        EventType = y,
            //        Count = x.Count,
            //        InnerEvents = x.Events
            //    })
            //    .OrderByDescending(t => t.Count);

            //var model = new BugTrackerModel()
            //{
            //    ComponentId = componentId,
            //    Events = result.ToList()
            //};
            //return View(model);
        }

        protected class BugTrackerEventWithVersionModel
        {
            public Guid Id;
            public Guid ComponentId;
            public Guid EventTypeId;
            public DateTime StartDate;
            public string Version;
            public long VersionLong;
        }
    }
}