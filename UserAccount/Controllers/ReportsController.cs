using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ReportsController : BaseController
    {
        /// <summary>
        /// Статистика запусков
        /// </summary>
        public ActionResult Starts(Guid? componentTypeId = null, Guid? componentId = null, string fromDate = null, string toDate = null)
        {
            var sDate = !string.IsNullOrEmpty(fromDate) ? DecodeDateTimeParameter(fromDate) : DateTime.Now.Date;
            var eDate = !string.IsNullOrEmpty(toDate) ? DecodeDateTimeParameter(toDate) : DateTime.Now;

            ComponentForRead component = null;
            if (componentId.HasValue)
            {
                component = GetStorage().Components.GetOneById(componentId.Value);
            }

            ComponentTypeForRead componentType = null;
            if (componentTypeId.HasValue)
            {
                componentType = GetStorage().ComponentTypes.GetOneById(componentTypeId.Value);
            }

            EventTypeForRead startEventType = null;
            var items = new List<StartsReportItemModel>();
            var graph = new List<StartsReportGrapthItemModel>();
            string error = null;

            if (componentType != null)
            {
                ComponentForRead[] components;
                if (component != null)
                    components = new[] { component };
                else
                    components = GetStorage().Components.GetByComponentTypeId(componentType.Id);

                var componentsIds = components.Select(t => t.Id).ToArray();

                startEventType = GetStorage().EventTypes.GetOneOrNullBySystemName(SystemEventType.ComponentStart);
                if (startEventType != null)
                {
                    var startEventTypeId = startEventType.Id;
                    var listLockObject = new object();

                    // Все события запуска за указанный интервал
                    var allEvents = GetStorage().Events.Filter(componentsIds, startEventTypeId, sDate, eDate);

                    // События запуска по компонентам
                    var startEventsQuery =
                        allEvents.Select(t => new
                        {
                            Id = t.Id,
                            StartDate = t.StartDate,
                            ComponentId = t.OwnerId,
                            Count = t.Count
                        });

                    var startEvents = startEventsQuery.GroupBy(t => t.ComponentId).ToArray();

                    var list =
                        startEvents.Join(components, a => a.Key, b => b.Id, (a, b) => new { Component = b, Events = a })
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
                    var statsEventsQuery = allEvents.Select(t => new
                    {
                        StartDate = t.StartDate,
                        Count = t.Count
                    });
                    var stats =
                        statsEventsQuery
                            .GroupBy(t => t.StartDate.Date)
                            .Select(t => new StartsReportGrapthItemModel()
                            {
                                Date = t.Key,
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
                ComponentId = componentId,
                ComponentTypeId = componentTypeId,
                FromDate = sDate,
                ToDate = eDate,
                Items = items.OrderBy(t => t.ComponentDisplayName).ToList(),
                Graph = graph.OrderBy(t => t.Date).ToList(),
                Error = error,
                StartEventTypeId = startEventType != null ? startEventType.Id : (Guid?)null,
                Total = items.Sum(t => t.Count)
            };
            return View(model);
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        internal ReportsController(Guid userId) : base(userId) { }

        public ReportsController(ILogger<ReportsController> logger) : base(logger)
        {
        }
    }
}