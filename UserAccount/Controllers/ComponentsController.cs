using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Components;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ComponentsController : BaseController
    {
        public ActionResult Index(ColorStatusSelectorValue color, Guid? componentTypeId = null, string search = null)
        {
            var service = new UserSettingService(GetStorage());
            var showAsList = service.ShowComponentsAsList(CurrentUser.Id);
            if (showAsList)
                return RedirectToAction("List", new { color, componentTypeId, search });
            return RedirectToAction("Index", "ComponentTree", new { color, componentTypeId, search });
        }

        public ActionResult List(
            ColorStatusSelectorValue color,
            Guid? componentTypeId = null,
            Guid? parentComponentId = null,
            string search = null,
            string save = null)
        {
            if (save == "1")
            {
                var service = new UserSettingService(GetStorage());
                service.ShowComponentsAsList(CurrentUser.Id, true);
                return RedirectToAction("List", new { color = color.HasValue ? color : null, componentTypeId, search });
            }

            var model = new ComponentsListModel()
            {
                ComponentTypeId = componentTypeId,
                Search = search,
                Color = color,
                ParentComponentId = parentComponentId
            };

            var statuses = model.Color.Checked ? model.Color.GetSelectedMonitoringStatuses() : null;

            var list = GetStorage().Gui.GetComponentList(
                componentTypeId,
                new[] { SystemComponentType.Root.Id, SystemComponentType.Folder.Id },
                parentComponentId,
                statuses,
                search,
                100);

            model.Components = list;
            return View(model);
        }


        public ActionResult Show(Guid id)
        {
            var storage = GetStorage();
            var data = storage.Gui.GetComponentShow(id, Now());

            // Блок с мини-статусами

            var eventsMiniStatus = this.GetEventsMiniStatusModel(id, data);
            var unittestsMiniStatus = this.GetUnittestsMiniStatusModel(id, data);
            var metricsMiniStatus = this.GetMetricsMiniStatusModel(id, data);
            var childsMiniStatus = this.GetChildsMiniStatusModel(id, data);

            // Списки

            var unitTests = data.UnitTests
                .OrderByDescending(x => x.Bulb.Status)
                .ThenBy(x => x.DisplayName)
                .ToArray();

            var metrics = data.Metrics
                .OrderByDescending(x => x.Bulb.Status)
                .ThenBy(t => t.DisplayName)
                .ToArray();

            var childs = data.Childs
                .OrderByDescending(x => x.ExternalStatus.Status).ThenBy(x => x.DisplayName)
                .ToArray();

            var component = storage.Components.GetOneById(id);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);

            var model = new ComponentShowModel()
            {
                Now = Now(),
                ComponentBreadCrumbs = ComponentBreadCrumbsModel.Create(id, GetStorage()),
                Component = component,
                ComponentType = componentType,
                Properties = properties,
                LogConfig = storage.LogConfigs.GetOneByComponentId(id),
                ExternalState = storage.Bulbs.GetOneById(component.ExternalStatusId),
                EventsMiniStatus = eventsMiniStatus,
                UnittestsMiniStatus = unittestsMiniStatus,
                MetricsMiniStatus = metricsMiniStatus,
                ChildsMiniStatus = childsMiniStatus,
                UnitTests = unitTests,
                Metrics = metrics,
                Childs = childs
            };

            return View(model);
        }

        public ActionResult GetComponentErrorStatisticsHtml(Guid componentId)
        {
            var now = DateTime.UtcNow;
            var repository = GetStorage().Events;

            var model = new ComponentErrorStatisticsModel();

            // month
            var period = ReportPeriodHelper.GetRange(ReportPeriod.Month, now);
            model.ByMonth = repository.GetErrorsCountByPeriod(componentId, period.From, period.To);

            // week
            period = ReportPeriodHelper.GetRange(ReportPeriod.Week, now);
            model.ByWeek = repository.GetErrorsCountByPeriod(componentId, period.From, period.To);

            // day
            period = ReportPeriodHelper.GetRange(ReportPeriod.Day, now);
            model.ByDay = repository.GetErrorsCountByPeriod(componentId, period.From, period.To);

            // hour
            period = ReportPeriodHelper.GetRange(ReportPeriod.Hour, now);
            model.ByHour = repository.GetErrorsCountByPeriod(componentId, period.From, period.To);

            return View("ComponentErrorStatistics", model);
        }

        [CanEditAllData]
        public ActionResult Add(Guid? parentId = null)
        {
            Guid parentId2;
            if (parentId.HasValue)
                parentId2 = parentId.Value;
            else
            {
                var root = GetStorage().Components.GetRoot();
                parentId2 = root.Id;
            }
            var model = new ComponentAddModel()
            {
                ParentId = parentId2,
                ComponentTypeId = SystemComponentType.Others.Id,
                CanEditParentId = parentId == null
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult Add(ComponentAddModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var storage = GetStorage();

            storage.Components.GetOneById(model.ParentId);

            storage.ComponentTypes.GetOneById(model.ComponentTypeId);

            var client = GetDispatcherClient();
            var createData = new CreateComponentRequestData()
            {
                SystemName = model.SystemName ?? model.DisplayName,
                DisplayName = model.DisplayName,
                ParentComponentId = model.ParentId,
                TypeId = model.ComponentTypeId,
                Version = model.Version
            };
            var response = client.CreateComponent(createData);

            if (!response.Success)
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage);
                return View(model);
            }

            var component = response.Data.Component;

            if (!Request.IsSmartBlocksRequest())
            {
                var message = string.Format("Добавлен компонент <a href='{1}' class='alert-link'>{0}</a>",
                    component.DisplayName,
                    Url.Action("Show", new { id = component.Id }));

                this.SetTempMessage(TempMessageType.Success, message);
                return RedirectToAction("Index");
            }

            return GetSuccessJsonResponse(component.Id);
        }

        [CanEditAllData]
        public ActionResult Edit(Guid id)
        {
            var component = GetStorage().Components.GetOneById(id);
            var properties = GetStorage().ComponentProperties.GetByComponentId(id);

            CheckEditingPermissions(component);
            var model = new ComponentEditModel()
            {
                Id = component.Id,
                DisplayName = component.DisplayName,
                SystemName = component.SystemName,
                ParentId = component.ParentId,
                ComponentTypeId = component.ComponentTypeId,
                Version = component.Version,
                IsDeleted = component.IsDeleted,
                Properties = properties.OrderBy(t => t.Name).ToArray()
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ComponentEditModel model)
        {
            var component = GetStorage().Components.GetOneById(model.Id);
            CheckEditingPermissions(component);

            if (!ModelState.IsValid)
            {
                var properties = GetStorage().ComponentProperties.GetByComponentId(model.Id);
                model.Properties = properties.OrderBy(t => t.Name).ToArray();
                return View(model);
            }

            var dispatcher = GetDispatcherClient();
            var updateData = new UpdateComponentRequestDataDto()
            {
                Id = model.Id,
                DisplayName = model.DisplayName,
                SystemName = model.SystemName,
                ParentId = model.ParentId,
                TypeId = model.ComponentTypeId,
                Version = model.Version
            };
            var response = dispatcher.UpdateComponent(updateData);

            if (!response.Success)
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage);
                var properties = GetStorage().ComponentProperties.GetByComponentId(model.Id);
                model.Properties = properties.OrderBy(t => t.Name).ToArray();
                return View(model);
            }
            return RedirectToAction("Index");
        }

        protected void CheckEditingPermissions(ComponentForRead component)
        {
            // Все компоненты можно менять
        }

        [CanEditAllData]
        public ActionResult Disable(Guid id)
        {
            var model = new DisableDialogGetModel()
            {
                Title = "Выключить компонент",
                Message = "На какое время выключить компонент?"
            };
            return View("Dialogs/DisableDialogAjax", model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult Disable(DisableDialogPostModel model)
        {
            try
            {
                var client = GetDispatcherClient();
                var date = model.GetDate();
                var data = new SetComponentDisableRequestDataDto()
                {
                    Comment = model.Comment,
                    ToDate = date,
                    ComponentId = model.Id
                };
                client.SetComponentDisable(data).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception, Logger);
                return GetErrorJsonResponse(exception);
            }
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var component = GetStorage().Components.GetOneById(id);
            var model = new DeleteConfirmationAjaxModel()
            {
                Title = "Удаление компонента",
                Message = "Вы действительно хотите удалить компонент " + component.DisplayName + "?"
            };
            return ViewDialog(model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult Delete(Guid id, string fake)
        {
            try
            {
                var client = GetDispatcherClient();
                client.DeleteComponent(id).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception, Logger);
                return GetErrorJsonResponse(exception);
            }
        }

        protected MonitoringStatus[] ParseStatusString(string statusStr)
        {
            var statuses = statusStr.Split('~').Where(t => t != string.Empty);
            var result = new List<MonitoringStatus>();
            foreach (var status in statuses)
            {
                MonitoringStatus value;
                if (Enum.TryParse(status, true, out value))
                    result.Add(value);
            }
            return result.ToArray();
        }

        public JsonResult CheckSystemName(ComponentEditModel model)
        {
            if (model.SystemName != null && model.ParentId != null)
            {
                var component = GetStorage().Components.GetChild(model.ParentId.Value, model.SystemName);
                if (component != null && (model.Id == Guid.Empty || model.Id != component.Id))
                    return Json("Компонент с таким системным именем уже существует");
            }
            return Json(true);
        }

        public ActionResult GetComponentsMiniList()
        {
            var list = GetStorage().Gui.GetComponentMiniList()
                .Where(t => t.ComponentTypeId != SystemComponentType.Root.Id &&
                            t.ComponentTypeId != SystemComponentType.Folder.Id)
                .OrderBy(t => t.DisplayName);

            var model = new ComponentsMiniListModel()
            {
                Components = list.Select(t => new ComponentsMiniListItemModel()
                {
                    Id = t.Id,
                    ComponentTypeId = t.ComponentTypeId,
                    DisplayName = t.DisplayName,
                    SystemName = t.SystemName,
                    Status = t.Status
                }).ToArray()
            };
            return PartialView("ComponentsMiniList", model);
        }

        [CanEditAllData]
        public ActionResult AddProperty(Guid componentId)
        {
            var component = GetStorage().Components.GetOneById(componentId);
            CheckEditingPermissions(component);
            var model = new ComponentPropertyEditModel()
            {
                ModalMode = Request.IsAjaxRequest(),
                ReturnUrl = Url.Action("Edit", new { id = componentId.ToString() }),
                ComponentId = componentId,
                Id = Guid.Empty,
                DataType = DataType.Unknown
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProperty(ComponentPropertyEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var component = GetStorage().Components.GetOneById(model.ComponentId);
            CheckEditingPermissions(component);

            var id = Ulid.NewUlid();
            GetStorage().ComponentProperties.Add(new ComponentPropertyForAdd()
            {
                Id = id,
                ComponentId = component.Id,
                Name = model.Name,
                Value = model.Value,
                DataType = model.DataType
            });
            var property = GetStorage().ComponentProperties.GetOneById(id);

            if (model.ModalMode)
                return PartialView("ComponentPropertyRow", property);

            return Redirect(model.ReturnUrl);
        }

        [CanEditAllData]
        public ActionResult EditProperty(Guid id)
        {
            var property = GetStorage().ComponentProperties.GetOneById(id);
            var model = new ComponentPropertyEditModel()
            {
                ModalMode = Request.IsAjaxRequest(),
                ReturnUrl = Url.Action("Edit", new { id = property.ComponentId.ToString() }),
                ComponentId = property.ComponentId,
                Id = property.Id,
                Name = property.Name,
                Value = property.Value,
                DataType = property.DataType
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProperty(ComponentPropertyEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var propertyForUpdate = new ComponentPropertyForUpdate(model.Id);
            propertyForUpdate.Name.Set(model.Name);
            propertyForUpdate.Value.Set(model.Value);
            propertyForUpdate.DataType.Set(model.DataType);
            GetStorage().ComponentProperties.Update(propertyForUpdate);

            if (model.ModalMode)
            {
                var property = GetStorage().ComponentProperties.GetOneById(model.Id);
                return PartialView("ComponentPropertyData", property);
            }
            return Redirect(model.ReturnUrl);
        }

        [CanEditAllData]
        public ActionResult DeleteProperty(Guid id)
        {
            var property = GetStorage().ComponentProperties.GetOneById(id);
            var model = new DeleteConfirmationModel()
            {
                Id = id.ToString(),
                Title = "Удаление свойства",
                ModalMode = Request.IsAjaxRequest(),
                Message = "Вы действительно хотите удалить это свойство?",
                ReturnUrl = Url.Action("Edit", new { id = property.ComponentId.ToString() }),
                AjaxUpdateTargetId = "cpr_" + id.ToString(),
                OnAjaxSuccess = "HideModal"
            };
            return View("Dialogs/DeleteConfirmation", model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProperty(DeleteConfirmationModel model)
        {
            GetStorage().ComponentProperties.Delete(Guid.Parse(model.Id));

            if (model.ModalMode)
                return new EmptyResult();
            return Redirect(model.ReturnUrl);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult Enable(Guid id)
        {
            try
            {
                var dispatcher = GetDispatcherClient();
                dispatcher.SetComponentEnable(id).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception, Logger);
                return GetErrorJsonResponse(exception);
            }
        }

        [CanEditAllData]
        public ActionResult CreateNewInFolder(CreateNewInFolderModel model)
        {
            ModelState.Clear();
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult CreateNewInFolder(CreateNewInFolderModel model, string fake)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var dispatcher = GetDispatcherClient();

                    var rootId = GetStorage().Components.GetRoot().Id;

                    // создаем папку
                    Guid folderId;
                    if (!string.IsNullOrEmpty(model.FolderDisplayName) && !string.IsNullOrEmpty(model.FolderSystemName))
                    {
                        var getFolderData = new GetOrCreateComponentRequestDataDto()
                        {
                            SystemName = model.FolderSystemName,
                            DisplayName = model.FolderDisplayName,
                            TypeId = SystemComponentType.Folder.Id,
                            ParentComponentId = rootId
                        };
                        var folderResponse = dispatcher.GetOrCreateComponent(getFolderData);
                        if (folderResponse.Success == false)
                        {
                            throw new UserFriendlyException("Не удалось создать папку для нового компонента: " + folderResponse.ErrorMessage);
                        }
                        folderId = folderResponse.Data.Component.Id;
                    }
                    else
                    {
                        var folderResponse = dispatcher.GetRoot();
                        folderId = folderResponse.Data.Id;
                    }

                    // создаем компонент
                    var data = new GetOrCreateComponentRequestDataDto()
                    {
                        DisplayName = model.DisplayName,
                        SystemName = model.SystemName,
                        TypeId = model.ComponentTypeId,
                        ParentComponentId = folderId
                    };
                    var componentResponse = dispatcher.GetOrCreateComponent(data);
                    if (componentResponse.Success)
                    {
                        model.ComponentId = componentResponse.Data.Component.Id;
                    }
                    else
                    {
                        throw new UserFriendlyException("Не удалось создать новый компонент: " + componentResponse.ErrorMessage);
                    }
                }
            }
            catch (UserFriendlyException exception)
            {
                model.ErrorMessage = exception.Message;
            }
            return View(model);
        }

        protected ColorCountGroupItem CreateUnitTestColorItem(Guid componentId, GetGuiComponentStatesInfo.UnitTestInfo[] unitTests, ImportanceColor color)
        {
            var item = new ColorCountGroupItem()
            {
                Count = unitTests.Length
            };
            if (item.Count == 1)
            {
                item.Url = "/UnitTests/ResultDetails/" + unitTests.First().Id;
            }
            else
            {
                item.Url = "/UnitTests?ComponentId=" + componentId + "&Color=" + color;
            }
            return item;
        }

        protected ColorCountGroupItem CreateChildComponentColorItem(Guid componentId, GetGuiComponentStatesInfo.ChildInfo[] childs, ImportanceColor color)
        {
            var item = new ColorCountGroupItem()
            {
                Count = childs.Length
            };
            if (item.Count == 1)
            {
                item.Url = "/Components/" + childs.First().Id;
            }
            else
            {
                item.Url = "/Components/States?ParentComponentId=" + componentId + "&Color=" + color;
            }
            return item;
        }

        protected ColorCountGroupItem CreateMetricsColorItem(Guid componentId, GetGuiComponentStatesInfo.MetricInfo[] metrics, ImportanceColor color)
        {
            var item = new ColorCountGroupItem()
            {
                Count = metrics.Length
            };
            if (item.Count == 1)
            {
                item.Url = "/Metrics/Show/" + metrics.First().Id;
            }
            else
            {
                item.Url = "/Metrics/Values?componentId=" + componentId + "&Color=" + color;
            }
            return item;
        }

        protected ColorCountGroupItem CreateEventsColorItem(Guid componentId, GetGuiComponentStatesInfo.EventInfo[] events, ImportanceColor color, DateTime now)
        {
            var item = new ColorCountGroupItem()
            {
                Count = events.Length
            };
            if (item.Count == 1)
            {
                item.Url = "/Events/" + events.First().Id;
            }
            else
            {
                item.Url = "/Events?"
                    + "componentId=" + componentId
                    + "&fromDate=" + GuiHelper.GetUrlDateTimeString(now)
                    + "&color=" + color;
            }
            return item;
        }

        public ActionResult States(StatesModel model)
        {
            int componentsCount = GetStorage().Components.GetCount();

            if (model.IsFilterEmpty() == false || componentsCount < 20)
            {
                var query = GetStorage().Gui.GetComponentStates(
                    model.ComponentTypeId,
                    model.ComponentId,
                    model.ParentComponentId,
                    new[]
                    {
                        SystemComponentType.Folder.Id,
                        SystemComponentType.Root.Id
                    },
                    model.Color.Checked ? model.Color.GetSelectedMonitoringStatuses() : null,
                    model.SearchString,
                    Now());

                var components = query
                    .OrderBy(x => x.ComponentType.DisplayName)
                    .ThenBy(x => x.DisplayName)
                    .ThenBy(x => x.CreatedDate)
                    .ToList();

                var now = DateTime.UtcNow;
                foreach (var component in components)
                {
                    var row = new StatesModelRow
                    {
                        ComponentTypeId = component.ComponentType.Id,
                        ComponentTypeName = component.ComponentType.DisplayName,
                        ComponentId = component.Id,
                        ComponentName = component.DisplayName
                    };

                    // события
                    var events = component.Events;

                    var colorEvents = events.Where(x => x.Importance == EventImportance.Alarm).ToArray();
                    row.Events.RedItem = CreateEventsColorItem(component.Id, colorEvents, ImportanceColor.Red, now);

                    colorEvents = events.Where(x => x.Importance == EventImportance.Warning).ToArray();
                    row.Events.YellowItem = CreateEventsColorItem(component.Id, colorEvents, ImportanceColor.Yellow, now);

                    colorEvents = events.Where(x => x.Importance == EventImportance.Success).ToArray();
                    row.Events.GreenItem = CreateEventsColorItem(component.Id, colorEvents, ImportanceColor.Green, now);

                    colorEvents = events.Where(x => x.Importance == EventImportance.Unknown).ToArray();
                    row.Events.GrayItem = CreateEventsColorItem(component.Id, colorEvents, ImportanceColor.Gray, now);


                    // проверки
                    var unitTests = component.UnitTests;

                    var colorTests = unitTests.Where(x => x.Status == MonitoringStatus.Alarm).ToArray();
                    row.Checks.RedItem = CreateUnitTestColorItem(component.Id, colorTests, ImportanceColor.Red);

                    colorTests = unitTests.Where(x => x.Status == MonitoringStatus.Warning).ToArray();
                    row.Checks.YellowItem = CreateUnitTestColorItem(component.Id, colorTests, ImportanceColor.Yellow);

                    colorTests = unitTests.Where(x => x.Status == MonitoringStatus.Success).ToArray();
                    row.Checks.GreenItem = CreateUnitTestColorItem(component.Id, colorTests, ImportanceColor.Green);

                    colorTests = unitTests.Where(x => x.Status == MonitoringStatus.Unknown || x.Status == MonitoringStatus.Disabled).ToArray();
                    row.Checks.GrayItem = CreateUnitTestColorItem(component.Id, colorTests, ImportanceColor.Gray);


                    // дети
                    var childs = component.Childs;

                    var colorChilds = childs.Where(x => x.Status == MonitoringStatus.Alarm).ToArray();
                    row.Childs.RedItem = CreateChildComponentColorItem(component.Id, colorChilds, ImportanceColor.Red);

                    colorChilds = childs.Where(x => x.Status == MonitoringStatus.Warning).ToArray();
                    row.Childs.YellowItem = CreateChildComponentColorItem(component.Id, colorChilds, ImportanceColor.Yellow);

                    colorChilds = childs.Where(x => x.Status == MonitoringStatus.Success).ToArray();
                    row.Childs.GreenItem = CreateChildComponentColorItem(component.Id, colorChilds, ImportanceColor.Green);

                    colorChilds = childs.Where(x => x.Status == MonitoringStatus.Unknown || x.Status == MonitoringStatus.Disabled).ToArray();
                    row.Childs.GrayItem = CreateChildComponentColorItem(component.Id, colorChilds, ImportanceColor.Gray);


                    // метрики
                    var metrics = component.Metrics;

                    var colorMetrics = metrics.Where(t => t.Status == MonitoringStatus.Alarm).ToArray();
                    row.Counters.RedItem = CreateMetricsColorItem(component.Id, colorMetrics, ImportanceColor.Red);

                    colorMetrics = metrics.Where(t => t.Status == MonitoringStatus.Warning).ToArray();
                    row.Counters.YellowItem = CreateMetricsColorItem(component.Id, colorMetrics, ImportanceColor.Yellow);

                    colorMetrics = metrics.Where(t => t.Status == MonitoringStatus.Success).ToArray();
                    row.Counters.GreenItem = CreateMetricsColorItem(component.Id, colorMetrics, ImportanceColor.Green);

                    colorMetrics = metrics.Where(t => t.Status == MonitoringStatus.Unknown || t.Status == MonitoringStatus.Disabled).ToArray();
                    row.Counters.GrayItem = CreateMetricsColorItem(component.Id, colorMetrics, ImportanceColor.Gray);

                    model.Rows.Add(row);
                }
            }
            return View(model);
        }

        public PartialViewResult ShowTimelinesPartial(Guid id, TimelineInterval interval)
        {
            var model = new ComponentShowTimelinesPartialModel()
            {
                ComponentId = id
            };

            model.ToDate = Now();
            model.FromDate = TimelineHelper.IntervalToStartDate(model.ToDate, interval);

            return PartialView(model);
        }

        public PartialViewResult ShowTimelinesUnitTestsPartial(Guid id, DateTime fromDate, DateTime toDate, string importance)
        {
            var importances = EnumHelper.StringToEnumArray<EventImportance>(importance);

            var items = GetStorage().Gui.GetTimelinesUnitTests(id, importances, fromDate, toDate);

            var model = new ComponentShowTimelinesGroupModel()
            {
                FromDate = fromDate,
                ToDate = toDate,
                Items = importances.Length > 0 ?
                    items.Select(t => new ComponentShowTimelinesGroupItemModel()
                    {
                        Action = "ForUnitTest",
                        Category = EventCategory.UnitTestStatus,
                        OwnerId = t.UnitTestId,
                        Name = t.DisplayName,
                        Url = Url.Action("ResultDetails", "Unittests", new { id = t.UnitTestId })
                    })
                    .OrderBy(t => t.Name)
                    .ToArray()
                    : null
            };
            return PartialView("ShowTimelinesPartialGroup", model);
        }

        public PartialViewResult ShowTimelinesMetricsPartial(Guid id, DateTime fromDate, DateTime toDate, string importance)
        {
            var importances = EnumHelper.StringToEnumArray<EventImportance>(importance);

            var items = GetStorage().Gui.GetTimelinesMetrics(id, importances, fromDate, toDate);

            var model = new ComponentShowTimelinesGroupModel()
            {
                FromDate = fromDate,
                ToDate = toDate,
                Items = importances.Length > 0 ?
                    items.Select(t => new ComponentShowTimelinesGroupItemModel()
                    {
                        Action = "ForMetric",
                        Category = EventCategory.MetricStatus,
                        OwnerId = t.MetricId,
                        Name = t.DisplayName,
                        Url = Url.Action("Show", "Metrics", new { id = t.MetricId })
                    })
                    .OrderBy(t => t.Name)
                    .ToArray()
                    : null
            };
            return PartialView("ShowTimelinesPartialGroup", model);
        }

        public PartialViewResult ShowTimelinesChildsPartial(Guid id, DateTime fromDate, DateTime toDate, string importance)
        {
            var importances = EnumHelper.StringToEnumArray<EventImportance>(importance);

            var items = GetStorage().Gui.GetTimelinesChilds(id, importances, fromDate, toDate);

            var model = new ComponentShowTimelinesGroupModel()
            {
                FromDate = fromDate,
                ToDate = toDate,
                Items = importances.Length > 0 ?
                    items.Select(t => new ComponentShowTimelinesGroupItemModel()
                    {
                        Action = "ForComponent",
                        Category = EventCategory.ComponentExternalStatus,
                        OwnerId = t.ComponentId,
                        Name = t.DisplayName,
                        Url = Url.Action("Show", "Components", new { id = t.ComponentId })
                    })
                    .OrderBy(t => t.Name)
                    .ToArray()
                    : null
            };
            return PartialView("ShowTimelinesPartialGroup", model);
        }

        public PartialViewResult ShowTimelinesEventsPartial(Guid id, DateTime fromDate, DateTime toDate, string importance)
        {
            var importances = EnumHelper.StringToEnumArray<EventImportance>(importance);

            var items = GetStorage().Gui.GetTimelinesEvents(id, importances, fromDate, toDate);

            var model = new ComponentShowTimelinesGroupModel()
            {
                FromDate = fromDate,
                ToDate = toDate,
                Items = importances.Length > 0 ?
                    items.Select(t => new ComponentShowTimelinesGroupItemModel()
                    {
                        Action = "ForEventType",
                        Category = null,
                        OwnerId = id,
                        EventTypeId = t.EventTypeId,
                        Name = t.DisplayName,
                        Code = t.Code,
                        Comment = t.LastMessage,
                        Url = Url.Action("Show", "EventTypes", new { Id = t.EventTypeId })
                    })
                    .OrderBy(t => t.Name)
                    .ToArray()
                    : null
            };
            return PartialView("ShowTimelinesPartialGroup", model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult RecalcStatus(Guid id)
        {
            var dispatcher = GetDispatcherClient();
            var response = dispatcher.GetComponentTotalState(id, true);
            response.Check();
            return RedirectToAction("Show", new { id = id });
        }

        [CanEditAllData]
        public ActionResult DisableAjax(Guid id)
        {
            var model = new DisableDialogAjaxModel()
            {
                Id = id,
                Message = "На какое время выключить компонент?",
                Interval = DisableDialogAjaxModel.DisableInterval.Forever
            };
            return PartialView("Dialogs/DisableDialogAjaxNew", model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult DisableAjax(DisableDialogAjaxModel model)
        {
            if (model.Interval == DisableDialogAjaxModel.DisableInterval.Custom && !model.Date.HasValue)
                ModelState.AddModelError("Date", "Пожалуйста, укажите дату");

            if (!ModelState.IsValid)
                return PartialView("Dialogs/DisableDialogAjaxNew", model);

            DateTime? date;

            if (model.Interval == DisableDialogAjaxModel.DisableInterval.Hour)
                date = Now().AddHours(1);
            else if (model.Interval == DisableDialogAjaxModel.DisableInterval.Day)
                date = Now().AddDays(1);
            else if (model.Interval == DisableDialogAjaxModel.DisableInterval.Week)
                date = Now().AddDays(7);
            else
                date = model.Date;

            var client = GetDispatcherClient();
            var data = new SetComponentDisableRequestDataDto()
            {
                Comment = model.Comment,
                ToDate = date,
                ComponentId = model.Id
            };
            client.SetComponentDisable(data).Check();

            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult EnableAjax(Guid id)
        {
            var client = GetDispatcherClient();
            client.SetComponentEnable(id).Check();
            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        public ActionResult DeleteAjax(Guid id)
        {
            var component = GetStorage().Components.GetOneById(id);

            var model = new DeleteDialogAjaxModel()
            {
                Id = component.Id,
                Message = "Вы действительно хотите удалить компонент " + component.DisplayName + "?"
            };
            return PartialView("Dialogs/DeleteDialogAjaxNew", model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult DeleteAjax(DeleteDialogAjaxModel model)
        {
            var client = GetDispatcherClient();
            client.DeleteComponent(model.Id).Check();
            return GetSuccessJsonResponse();
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        internal ComponentsController(Guid userId) : base(userId) { }

        public ComponentsController(ILogger<ComponentsController> logger) : base(logger)
        {
        }
    }
}