using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.UserAccount.Models.ComponentTree;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ComponentTreeController : ContextController
    {
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Index(
            ColorStatusSelectorValue color,
            Guid? componentTypeId = null,
            string search = null,
            string save = null)
        {
            if (save == "1")
            {
                var service = CurrentAccountDbContext.GetUserSettingService();
                service.ShowComponentsAsList(CurrentUser.Id, false);
                CurrentAccountDbContext.SaveChanges();
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
            var now = MvcApplication.GetServerDateTime();
            var statuses = color.GetSelectedMonitoringStatuses();
            var components = GetSimplifiedComponentList();
            var root = components.Single(t => t.Parent == null);

            var model = GetTreeItemModel(statuses, componentTypeId, search, root, now, expandedItems);

            return PartialView("ComponentsTreeElement", model);
        }

        public PartialViewResult GetComponentsTreeItemContent(Guid id)
        {
            var expandedItems = GetExpandedItemsFromCookie();
            var now = MvcApplication.GetServerDateTime();
            var components = GetSimplifiedComponentList();
            var component = components.Single(t => t.Id == id);

            var model = GetTreeItemContentModel(null, null, null, component, now, expandedItems);
            model.Details = GetTreeItemDetailsModel(component, now, expandedItems);

            return PartialView("ComponentsTreeElementContent", model);
        }

        public PartialViewResult GetComponentsTreeItemDetails(Guid id)
        {
            var expandedItems = GetExpandedItemsFromCookie();
            var now = MvcApplication.GetServerDateTime();
            var components = GetSimplifiedComponentList();
            var component = components.Single(t => t.Id == id);

            var model = GetTreeItemDetailsModel(component, now, expandedItems);

            return PartialView("ComponentsTreeElementDetails", model);
        }

        protected ComponentsTreeItemModel GetTreeItemModel(
            List<MonitoringStatus> statuses, Guid? componentTypeId, string search,
            SimplifiedComponent component,
            DateTime now,
            string[] expandedItems)
        {
            // Загружаем необходимые данные
            if (!component.ItemModelInternalDataLoaded)
            {
                var componentRepository = CurrentAccountDbContext.GetComponentRepository();
                var dbComponent = componentRepository.GetById(component.Id);

                component.ExternalStatus = new SimplifiedStatusData()
                {
                    Status = dbComponent.ExternalStatus.Status,
                    StartDate = dbComponent.ExternalStatus.StartDate,
                    ActualDate = dbComponent.ExternalStatus.ActualDate,
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
                IsRoot = component.ComponentTypeId == SystemComponentTypes.Root.Id,
                IsFolder = component.ComponentTypeId == SystemComponentTypes.Folder.Id,
                ParentId = component.ParentId,
                ComponentTypeId = component.ComponentTypeId
            };

            // Изначально развёрнут только корень дерева
            var expanded = result.IsRoot;

            // Если заполнен хотя бы один фильтр, то нужно проверять всех детей
            var isFilterActive = (statuses != null && statuses.Count > 0) || componentTypeId.HasValue || !string.IsNullOrEmpty(search);
            if (isFilterActive)
            {
                var filtered = true;

                if (statuses != null && statuses.Count > 0)
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
                        filtered = false;
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
            List<MonitoringStatus> statuses, Guid? componentTypeId, string search,
            SimplifiedComponent component,
            DateTime now,
            string[] expandedItems)
        {

            // Загружаем необходимые данные
            var childIds = component.Childs.Select(t => t.Id).ToArray();
            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            var dbChildStatuses = componentRepository
                .QueryAll()
                .Where(t => childIds.Contains(t.Id))
                .Include(t => t.ExternalStatus)
                .Select(t => new
                {
                    Id = t.Id,
                    ExternalStatus = new SimplifiedStatusData()
                    {
                        Status = t.ExternalStatus.Status,
                        StartDate = t.ExternalStatus.StartDate,
                        ActualDate = t.ExternalStatus.ActualDate,
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
            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            var dbComponent = componentRepository.GetById(component.Id);

            component.EventsStatus = new SimplifiedStatusData()
            {
                Status = dbComponent.EventsStatus.Status,
                StartDate = dbComponent.EventsStatus.StartDate,
                ActualDate = dbComponent.EventsStatus.ActualDate,
                FirstEventId = dbComponent.EventsStatus.FirstEventId,
                Message = null
            };

            component.UnitTestsStatus = new SimplifiedStatusData()
            {
                Status = dbComponent.UnitTestsStatus.Status,
                StartDate = dbComponent.UnitTestsStatus.StartDate,
                ActualDate = dbComponent.UnitTestsStatus.ActualDate,
                FirstEventId = null,
                Message = null
            };

            component.MetricsStatus = new SimplifiedStatusData()
            {
                Status = dbComponent.MetricsStatus.Status,
                StartDate = dbComponent.MetricsStatus.StartDate,
                ActualDate = dbComponent.MetricsStatus.ActualDate,
                FirstEventId = null,
                Message = null
            };

            component.Unittests = dbComponent.UnitTests.Where(a => a.IsDeleted == false).Select(x =>
                new SimplifiedUnittest()
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    StatusData = new SimplifiedStatusData()
                    {
                        Status = x.Bulb.Status,
                        StartDate = x.Bulb.StartDate,
                        ActualDate = x.Bulb.ActualDate,
                        FirstEventId = null,
                        Message = x.Bulb.Message
                    }
                });

            component.Metrics = dbComponent.Metrics.Where(a => a.IsDeleted == false && a.MetricType.IsDeleted == false)
                .Select(x => new SimplifiedMetric()
                {
                    Id = x.Id,
                    DisplayName = x.MetricType.DisplayName,
                    StatusData = new SimplifiedStatusData()
                    {
                        Status = x.Bulb.Status,
                        StartDate = x.Bulb.StartDate,
                        ActualDate = x.Bulb.ActualDate,
                        FirstEventId = null,
                        Message = null
                    },
                    Value = x.Value
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
            var repository = CurrentAccountDbContext.GetComponentRepository();

            // Получим нужные данные по всем компонентам
            var query = repository
                .QueryAll()
                .Include("ExternalStatus")
                .Select(t => new ComponentsMiniTreeItemModel()
                {
                    Id = t.Id,
                    DisplayName = t.DisplayName,
                    SystemName = t.SystemName,
                    Status = t.ExternalStatus.Status,
                    ParentId = t.ParentId,
                    ComponentTypeId = t.ComponentTypeId,
                    IsRoot = t.ComponentTypeId == SystemComponentTypes.Root.Id,
                    IsFolder = t.ComponentTypeId == SystemComponentTypes.Folder.Id
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

            var root = result.Single(t => t.IsRoot);

            return PartialView("ComponentsMiniTreeElement", root);
        }

        protected string[] GetExpandedItemsFromCookie()
        {
            var cookie = Request.Cookies[ComponentsTreeModel.ExpandedItemsCookieName];
            var value = cookie != null ? HttpUtility.UrlDecode(cookie.Value) : null;

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

            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            var allComponents = componentRepository.QueryAll();

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
        public ComponentTreeController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public ComponentTreeController()
        {
        }

    }
}