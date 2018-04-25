using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ReportsController : ContextController
    {
        /// <summary>
        /// Статистика запусков
        /// </summary>
        public ActionResult Starts(Guid? componentTypeId = null, Guid? componentId = null, string fromDate = null, string toDate = null)
        {
            var sDate = !string.IsNullOrEmpty(fromDate) ? DecodeDateTimeParameter(fromDate) : (DateTime?)null;
            var eDate = !string.IsNullOrEmpty(toDate) ? DecodeDateTimeParameter(toDate) : (DateTime?)null;

            Component component = null;
            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            if (componentId.HasValue)
            {
                component = componentRepository.GetById(componentId.Value);
            }

            ComponentType componentType = null;
            var componentTypeRepository = CurrentAccountDbContext.GetComponentTypeRepository();
            if (componentTypeId.HasValue)
            {
                componentType = componentTypeRepository.GetById(componentTypeId.Value);
            }
            else
                componentType = componentTypeRepository.QueryAll().Where(t => t.IsDeleted == false).OrderBy(t => t.DisplayName).FirstOrDefault();

            EventType startEventType = null;
            Component[] components;
            var items = new List<StartsReportItemModel>();
            var graph = new List<StartsReportGrapthItemModel>();
            string error = null;

            if (componentType != null)
            {
                if (component != null)
                    components = new[] {component};
                else
                    components =
                        componentRepository.QueryAll()
                            .Where(t => t.ComponentTypeId == componentType.Id && t.IsDeleted == false)
                            .ToArray();

                var componentsIds = components.Select(t => t.Id).ToArray();

                var eventTypeRepository = CurrentAccountDbContext.GetEventTypeRepository();
                startEventType = eventTypeRepository.GetOneOrNullBySystemName(SystemEventType.ComponentStart);
                if (startEventType != null)
                {
                    var startEventTypeId = startEventType.Id;
                    var listLockObject = new Object();

                    var eventRepository = CurrentAccountDbContext.GetEventRepository();

                    // Все события запуска за указанный интервал
                    var allEventsQuery = eventRepository.QueryAllByComponentId(componentsIds)
                        .Where(t => t.EventTypeId == startEventTypeId);
                    if (sDate.HasValue)
                        allEventsQuery = allEventsQuery.Where(t => t.StartDate >= sDate.Value);
                    if (eDate.HasValue)
                        allEventsQuery = allEventsQuery.Where(t => t.StartDate <= eDate.Value);

                    // События запуска по компонентам
                    var startEventsQuery =
                        allEventsQuery.Select(
                            t => new {Id = t.Id, StartDate = t.StartDate, ComponentId = t.OwnerId, Count = t.Count});
                    var startEvents = startEventsQuery.GroupBy(t => t.ComponentId).ToArray();

                    var list =
                        startEvents.Join(components, a => a.Key, b => b.Id, (a, b) => new {Component = b, Events = a})
                            .Select(t => new StartsReportItemModel()
                            {
                                ComponentId = t.Component.Id,
                                ComponentDisplayName = t.Component.DisplayName,
                                ComponentSystemName = t.Component.SystemName,
                                CreateDate = t.Component.CreatedDate,
                                Version = t.Component.Version,
                                FirstStart =
                                    t.Events.OrderBy(x => x.StartDate).Select(x => x.StartDate).FirstOrDefault(),
                                FirstStartId = t.Events.OrderBy(x => x.StartDate).Select(x => x.Id).FirstOrDefault(),
                                LastStart =
                                    t.Events.OrderByDescending(x => x.StartDate)
                                        .Select(x => x.StartDate)
                                        .FirstOrDefault(),
                                LastStartId =
                                    t.Events.OrderByDescending(x => x.StartDate).Select(x => x.Id).FirstOrDefault(),
                                Count = t.Events.Sum(x => x.Count)
                            }).ToList();

                    // События запуска по датам
                    var statsEventsQuery = allEventsQuery.Select(t => new {StartDate = t.StartDate, Count = t.Count});
                    var stats =
                        statsEventsQuery.GroupBy(t => DbFunctions.TruncateTime(t.StartDate))
                            .Select(t => new StartsReportGrapthItemModel()
                            {
                                Date = t.Key.Value,
                                Count = t.Sum(x => x.Count)
                            }).ToList();

                    lock (listLockObject)
                    {
                        items.AddRange(list);
                        graph.AddRange(stats);
                    }

                }
                else
                    error = "У выбранного типа компонента нет типа события с системным именем '" +
                            SystemEventType.ComponentStart + "'";
            }

            var model = new StartsReportModel()
            {
                AccountId = CurrentUser.AccountId,
                ComponentId = componentId,
                ComponentTypeId = componentTypeId,
                FromDate = sDate,
                ToDate = eDate,
                Items = items.OrderBy(t => t.ComponentDisplayName).ToList(),
                Graph = graph.OrderBy(t => t.Date).ToList(),
                Error = error,
                StartEventTypeId = startEventType != null ? startEventType.Id : (Guid?) null,
                Total = items.Sum(t => t.Count)
            };
            return View(model);
        }




        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        public ReportsController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public ReportsController() { }

    }
}