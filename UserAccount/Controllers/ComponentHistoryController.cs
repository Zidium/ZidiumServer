﻿using System;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.ComponentHistory;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ComponentHistoryController : BaseController
    {
        public ComponentHistoryController(ILogger<ComponentHistoryController> logger) : base(logger)
        {
        }

        public ActionResult Index(TimelineInterval? interval)
        {
            var storage = GetStorage();
            var userSettingService = new UserSettingService(storage);
            var savedInterval = (TimelineInterval?)userSettingService.ComponentHistoryInterval(CurrentUser.Id) ?? TimelineInterval.Hour;

            if (interval != null)
            {
                userSettingService.ComponentHistoryInterval(CurrentUser.Id, (int)interval.Value);
            }

            var model = new IndexModel()
            {
                Interval = interval ?? savedInterval
            };

            model.ToDate = Now();
            model.FromDate = TimelineHelper.IntervalToStartDate(model.ToDate, model.Interval);

            return View(model);
        }

        public PartialViewResult GetComponentsTree(DateTime from, DateTime to)
        {
            var expandedItems = GetExpandedItemsFromCookie();
            var components = GetSimplifiedComponentList();
            var root = components.Single(t => t.Parent == null);

            var model = GetTreeItemModel(root, expandedItems, from, to);

            return PartialView("ComponentsTreeElement", model);
        }

        public PartialViewResult GetComponentsTreeItemContent(Guid id, DateTime from, DateTime to)
        {
            var expandedItems = GetExpandedItemsFromCookie();
            var components = GetSimplifiedComponentList();
            var component = components.Single(t => t.Id == id);

            var model = GetTreeItemContentModel(component, expandedItems, from, to);
            model.Details = GetTreeItemDetailsModel(component, expandedItems, from, to);

            return PartialView("ComponentsTreeElementContent", model);
        }

        public PartialViewResult GetComponentsTreeItemDetails(Guid id, DateTime from, DateTime to)
        {
            var expandedItems = GetExpandedItemsFromCookie();
            var components = GetSimplifiedComponentList();
            var component = components.Single(t => t.Id == id);

            var model = GetTreeItemDetailsModel(component, expandedItems, from, to);

            return PartialView("ComponentsTreeElementDetails", model);
        }

        protected string[] GetExpandedItemsFromCookie()
        {
            var cookie = Request.Cookies[IndexModel.ExpandedItemsCookieName];
            var value = cookie != null ? HttpUtility.UrlDecode(cookie) : null;

            if (string.IsNullOrEmpty(value))
                return new string[0];

            try
            {
                var items = new JsonSerializer().GetObject<object[]>(value);
                var result = items.Select(t => (string)t).ToArray();
                return result;
            }
            catch
            {
                return new string[0];
            }
        }

        protected SimplifiedComponent[] GetSimplifiedComponentList()
        {
            // Одним запросом получаем все нужные данные
            // Это быстрее, чем загружать данные компонентов по необходимости

            var list = GetStorage().Gui.GetComponentHistory()
                .Select(t => new SimplifiedComponent()
                {
                    Id = t.Id,
                    DisplayName = t.DisplayName,
                    SystemName = t.SystemName,
                    ParentId = t.ParentId,
                    ComponentTypeId = t.ComponentTypeId,

                    HasEvents = t.HasEvents,

                    Unittests = t.UnitTests.Select(x => new SimplifiedUnittest()
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName
                    }).ToArray(),

                    Metrics = t.Metrics.Select(x => new SimplifiedMetric()
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName
                    }).ToArray()
                }).ToList();

            // Проставим ссылки на родителя и детей
            foreach (var component in list.ToArray())
            {
                if (component.ParentId.HasValue)
                {
                    var parent = list.FirstOrDefault(t => t.Id == component.ParentId.Value);
                    if (parent != null)
                    {
                        component.Parent = list.First(t => t.Id == component.ParentId.Value);
                        component.Parent.Childs.Add(component);
                    }
                    else
                    {
                        // Если не нашли родителя, значит, он удалён, выбросим компонент из дерева
                        // Вообще такого не должно быть, потому что компонент удаляется вместе с детьми
                        // Но иногда всё равно бывает
                        list.Remove(component);
                    }
                }
            }

            var result = list.ToArray();

            return result;
        }

        protected ComponentsTreeItemModel GetTreeItemModel(
            SimplifiedComponent component,
            string[] expandedItems,
            DateTime from,
            DateTime to)
        {
            var service = new EventService(GetStorage(), TimeService);
            var states = service.GetTimelineStates(component.Id, EventCategory.ComponentExternalStatus, from, to);
            var okTime = service.GetTimelineOkTime(states, from, to);
            var items = this.GetTimelineItemsByStates(states, from, to);

            var result = new ComponentsTreeItemModel()
            {
                Id = component.Id,
                ParentId = component.ParentId,
                ComponentTypeId = component.ComponentTypeId,
                DisplayName = component.DisplayName,
                SystemName = component.SystemName,
                Path = component.Path,
                From = from,
                To = to,
                OkTime = okTime,
                Timeline = new TimelineModel()
                {
                    Category = EventCategory.ComponentExternalStatus,
                    OwnerId = component.Id,
                    DateFrom = from,
                    DateTo = to,
                    Items = items,
                    OkTime = okTime
                }
            };

            // Изначально развёрнут только корень дерева
            var expanded = result.IsRoot;

            if (expandedItems.Contains(result.Id.ToString()))
                expanded = true;

            result.Expanded = expanded;

            // Если узел раскрыт, то загрузим содержимое
            if (result.Expanded)
            {
                if (result.Content == null)
                {
                    result.Content = GetTreeItemContentModel(component, expandedItems, from, to);
                }

                if (result.Content.Details == null)
                {
                    result.Content.Details = GetTreeItemDetailsModel(component, expandedItems, from, to);
                }
            }

            return result;
        }

        protected ComponentsTreeItemContentModel GetTreeItemContentModel(
            SimplifiedComponent component,
            string[] expandedItems,
            DateTime from,
            DateTime to)
        {
            var result = new ComponentsTreeItemContentModel()
            {
                Childs = component.Childs
                    .Select(t => GetTreeItemModel(t, expandedItems, from, to))
                    .Where(t => t != null)
                    .OrderBy(t => t.OkTime)
                    .ToArray()
            };
            return result;
        }

        protected ComponentsTreeItemDetailsModel GetTreeItemDetailsModel(
            SimplifiedComponent component,
            string[] expandedItems,
            DateTime from,
            DateTime to)
        {
            var service = new EventService(GetStorage(), TimeService);

            var eventsStates = service.GetTimelineStates(component.Id, EventCategory.ComponentEventsStatus, from, to);
            var eventsOkTime = service.GetTimelineOkTime(eventsStates, from, to);
            var eventsItems = this.GetTimelineItemsByStates(eventsStates, from, to);

            var unittestsStates = service.GetTimelineStates(component.Id, EventCategory.ComponentUnitTestsStatus, from, to);
            var unittestsOkTime = service.GetTimelineOkTime(unittestsStates, from, to);
            var unittestsItems = this.GetTimelineItemsByStates(unittestsStates, from, to);

            var metricsStates = service.GetTimelineStates(component.Id, EventCategory.ComponentMetricsStatus, from, to);
            var metricsOkTime = service.GetTimelineOkTime(metricsStates, from, to);
            var metricsItems = this.GetTimelineItemsByStates(metricsStates, from, to);

            var result = new ComponentsTreeItemDetailsModel()
            {
                Id = component.Id,
                HasEvents = component.HasEvents,
                EventsOkTime = eventsOkTime,
                EventsTimeline = new TimelineModel()
                {
                    Category = EventCategory.ComponentEventsStatus,
                    OwnerId = component.Id,
                    DateFrom = from,
                    DateTo = to,
                    Items = eventsItems,
                    OkTime = eventsOkTime
                },
                From = from,
                To = to,
                Path = component.Path,

                Unittests = new ComponentsTreeItemUnittestsDetailsModel()
                {
                    Id = component.Id,
                    Items = component.Unittests
                        .Select(t => new ComponentsTreeItemUnittestsDetailsItemModel()
                        {
                            Id = t.Id,
                            DisplayName = t.DisplayName
                        })
                        .ToArray()
                },
                UnittestsOkTime = unittestsOkTime,
                UnittestsTimeline = new TimelineModel()
                {
                    Category = EventCategory.ComponentUnitTestsStatus,
                    OwnerId = component.Id,
                    DateFrom = from,
                    DateTo = to,
                    Items = unittestsItems,
                    OkTime = unittestsOkTime
                },

                Metrics = new ComponentsTreeItemMetricsDetailsModel()
                {
                    Id = component.Id,
                    Items = component.Metrics
                        .Select(t => new ComponentsTreeItemMetricsDetailsItemModel()
                        {
                            Id = t.Id,
                            DisplayName = t.DisplayName
                        })
                        .ToArray()
                },
                MetricsOkTime = metricsOkTime,
                MetricsTimeline = new TimelineModel()
                {
                    Category = EventCategory.ComponentMetricsStatus,
                    OwnerId = component.Id,
                    DateFrom = from,
                    DateTo = to,
                    Items = metricsItems,
                    OkTime = metricsOkTime
                }
            };

            foreach (var unittest in result.Unittests.Items)
            {
                var unittestStates = service.GetTimelineStates(unittest.Id, EventCategory.UnitTestStatus, from, to);
                unittest.OkTime = service.GetTimelineOkTime(unittestStates, from, to);
                var unittestItems = this.GetTimelineItemsByStates(unittestStates, from, to);
                unittest.Timeline = new TimelineModel()
                {
                    Category = EventCategory.UnitTestStatus,
                    OwnerId = unittest.Id,
                    DateFrom = from,
                    DateTo = to,
                    Items = unittestItems,
                    OkTime = unittest.OkTime
                };
            }
            result.Unittests.Items = result.Unittests.Items.OrderBy(t => t.OkTime).ToArray();

            foreach (var metric in result.Metrics.Items)
            {
                var metricStates = service.GetTimelineStates(metric.Id, EventCategory.MetricStatus, from, to);
                metric.OkTime = service.GetTimelineOkTime(metricStates, from, to);
                var metricItems = this.GetTimelineItemsByStates(metricStates, from, to);
                metric.Timeline = new TimelineModel()
                {
                    Category = EventCategory.MetricStatus,
                    OwnerId = metric.Id,
                    DateFrom = from,
                    DateTo = to,
                    Items = metricItems,
                    OkTime = metric.OkTime
                };
            }
            result.Metrics.Items = result.Metrics.Items.OrderBy(t => t.OkTime).ToArray();

            result.Unittests.Expanded = expandedItems.Contains(result.Unittests.HtmlId);
            result.Metrics.Expanded = expandedItems.Contains(result.Metrics.HtmlId);

            return result;
        }

    }
}