using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zidium.Api.Dto;
using Zidium.Core.Api;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.ComponentHistory;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ComponentHistoryController : ContextController
    {
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Index(TimelineInterval? interval)
        {
            var userSettingService = CurrentAccountDbContext.GetUserSettingService();
            var savedInterval = (TimelineInterval?) userSettingService.ComponentHistoryInterval(CurrentUser.Id) ?? TimelineInterval.Hour;

            if (interval != null)
            {
                userSettingService.ComponentHistoryInterval(CurrentUser.Id, (int) interval.Value);
                DbContext.SaveChanges();
            }

            var model = new IndexModel()
            {
                Interval = interval ?? savedInterval
            };

            model.ToDate = MvcApplication.GetServerDateTime();
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
            var value = cookie != null ? HttpUtility.UrlDecode(cookie.Value) : null;

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

            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            var allComponents = componentRepository.QueryAll();

            var query = allComponents
                .Include("EventsStatus")
                .Select(t => new SimplifiedComponent()
                {
                    Id = t.Id,
                    DisplayName = t.DisplayName,
                    SystemName = t.SystemName,
                    ParentId = t.ParentId,
                    ComponentTypeId = t.ComponentTypeId,

                    HasEvents = t.EventsStatus.FirstEventId.HasValue,

                    Unittests = t.UnitTests.Where(a => a.IsDeleted == false).Select(x => new SimplifiedUnittest()
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName
                    }),

                    Metrics = t.Metrics.Where(a => a.IsDeleted == false && a.MetricType.IsDeleted == false).Select(x => new SimplifiedMetric()
                    {
                        Id = x.Id,
                        DisplayName = x.MetricType.DisplayName
                    })
                });

            var list = query.ToList();

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
            var eventRepository = CurrentAccountDbContext.GetEventRepository();
            var states = eventRepository.GetTimelineStates(component.Id, EventCategory.ComponentExternalStatus, from, to);
            var okTime = eventRepository.GetTimelineOkTime(states, from, to);
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
            var eventRepository = CurrentAccountDbContext.GetEventRepository();

            var eventsStates = eventRepository.GetTimelineStates(component.Id, EventCategory.ComponentEventsStatus, from, to);
            var eventsOkTime = eventRepository.GetTimelineOkTime(eventsStates, from, to);
            var eventsItems = this.GetTimelineItemsByStates(eventsStates, from, to);

            var unittestsStates = eventRepository.GetTimelineStates(component.Id, EventCategory.ComponentUnitTestsStatus, from, to);
            var unittestsOkTime = eventRepository.GetTimelineOkTime(unittestsStates, from, to);
            var unittestsItems = this.GetTimelineItemsByStates(unittestsStates, from, to);

            var metricsStates = eventRepository.GetTimelineStates(component.Id, EventCategory.ComponentMetricsStatus, from, to);
            var metricsOkTime = eventRepository.GetTimelineOkTime(metricsStates, from, to);
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
                var unittestStates = eventRepository.GetTimelineStates(unittest.Id, EventCategory.UnitTestStatus, from, to);
                unittest.OkTime = eventRepository.GetTimelineOkTime(unittestStates, from, to);
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
                var metricStates = eventRepository.GetTimelineStates(metric.Id, EventCategory.MetricStatus, from, to);
                metric.OkTime = eventRepository.GetTimelineOkTime(metricStates, from, to);
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