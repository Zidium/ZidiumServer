using System;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models.ComponentTree;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ComponentTreeController : BaseController
    {
        public ActionResult Index(
            ColorStatusSelectorValue color,
            Guid? componentTypeId = null,
            string search = null,
            string save = null)
        {
            if (save == "1")
            {
                var service = new UserSettingService(GetStorage());
                service.ShowComponentsAsList(CurrentUser.Id, false);
                return RedirectToAction("Index", new { color = color.HasValue ? color : null, componentTypeId, search });
            }

            var model = new ComponentsTreeModel()
            {
                ComponentTypeId = componentTypeId,
                Color = color,
                Search = search
            };

            return View(model);
        }

        public PartialViewResult GetComponentsTree(
            ColorStatusSelectorValue color,
            Guid? componentTypeId = null,
            string search = null)
        {
            var expandedItems = GetExpandedItemsFromCookie();
            var now = Now();
            var statuses = color.GetSelectedMonitoringStatuses();
            var components = GetSimplifiedComponentList();
            var root = components.Single(t => t.Parent == null);

            var model = GetTreeItemModel(statuses, componentTypeId, search, root, now, expandedItems);

            return PartialView("ComponentsTreeElement", model);
        }

        public PartialViewResult GetComponentsTreeItemContent(Guid id)
        {
            var expandedItems = GetExpandedItemsFromCookie();
            var now = Now();
            var components = GetSimplifiedComponentList();
            var component = components.Single(t => t.Id == id);

            var model = GetTreeItemContentModel(null, null, null, component, now, expandedItems);
            model.Details = GetTreeItemDetailsModel(component, now, expandedItems);

            return PartialView("ComponentsTreeElementContent", model);
        }

        public PartialViewResult GetComponentsTreeItemDetails(Guid id)
        {
            var expandedItems = GetExpandedItemsFromCookie();
            var now = Now();
            var components = GetSimplifiedComponentList();
            var component = components.Single(t => t.Id == id);

            var model = GetTreeItemDetailsModel(component, now, expandedItems);

            return PartialView("ComponentsTreeElementDetails", model);
        }

        protected ComponentsTreeItemModel GetTreeItemModel(
            MonitoringStatus[] statuses, Guid? componentTypeId, string search,
            SimplifiedComponent component,
            DateTime now,
            string[] expandedItems)
        {
            // Загружаем необходимые данные
            if (!component.ItemModelInternalDataLoaded)
            {
                var storage = GetStorage();

                var dbComponent = storage.Components.GetOneById(component.Id);
                var bulb = storage.Bulbs.GetOneById(dbComponent.ExternalStatusId);

                component.ExternalStatus = new SimplifiedStatusData()
                {
                    Status = bulb.Status,
                    StartDate = bulb.StartDate,
                    ActualDate = bulb.ActualDate,
                    FirstEventId = null,
                    Message = null
                };

                component.ItemModelInternalDataLoaded = true;
            }

            var result = new ComponentsTreeItemModel()
            {
                Id = component.Id,
                DisplayName = component.DisplayName,
                SystemName = component.SystemName,
                Status = component.ExternalStatus.Status,
                StatusDuration = component.ExternalStatus.GetDuration(now),
                IsRoot = component.ComponentTypeId == SystemComponentType.Root.Id,
                IsFolder = component.ComponentTypeId == SystemComponentType.Folder.Id,
                ParentId = component.ParentId,
                ComponentTypeId = component.ComponentTypeId
            };

            // Изначально развёрнут только корень дерева
            var expanded = result.IsRoot;

            // Если заполнен хотя бы один фильтр, то нужно проверять всех детей
            var isFilterActive = (statuses != null && statuses.Length > 0) || componentTypeId.HasValue || !string.IsNullOrEmpty(search);
            if (isFilterActive)
            {
                var filtered = true;

                if (statuses != null && statuses.Length > 0)
                {
                    if (!statuses.Contains(result.Status))
                        filtered = false;
                }

                if (componentTypeId.HasValue)
                {
                    if (result.ComponentTypeId != componentTypeId.Value)
                        filtered = false;
                }

                if (!string.IsNullOrEmpty(search))
                {
                    if (!(
                        result.Id.ToString().IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        result.SystemName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        result.DisplayName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                    ))
                    {
                        filtered = false;
                    }
                }

                result.Content = GetTreeItemContentModel(statuses, componentTypeId, search, component, now, expandedItems);
                filtered = filtered || (result.Content.Childs.Length > 0);

                // Критерии поиска не выполняются ни у себя, ни у детей -> этот элемент не нужен
                if (!filtered)
                    return null;

                // Раскрываем, если есть дети, подходящие под условия
                if (result.Content.Childs.Length > 0)
                {
                    expanded = true;
                }
            }

            if (expandedItems.Contains(result.Id.ToString()))
                expanded = true;

            result.Expanded = expanded;

            // Если узел раскрыт, то загрузим содержимое
            if (result.Expanded)
            {
                if (result.Content == null)
                {
                    result.Content = GetTreeItemContentModel(statuses, componentTypeId, search, component, now, expandedItems);
                }

                if (result.Content.Details == null)
                {
                    result.Content.Details = GetTreeItemDetailsModel(component, now, expandedItems);
                }
            }

            return result;
        }

        protected ComponentsTreeItemContentModel GetTreeItemContentModel(
            MonitoringStatus[] statuses, Guid? componentTypeId, string search,
            SimplifiedComponent component,
            DateTime now,
            string[] expandedItems)
        {

            // Загружаем необходимые данные
            var childIds = component.Childs.Select(t => t.Id).ToArray();

            var storage = GetStorage();
            var childs = storage.Components.GetMany(childIds);

            var dbChildStatuses = storage.Bulbs.GetMany(childs.Select(t => t.ExternalStatusId).ToArray())
                .Select(t => new
                {
                    Id = t.ComponentId,
                    ExternalStatus = new SimplifiedStatusData()
                    {
                        Status = t.Status,
                        StartDate = t.StartDate,
                        ActualDate = t.ActualDate,
                        FirstEventId = null,
                        Message = null
                    }
                })
                .ToDictionary(a => a.Id, b => b.ExternalStatus);

            foreach (var child in component.Childs)
            {
                child.ExternalStatus = dbChildStatuses[child.Id];
                child.ItemModelInternalDataLoaded = true;
            }

            var result = new ComponentsTreeItemContentModel()
            {
                Childs = component.Childs
                    .Select(t => GetTreeItemModel(statuses, componentTypeId, search, t, now, expandedItems))
                    .Where(t => t != null)
                    .OrderBy(t => t.DisplayName)
                    .ToArray()
            };
            return result;
        }

        protected ComponentsTreeItemDetailsModel GetTreeItemDetailsModel(SimplifiedComponent component, DateTime now, string[] expandedItems)
        {
            // Загружаем необходимые данные
            var storage = GetStorage();

            var dbComponent = storage.Components.GetOneById(component.Id);
            var dbEventsStatus = storage.Bulbs.GetOneById(dbComponent.EventsStatusId);
            var dbUnitTestsStatus = storage.Bulbs.GetOneById(dbComponent.UnitTestsStatusId);
            var dbMetricsStatus = storage.Bulbs.GetOneById(dbComponent.MetricsStatusId);

            component.EventsStatus = new SimplifiedStatusData()
            {
                Status = dbEventsStatus.Status,
                StartDate = dbEventsStatus.StartDate,
                ActualDate = dbEventsStatus.ActualDate,
                FirstEventId = dbEventsStatus.FirstEventId,
                Message = null
            };

            component.UnitTestsStatus = new SimplifiedStatusData()
            {
                Status = dbUnitTestsStatus.Status,
                StartDate = dbUnitTestsStatus.StartDate,
                ActualDate = dbUnitTestsStatus.ActualDate,
                FirstEventId = null,
                Message = null
            };

            component.MetricsStatus = new SimplifiedStatusData()
            {
                Status = dbMetricsStatus.Status,
                StartDate = dbMetricsStatus.StartDate,
                ActualDate = dbMetricsStatus.ActualDate,
                FirstEventId = null,
                Message = null
            };

            var dbUnitTests = storage.UnitTests.GetByComponentId(component.Id);
            var dbUnitTestBulbs = storage.Bulbs.GetMany(dbUnitTests.Select(t => t.StatusDataId).ToArray()).ToDictionary(a => a.Id, b => b);
            component.Unittests = dbUnitTests.Select(x =>
            {
                var bulb = dbUnitTestBulbs[x.StatusDataId];
                return new SimplifiedUnittest()
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    StatusData = new SimplifiedStatusData()
                    {
                        Status = bulb.Status,
                        StartDate = bulb.StartDate,
                        ActualDate = bulb.ActualDate,
                        FirstEventId = null,
                        Message = bulb.Message
                    }
                };
            });

            var dbMetrics = storage.Metrics.GetByComponentId(component.Id);
            var dbMetricBulbs = storage.Bulbs.GetMany(dbMetrics.Select(t => t.StatusDataId).ToArray()).ToDictionary(a => a.Id, b => b);
            var dbMetricTypes = storage.MetricTypes.GetMany(dbMetrics.Select(t => t.MetricTypeId).ToArray()).ToDictionary(a => a.Id, b => b);
            component.Metrics = dbMetrics.Select(x =>
            {
                var bulb = dbMetricBulbs[x.StatusDataId];
                var metricType = dbMetricTypes[x.MetricTypeId];
                return new SimplifiedMetric()
                {
                    Id = x.Id,
                    DisplayName = metricType.DisplayName,
                    StatusData = new SimplifiedStatusData()
                    {
                        Status = bulb.Status,
                        StartDate = bulb.StartDate,
                        ActualDate = bulb.ActualDate,
                        FirstEventId = null,
                        Message = null
                    },
                    Value = x.Value
                };
            });

            var result = new ComponentsTreeItemDetailsModel()
            {
                Id = component.Id,
                Events = new ComponentsTreeItemEventsDetailsModel()
                {
                    HasEvents = component.EventsStatus.FirstEventId.HasValue,
                    Status = component.EventsStatus.Status,
                    FromDate = component.EventsStatus.StartDate,
                    StatusDuration = component.EventsStatus.GetDuration(now)
                },
                Unittests = new ComponentsTreeItemUnittestsDetailsModel()
                {
                    Id = component.Id,
                    Status = component.UnitTestsStatus.Status,
                    Items = component.Unittests
                        .OrderByDescending(t => t.StatusData.Status)
                        .ThenBy(t => t.DisplayName)
                        .Select(t => new ComponentsTreeItemUnittestsDetailsItemModel()
                        {
                            Id = t.Id,
                            DisplayName = t.DisplayName,
                            Status = t.StatusData.Status,
                            StatusDuration = t.StatusData.GetDuration(now),
                            Message = t.StatusData.Message
                        })
                        .ToArray(),
                    StatusDuration = component.UnitTestsStatus.GetDuration(now)
                },
                Metrics = new ComponentsTreeItemMetricsDetailsModel()
                {
                    Id = component.Id,
                    Status = component.MetricsStatus.Status,
                    Items = component.Metrics
                        .OrderByDescending(t => t.StatusData.Status)
                        .ThenBy(t => t.DisplayName)
                        .Select(t => new ComponentsTreeItemMetricsDetailsItemModel()
                        {
                            Id = t.Id,
                            DisplayName = t.DisplayName,
                            Status = t.StatusData.Status,
                            StatusDuration = t.StatusData.GetDuration(now),
                            Value = t.Value
                        })
                        .ToArray(),
                    StatusDuration = component.MetricsStatus.GetDuration(now)
                }
            };

            result.Unittests.Expanded = expandedItems.Contains(result.Unittests.HtmlId);
            result.Metrics.Expanded = expandedItems.Contains(result.Metrics.HtmlId);

            return result;
        }

        public PartialViewResult GetComponentsMiniTree()
        {
            var storage = GetStorage();

            // Получим нужные данные по всем компонентам
            var query = storage.Gui.GetComponentMiniTree()
                .Select(t => new ComponentsMiniTreeItemModel()
                {
                    Id = t.Id,
                    DisplayName = t.DisplayName,
                    SystemName = t.SystemName,
                    Status = t.Status,
                    ParentId = t.ParentId,
                    ComponentTypeId = t.ComponentTypeId,
                    IsRoot = t.ComponentTypeId == SystemComponentType.Root.Id,
                    IsFolder = t.ComponentTypeId == SystemComponentType.Folder.Id
                });

            var dict = query.ToDictionary(t => t.Id, t => t);

            // Проставим ссылки на родителя и детей
            foreach (var component in dict.Values.ToArray())
            {
                if (component.ParentId.HasValue)
                {
                    dict.TryGetValue(component.ParentId.Value, out var parent);
                    if (parent != null)
                    {
                        component.Parent = parent;
                        component.Parent.Childs.Add(component);
                    }
                    else
                    {
                        // Если не нашли родителя, значит, он удалён, выбросим компонент из дерева
                        // Вообще такого не должно быть, потому что компонент удаляется вместе с детьми
                        // Но иногда всё равно бывает
                        dict.Remove(component.Id);
                    }
                }
            }

            var result = dict.Values.ToArray();

            var root = result.Single(t => t.IsRoot);

            return PartialView("ComponentsMiniTreeElement", root);
        }

        protected string[] GetExpandedItemsFromCookie()
        {
            var cookie = Request.Cookies[ComponentsTreeModel.ExpandedItemsCookieName];
            var value = cookie != null ? HttpUtility.UrlDecode(cookie) : null;

            if (string.IsNullOrEmpty(value))
                return new string[0];

            try
            {
                var items = new JsonSerializer().GetObject<Object[]>(value);
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
            // Получаем всё дерево одним запросом
            // Детализация по каждому компоненту загружается отдельно

            var allComponents = GetStorage().Gui.GetSimplifiedComponentList();

            var query = allComponents
                .Select(t => new SimplifiedComponent()
                {
                    Id = t.Id,
                    DisplayName = t.DisplayName,
                    SystemName = t.SystemName,
                    ComponentTypeId = t.ComponentTypeId,
                    ParentId = t.ParentId,
                });

            var tempComponents = query.ToDictionary(a => a.Id, b => b);

            // Проставим ссылки на родителя и детей
            foreach (var component in tempComponents.Values.ToArray())
            {
                if (component.ParentId.HasValue)
                {
                    tempComponents.TryGetValue(component.ParentId.Value, out var parent);
                    if (parent != null)
                    {
                        component.Parent = parent;
                        component.Parent.Childs.Add(component);
                    }
                    else
                    {
                        // Если не нашли родителя, значит, он удалён, выбросим компонент из дерева
                        // Вообще такого не должно быть, потому что компонент удаляется вместе с детьми
                        // Но иногда всё равно бывает
                        tempComponents.Remove(component.Id);
                    }
                }
            }

            var result = tempComponents.Values.ToArray();

            return result;
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        internal ComponentTreeController(Guid userId) : base(userId) { }

        public ComponentTreeController()
        {
        }

    }
}