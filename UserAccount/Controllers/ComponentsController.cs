using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.Api;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Components;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ComponentsController : ContextController
    {
        public ActionResult Index(ColorStatusSelectorValue color, Guid? componentTypeId = null, string search = null)
        {
            var service = CurrentAccountDbContext.GetUserSettingService();
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
                var service = CurrentAccountDbContext.GetUserSettingService();
                service.ShowComponentsAsList(CurrentUser.Id, true);
                CurrentAccountDbContext.SaveChanges();
                return RedirectToAction("List", new { color = color.HasValue ? color : null, componentTypeId, search });
            }

            var model = new ComponentsListModel()
            {
                ComponentTypeId = componentTypeId,
                Search = search,
                Color = color,
                ParentComponentId = parentComponentId
            };
            var repository = CurrentAccountDbContext.GetComponentRepository();
            var query = repository.QueryAll().Include("ComponentType").Include("ExternalStatus");
            if (componentTypeId == null && parentComponentId == null)
            {
                query = query.Where(t =>
                    t.ComponentTypeId != SystemComponentTypes.Root.Id &&
                    t.ComponentTypeId != SystemComponentTypes.Folder.Id);
            }

            if (parentComponentId.HasValue)
            {
                query = query.Where(x => x.ParentId == model.ParentComponentId);
            }

            if (model.ComponentTypeId.HasValue)
                query = query.Where(t => t.ComponentTypeId == model.ComponentTypeId);

            // фильтр по цвету
            if (model.Color.Checked)
            {
                var statuses = model.Color.GetSelectedMonitoringStatuses();
                query = query.Where(t => statuses.Contains(t.ExternalStatus.Status));
            }

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Id.ToString().Contains(search) || t.SystemName.Contains(search) || t.DisplayName.Contains(search));

            model.Components = query.OrderBy(t => t.DisplayName);
            return View(model);
        }


        public ActionResult Show(Guid id)
        {
            var repository = CurrentAccountDbContext.GetComponentRepository();
            var component = repository.GetById(id);

            // Блок с мини-статусами

            var eventsMiniStatus = GetEventsMiniStatusModel(id, CurrentAccountDbContext);
            var unittestsMiniStatus = GetUnittestsMiniStatusModel(CurrentUser.AccountId, id, CurrentAccountDbContext);
            var metricsMiniStatus = GetMetricsMiniStatusModel(CurrentUser.AccountId, id, CurrentAccountDbContext);
            var childsMiniStatus = GetChildsMiniStatusModel(CurrentUser.AccountId, id, CurrentAccountDbContext);

            // Списки

            var unitTestRepository = CurrentAccountDbContext.GetUnitTestRepository();
            var unitTests = unitTestRepository
                .QueryAll()
                .Where(t => t.ComponentId == id)
                .Include("Bulb")
                .Include("Type")
                .OrderByDescending(x => x.Bulb.Status)
                .ThenBy(x => x.DisplayName)
                .ToArray();

            var metricRepository = CurrentAccountDbContext.GetMetricRepository();
            var metrics = metricRepository
                .QueryAll()
                .Where(t => t.ComponentId == id)
                .Include("Bulb")
                .OrderBy(t => t.MetricType.DisplayName)
                .ToArray();

            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            var childs = componentRepository
                .QueryAll()
                .Where(t => t.ParentId == component.Id)
                .Include("ExternalStatus")
                .Include("ComponentType")
                .OrderByDescending(x => x.ExternalStatus.Status).ThenBy(x => x.DisplayName)
                .ToArray();

            var model = new ComponentShowModel()
            {
                Component = component,
                LogConfig = component.LogConfig ?? new LogConfig(),
                ExternalState = component.ExternalStatus,
                InternalState = component.InternalStatus,
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

        public static ComponentMiniStatusModel GetEventsMiniStatusModel(Guid id, AccountDbContext accountDbContext)
        {
            var now = MvcApplication.GetServerDateTime();
            var eventRepository = accountDbContext.GetEventRepository();
            var actualEvents = eventRepository.GetActualClientEvents(id, now).Select(t => new
            {
                Id = t.Id,
                Importance = t.Importance
            });

            var alarmEvents = actualEvents.Where(t => t.Importance == EventImportance.Alarm).ToArray();
            var warningEvents = actualEvents.Where(t => t.Importance == EventImportance.Warning).ToArray();
            var successEvents = actualEvents.Where(t => t.Importance == EventImportance.Success).ToArray();
            var unknownEvents = actualEvents.Where(t => t.Importance == EventImportance.Unknown).ToArray();

            var eventsMiniStatus = new ComponentMiniStatusModel()
            {
                ComponentId = id,
                Alarm = alarmEvents.Length,
                Warning = warningEvents.Length,
                Success = successEvents.Length,
                Unknown = unknownEvents.Length,
                AlarmUrl = ColorLinkHelper.GetEventsUrl(id, now, ObjectColor.Red, alarmEvents.Select(t => t.Id).ToArray()),
                WarningUrl = ColorLinkHelper.GetEventsUrl(id, now, ObjectColor.Yellow, warningEvents.Select(t => t.Id).ToArray()),
                SuccessUrl = ColorLinkHelper.GetEventsUrl(id, now, ObjectColor.Green, successEvents.Select(t => t.Id).ToArray()),
                UnknownUrl = ColorLinkHelper.GetEventsUrl(id, now, ObjectColor.Gray, unknownEvents.Select(t => t.Id).ToArray()),
            };

            return eventsMiniStatus;
        }

        public static ComponentMiniStatusModel GetUnittestsMiniStatusModel(Guid accountId, Guid id, AccountDbContext accountDbContext)
        {
            var unitTestRepository = accountDbContext.GetUnitTestRepository();
            var unitTests = unitTestRepository
                .QueryAll()
                .Where(t => t.ComponentId == id)
                .Include("Bulb")
                .Select(t => new
                {
                    Id = t.Id,
                    Status = t.Bulb.Status
                })
                .ToArray();

            var alarmUnitTests = unitTests.Where(t => t.Status == MonitoringStatus.Alarm).ToArray();
            var warningUnitTests = unitTests.Where(t => t.Status == MonitoringStatus.Warning).ToArray();
            var successUnitTests = unitTests.Where(t => t.Status == MonitoringStatus.Success).ToArray();
            var unknownUnitTests = unitTests.Where(t => t.Status == MonitoringStatus.Unknown || t.Status == MonitoringStatus.Disabled).ToArray();

            var checksMiniStatus = new ComponentMiniStatusModel()
            {
                ComponentId = id,
                Alarm = alarmUnitTests.Length,
                AlarmUrl = ColorLinkHelper.GetUnitTestsUrl(id, ObjectColor.Red, alarmUnitTests.Select(t => t.Id).ToArray()),
                Warning = warningUnitTests.Length,
                WarningUrl = ColorLinkHelper.GetUnitTestsUrl(id, ObjectColor.Yellow, warningUnitTests.Select(t => t.Id).ToArray()),
                Success = successUnitTests.Length,
                SuccessUrl = ColorLinkHelper.GetUnitTestsUrl(id, ObjectColor.Green, successUnitTests.Select(t => t.Id).ToArray()),
                Unknown = unknownUnitTests.Length,
                UnknownUrl = ColorLinkHelper.GetUnitTestsUrl(id, ObjectColor.Gray, unknownUnitTests.Select(t => t.Id).ToArray())
            };

            return checksMiniStatus;
        }

        public static ComponentMiniStatusModel GetMetricsMiniStatusModel(Guid accountId, Guid id, AccountDbContext accountDbContext)
        {
            var metricRepository = accountDbContext.GetMetricRepository();
            var metrics = metricRepository
                .QueryAll()
                .Where(t => t.ComponentId == id)
                .Include("Bulb")
                .Select(t => new
                {
                    Id = t.Id,
                    Status = t.Bulb.Status
                })
                .ToArray();

            var alarmMetrics = metrics.Where(t => t.Status == MonitoringStatus.Alarm).ToArray();
            var warningMetrics = metrics.Where(t => t.Status == MonitoringStatus.Warning).ToArray();
            var successMetrics = metrics.Where(t => t.Status == MonitoringStatus.Success).ToArray();
            var unknownMetrics = metrics.Where(t => t.Status == MonitoringStatus.Unknown || t.Status == MonitoringStatus.Disabled).ToArray();

            var metricsMiniStatus = new ComponentMiniStatusModel()
            {
                ComponentId = id,
                Alarm = alarmMetrics.Length,
                Warning = warningMetrics.Length,
                Success = successMetrics.Length,
                Unknown = unknownMetrics.Length,
                AlarmUrl = ColorLinkHelper.GetMetricsUrl(id, ObjectColor.Red, alarmMetrics.Select(t => t.Id).ToArray()),
                WarningUrl = ColorLinkHelper.GetMetricsUrl(id, ObjectColor.Yellow, warningMetrics.Select(t => t.Id).ToArray()),
                SuccessUrl = ColorLinkHelper.GetMetricsUrl(id, ObjectColor.Green, successMetrics.Select(t => t.Id).ToArray()),
                UnknownUrl = ColorLinkHelper.GetMetricsUrl(id, ObjectColor.Gray, unknownMetrics.Select(t => t.Id).ToArray()),
            };

            return metricsMiniStatus;
        }

        public static ComponentMiniStatusModel GetChildsMiniStatusModel(Guid accountId, Guid id, AccountDbContext accountDbContext)
        {
            var componentRepository = accountDbContext.GetComponentRepository();
            var childs = componentRepository
                .QueryAll()
                .Where(t => t.ParentId == id && t.IsDeleted == false)
                .Include("ExternalStatus")
                .Select(t => new
                {
                    Id = t.Id,
                    Status = t.ExternalStatus.Status
                })
                .ToArray();

            var alarmChilds = childs.Where(t => t.Status == MonitoringStatus.Alarm).ToArray();
            var warningChilds = childs.Where(t => t.Status == MonitoringStatus.Warning).ToArray();
            var successChilds = childs.Where(t => t.Status == MonitoringStatus.Success).ToArray();
            var unknownChilds = childs.Where(t => t.Status == MonitoringStatus.Unknown || t.Status == MonitoringStatus.Disabled).ToArray();

            var childsMiniStatus = new ComponentMiniStatusModel()
            {
                ComponentId = id,
                Alarm = alarmChilds.Length,
                Warning = warningChilds.Length,
                Success = successChilds.Length,
                Unknown = unknownChilds.Length,
                AlarmUrl = ColorLinkHelper.GetChildsUrl(id, ObjectColor.Red, alarmChilds.Select(t => t.Id).ToArray()),
                WarningUrl = ColorLinkHelper.GetChildsUrl(id, ObjectColor.Yellow, warningChilds.Select(t => t.Id).ToArray()),
                SuccessUrl = ColorLinkHelper.GetChildsUrl(id, ObjectColor.Green, successChilds.Select(t => t.Id).ToArray()),
                UnknownUrl = ColorLinkHelper.GetChildsUrl(id, ObjectColor.Gray, unknownChilds.Select(t => t.Id).ToArray()),
            };

            return childsMiniStatus;
        }

        public ActionResult GetComponentErrorStatisticsHtml(Guid componentId)
        {
            var now = DateTime.Now;
            var accountId = CurrentUser.AccountId;
            var repository = CurrentAccountDbContext.GetEventRepository();
            var model = new ComponentErrorStatisticsModel();

            // month
            var period = ReportPeriodHelper.GetRange(ReportPeriod.Month, now);
            model.ByMonth = repository
                .GetErrorsByPeriod(period.From, period.To)
                .Count(x => x.OwnerId == componentId);

            // week
            period = ReportPeriodHelper.GetRange(ReportPeriod.Week, now);
            model.ByWeek = repository
                .GetErrorsByPeriod(period.From, period.To)
                .Count(x => x.OwnerId == componentId);

            // day
            period = ReportPeriodHelper.GetRange(ReportPeriod.Day, now);
            model.ByDay = repository
                .GetErrorsByPeriod(period.From, period.To)
                .Count(x => x.OwnerId == componentId);

            // hour
            period = ReportPeriodHelper.GetRange(ReportPeriod.Hour, now);
            model.ByHour = repository
                .GetErrorsByPeriod(period.From, period.To)
                .Count(x => x.OwnerId == componentId);

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
                var repository = CurrentAccountDbContext.GetComponentRepository();
                var root = repository.GetRoot();
                parentId2 = root.Id;
            }
            var model = new ComponentAddModel()
            {
                ParentId = parentId2,
                ComponentTypeId = SystemComponentTypes.Others.Id,
                CanEditParentId = parentId==null
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult Add(ComponentAddModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            componentRepository.GetById(model.ParentId);

            var componentTypeRepository = CurrentAccountDbContext.GetComponentTypeRepository();
            componentTypeRepository.GetById(model.ComponentTypeId);

            var client = GetDispatcherClient();
            var createData = new CreateComponentRequestData()
            {
                SystemName = model.SystemName ?? model.DisplayName,
                DisplayName = model.DisplayName,
                ParentComponentId = model.ParentId,
                TypeId = model.ComponentTypeId,
                Version = model.Version
            };
            var response = client.CreateComponent(CurrentUser.AccountId, createData);

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
                    Url.Action("Show", new {id = component.Id}));

                this.SetTempMessage(TempMessageType.Success, message);
                return RedirectToAction("Index");
            }

            return GetSuccessJsonResponse(component.Id);
        }

        [CanEditAllData]
        public ActionResult Edit(Guid id)
        {
            var repository = CurrentAccountDbContext.GetComponentRepository();
            var component = repository.GetById(id);
            CheckEditingPermissions(component);
            var model = new ComponentEditModel()
            {
                Id = component.Id,
                DisplayName = component.DisplayName,
                SystemName = component.SystemName,
                ParentId = component.Parent.Id,
                ComponentTypeId = component.ComponentType.Id,
                Version = component.Version,
                IsDeleted = component.IsDeleted,
                Properties = component.Properties.OrderBy(t => t.Name).ToList()
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ComponentEditModel model)
        {
            var repository = CurrentAccountDbContext.GetComponentRepository();
            var component = repository.GetById(model.Id);
            CheckEditingPermissions(component);

            if (!ModelState.IsValid)
            {
                model.Properties = component.Properties.OrderBy(t => t.Name).ToList();
                return View(model);
            }
            var dispatcher = GetDispatcherClient();
            var updateData = new UpdateComponentRequestData()
            {
                Id = model.Id,
                DisplayName = model.DisplayName,
                SystemName = model.SystemName,
                ParentId = model.ParentId,
                TypeId = model.ComponentTypeId,
                Version = model.Version
            };
            var response = dispatcher.UpdateComponent(CurrentUser.AccountId, updateData);

            if (!response.Success)
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage);
                model.Properties = component.Properties.OrderBy(t => t.Name).ToList();
                return View(model);
            }
            return RedirectToAction("Index");
        }

        protected void CheckEditingPermissions(Component component)
        {
            if (component.IsRoot)
                throw new CantEditSystemObjectException(component.Id, Naming.Component);
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
                var data = new SetComponentDisableRequestData()
                {
                    Comment = model.Comment,
                    ToDate = date,
                    ComponentId = model.Id
                };
                client.SetComponentDisable(CurrentUser.AccountId, data).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
                return GetErrorJsonResponse(exception);
            }
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var component = GetComponentById(id);
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
                client.DeleteComponent(CurrentUser.AccountId, id).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
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
            if (model.SystemName != null)
            {
                var repository = CurrentAccountDbContext.GetComponentRepository();
                var component = repository.GetChild(model.ParentId, model.SystemName);
                if (component != null && (model.Id == Guid.Empty || model.Id != component.Id))
                    return Json("Компонент с таким системным именем уже существует", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetComponentsMiniList()
        {
            var repository = CurrentAccountDbContext.GetComponentRepository();
            var query = repository.QueryAll().Include("ExternalStatus");

            query = query.Where(t =>
                t.IsDeleted == false
                && t.ComponentTypeId != SystemComponentTypes.Root.Id
                && t.ComponentTypeId != SystemComponentTypes.Folder.Id);

            query = query.OrderBy(t => t.DisplayName);
            var model = new ComponentsMiniListModel()
            {
                Components = query.Select(t => new ComponentsMiniListItemModel()
                {
                    Id = t.Id,
                    ComponentTypeId = t.ComponentTypeId,
                    DisplayName = t.DisplayName,
                    SystemName = t.SystemName,
                    Status = t.ExternalStatus.Status
                }).ToArray()
            };
            return PartialView("ComponentsMiniList", model);
        }

        [CanEditAllData]
        public ActionResult AddProperty(Guid componentId)
        {
            var repository = CurrentAccountDbContext.GetComponentRepository();
            var component = repository.GetById(componentId);
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

            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            var component = componentRepository.GetById(model.ComponentId);
            CheckEditingPermissions(component);
            var propertyRepository = CurrentAccountDbContext.GetComponentPropertyRepository();
            var property = propertyRepository.Add(component, model.Name, model.Value, model.DataType);

            if (model.ModalMode)
                return PartialView("ComponentPropertyRow", property);

            return Redirect(model.ReturnUrl);
        }

        [CanEditAllData]
        public ActionResult EditProperty(Guid id)
        {
            var repository = CurrentAccountDbContext.GetComponentPropertyRepository();
            var property = repository.GetById(id);
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
            var repository = CurrentAccountDbContext.GetComponentPropertyRepository();
            var property = repository.GetById(model.Id);
            repository.Update(property, model.Name, model.Value, model.DataType);
            if (model.ModalMode)
                return PartialView("ComponentPropertyData", property);
            return Redirect(model.ReturnUrl);
        }

        [CanEditAllData]
        public ActionResult DeleteProperty(Guid id)
        {
            var repository = CurrentAccountDbContext.GetComponentPropertyRepository();
            var property = repository.GetById(id);
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
            var repository = CurrentAccountDbContext.GetComponentPropertyRepository();
            var property = repository.GetById(Guid.Parse(model.Id));
            repository.Remove(property);
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
                dispatcher.SetComponentEnable(CurrentUser.AccountId, id).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
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

                    var rootId = CurrentAccountDbContext.GetComponentRepository().GetRoot().Id;

                    // создаем папку
                    Guid folderId;
                    if (!string.IsNullOrEmpty(model.FolderDisplayName) && !string.IsNullOrEmpty(model.FolderSystemName))
                    {
                        var getFolderData = new GetOrCreateComponentRequestData()
                        {
                            SystemName = model.FolderSystemName,
                            DisplayName = model.FolderDisplayName,
                            TypeId = SystemComponentTypes.Folder.Id,
                            ParentComponentId = rootId
                        };
                        var folderResponse = dispatcher.GetOrCreateComponent(CurrentUser.AccountId, getFolderData);
                        if (folderResponse.Success == false)
                        {
                            throw new UserFriendlyException("Не удалось создать папку для нового компонента: " + folderResponse.ErrorMessage);
                        }
                        folderId = folderResponse.Data.Component.Id;
                    }
                    else
                    {
                        var folderResponse = dispatcher.GetRoot(CurrentUser.AccountId);
                        folderId = folderResponse.Data.Id;
                    }

                    // создаем компонент
                    var data = new GetOrCreateComponentRequestData()
                    {
                        DisplayName = model.DisplayName,
                        SystemName = model.SystemName,
                        TypeId = model.ComponentTypeId,
                        ParentComponentId = folderId
                    };
                    var componentResponse = dispatcher.GetOrCreateComponent(CurrentUser.AccountId, data);
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

        protected ColorCountGroupItem CreateUnitTestColorItem(Guid componentId, List<UnitTest> unitTests, ImportanceColor color)
        {
            var item = new ColorCountGroupItem()
            {
                Count = unitTests.Count
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

        protected ColorCountGroupItem CreateChildComponentColorItem(Guid componentId, List<Component> childs, ImportanceColor color)
        {
            var item = new ColorCountGroupItem()
            {
                Count = childs.Count
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

        protected ColorCountGroupItem CreateMetricsColorItem(Guid componentId, List<Metric> metrics, ImportanceColor color)
        {
            var item = new ColorCountGroupItem()
            {
                Count = metrics.Count
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

        protected ColorCountGroupItem CreateEventsColorItem(Guid componentId, List<Event> events, ImportanceColor color, DateTime now)
        {
            var item = new ColorCountGroupItem()
            {
                Count = events.Count()
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
            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            int componentsCount = componentRepository.QueryAll().Count();
            if (model.IsFilterEmpty() == false || componentsCount < 20)
            {
                var query = componentRepository
                    .QueryAll()
                    .Include("ComponentType")
                    .Include("ExternalStatus");

                // фильтр по типу компонента
                if (model.ComponentTypeId.HasValue)
                {
                    query = query.Where(t => t.ComponentTypeId == model.ComponentTypeId);
                }

                // фильтр по компоненту
                if (model.ComponentId.HasValue)
                {
                    query = query.Where(t => t.Id == model.ComponentId);
                }
                else if (model.ParentComponentId.HasValue)
                {
                    query = query.Where(t => t.ParentId == model.ParentComponentId);
                }
                else
                {
                    var types = new[]
                    {
                        SystemComponentTypes.Folder.Id,
                        SystemComponentTypes.Root.Id
                    };
                    query = query.Where(x => types.Contains(x.ComponentTypeId) == false);
                }

                // фильтр по цвету
                if (model.Color.Checked)
                {
                    var statuses = model.Color.GetSelectedMonitoringStatuses();
                    query = query.Where(t => statuses.Contains(t.ExternalStatus.Status));
                }

                // фильтр по строке
                if (string.IsNullOrEmpty(model.SearchString) == false)
                {
                    string searchString = model.SearchString;
                    query = query.Where(t =>
                        t.SystemName.Contains(searchString)
                        || t.DisplayName.Contains(searchString)
                        || t.ComponentType.SystemName.Contains(searchString)
                        || t.ComponentType.DisplayName.Contains(searchString));
                }

                var components = query
                    .OrderBy(x => x.ComponentType.DisplayName)
                    .ThenBy(x => x.DisplayName)
                    .ThenBy(x => x.CreatedDate)
                    .ToList();

                var now = DateTime.Now;
                foreach (var component in components)
                {
                    var row = new StatesModelRow
                    {
                        ComponentTypeId = component.ComponentTypeId,
                        ComponentTypeName = component.ComponentType.DisplayName,
                        ComponentId = component.Id,
                        ComponentName = component.DisplayName
                    };

                    // события
                    var accountDbContext = CurrentAccountDbContext;
                    var events = accountDbContext.GetEventRepository().GetActualClientEvents(component.Id, now).ToList();

                    var colorEvents = events.Where(x => x.Importance == EventImportance.Alarm).ToList();
                    row.Events.RedItem = CreateEventsColorItem(component.Id, colorEvents, ImportanceColor.Red, now);

                    colorEvents = events.Where(x => x.Importance == EventImportance.Warning).ToList();
                    row.Events.YellowItem = CreateEventsColorItem(component.Id, colorEvents, ImportanceColor.Yellow, now);

                    colorEvents = events.Where(x => x.Importance == EventImportance.Success).ToList();
                    row.Events.GreenItem = CreateEventsColorItem(component.Id, colorEvents, ImportanceColor.Green, now);

                    colorEvents = events.Where(x => x.Importance == EventImportance.Unknown).ToList();
                    row.Events.GrayItem = CreateEventsColorItem(component.Id, colorEvents, ImportanceColor.Gray, now);


                    // проверки
                    var unitTestRepository = CurrentAccountDbContext.GetUnitTestRepository();
                    var unitTests = unitTestRepository.QueryAll().Where(t => t.ComponentId == row.ComponentId).Include("Bulb").ToList();

                    var colorTests = unitTests.Where(x => x.Bulb.Status == MonitoringStatus.Alarm).ToList();
                    row.Checks.RedItem = CreateUnitTestColorItem(component.Id, colorTests, ImportanceColor.Red);

                    colorTests = unitTests.Where(x => x.Bulb.Status == MonitoringStatus.Warning).ToList();
                    row.Checks.YellowItem = CreateUnitTestColorItem(component.Id, colorTests, ImportanceColor.Yellow);

                    colorTests = unitTests.Where(x => x.Bulb.Status == MonitoringStatus.Success).ToList();
                    row.Checks.GreenItem = CreateUnitTestColorItem(component.Id, colorTests, ImportanceColor.Green);

                    colorTests = unitTests.Where(x => x.Bulb.Status == MonitoringStatus.Unknown || x.Bulb.Status == MonitoringStatus.Disabled).ToList();
                    row.Checks.GrayItem = CreateUnitTestColorItem(component.Id, colorTests, ImportanceColor.Gray);


                    // дети
                    var childs = componentRepository.QueryAll().Where(t => t.ParentId == row.ComponentId).Include("ExternalStatus").ToList();

                    var colorChilds = childs.Where(x => x.ExternalStatus.Status == MonitoringStatus.Alarm).ToList();
                    row.Childs.RedItem = CreateChildComponentColorItem(component.Id, colorChilds, ImportanceColor.Red);

                    colorChilds = childs.Where(x => x.ExternalStatus.Status == MonitoringStatus.Warning).ToList();
                    row.Childs.YellowItem = CreateChildComponentColorItem(component.Id, colorChilds, ImportanceColor.Yellow);

                    colorChilds = childs.Where(x => x.ExternalStatus.Status == MonitoringStatus.Success).ToList();
                    row.Childs.GreenItem = CreateChildComponentColorItem(component.Id, colorChilds, ImportanceColor.Green);

                    colorChilds = childs.Where(x => x.ExternalStatus.Status == MonitoringStatus.Unknown || x.ExternalStatus.Status == MonitoringStatus.Disabled).ToList();
                    row.Childs.GrayItem = CreateChildComponentColorItem(component.Id, colorChilds, ImportanceColor.Gray);


                    // метрики
                    var metricRepository = CurrentAccountDbContext.GetMetricRepository();
                    var metrics = metricRepository.QueryAll().Where(t => t.ComponentId == row.ComponentId).Include("Bulb").ToList();

                    var colorMetrics = metrics.Where(t => t.Bulb.Status == MonitoringStatus.Alarm).ToList();
                    row.Counters.RedItem = CreateMetricsColorItem(component.Id, colorMetrics, ImportanceColor.Red);

                    colorMetrics = metrics.Where(t => t.Bulb.Status == MonitoringStatus.Warning).ToList();
                    row.Counters.YellowItem = CreateMetricsColorItem(component.Id, colorMetrics, ImportanceColor.Yellow);

                    colorMetrics = metrics.Where(t => t.Bulb.Status == MonitoringStatus.Success).ToList();
                    row.Counters.GreenItem = CreateMetricsColorItem(component.Id, colorMetrics, ImportanceColor.Green);

                    colorMetrics = metrics.Where(t => t.Bulb.Status == MonitoringStatus.Unknown || t.Bulb.Status == MonitoringStatus.Disabled).ToList();
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

            model.ToDate = MvcApplication.GetServerDateTime();
            model.FromDate = TimelineHelper.IntervalToStartDate(model.ToDate, interval);

            return PartialView(model);
        }

        public PartialViewResult ShowTimelinesUnitTestsPartial(Guid id, DateTime fromDate, DateTime toDate, string importance)
        {
            var importances = EnumHelper.StringToEnumArray<EventImportance>(importance);
            var unitTestRepository = CurrentAccountDbContext.GetUnitTestRepository();
            var unitTests = unitTestRepository.QueryAll().Where(t => t.ComponentId == id).Select(t => new { t.Id, t.DisplayName }).ToDictionary(t => t.Id, t => t);
            var eventRepository = CurrentAccountDbContext.GetEventRepository();
            var events = eventRepository.QueryAllByAccount()
                .Where(t => unitTests.Keys.Contains(t.OwnerId) && t.Category == EventCategory.UnitTestStatus && importances.Contains(t.Importance) && t.StartDate <= toDate && t.ActualDate >= fromDate)
                .GroupBy(t => t.OwnerId)
                .Select(t => t.Key)
                .ToList();
            var model = new ComponentShowTimelinesGroupModel()
            {
                FromDate = fromDate,
                ToDate = toDate,
                Items = importances.Length > 0 ?
                    events.Select(t => new ComponentShowTimelinesGroupItemModel()
                    {
                        Action = "ForUnitTest",
                        Category = EventCategory.UnitTestStatus,
                        OwnerId = t,
                        Name = unitTests[t].DisplayName,
                        Url = Url.Action("ResultDetails", "Unittests", new { id = t })
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
            var metricRepository = CurrentAccountDbContext.GetMetricRepository();
            var metrics = metricRepository.QueryAll().Where(t => t.ComponentId == id).Include("MetricType").Select(t => new { t.Id, Name = t.MetricType.SystemName }).ToDictionary(t => t.Id, t => t);
            var eventRepository = CurrentAccountDbContext.GetEventRepository();
            var events = eventRepository.QueryAllByAccount()
                .Where(t => metrics.Keys.Contains(t.OwnerId) && t.Category == EventCategory.MetricStatus && importances.Contains(t.Importance) && t.StartDate <= toDate && t.ActualDate >= fromDate)
                .GroupBy(t => t.OwnerId)
                .Select(t => t.Key)
                .ToList();
            var model = new ComponentShowTimelinesGroupModel()
            {
                FromDate = fromDate,
                ToDate = toDate,
                Items = importances.Length > 0 ?
                    events.Select(t => new ComponentShowTimelinesGroupItemModel()
                    {
                        Action = "ForMetric",
                        Category = EventCategory.MetricStatus,
                        OwnerId = t,
                        Name = metrics[t].Name,
                        Url = Url.Action("Show", "Metrics", new { id = t })
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
            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            var childs = componentRepository.GetChildComponents(id).Select(t => new { t.Id, t.DisplayName }).ToDictionary(t => t.Id, t => t);
            var eventRepository = CurrentAccountDbContext.GetEventRepository();
            var events = eventRepository.QueryAllByAccount()
                .Where(t => childs.Keys.Contains(t.OwnerId) && t.Category == EventCategory.ComponentExternalStatus && importances.Contains(t.Importance) && t.StartDate <= toDate && t.ActualDate >= fromDate)
                .GroupBy(t => t.OwnerId)
                .Select(t => t.Key)
                .ToList();
            var model = new ComponentShowTimelinesGroupModel()
            {
                FromDate = fromDate,
                ToDate = toDate,
                Items = importances.Length > 0 ?
                    events.Select(t => new ComponentShowTimelinesGroupItemModel()
                    {
                        Action = "ForComponent",
                        Category = EventCategory.ComponentExternalStatus,
                        OwnerId = t,
                        Name = childs[t].DisplayName,
                        Url = Url.Action("Show", "Components", new { id = t })
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
            var eventTypeRepository = CurrentAccountDbContext.GetEventTypeRepository();
            var eventTypes = eventTypeRepository.QueryAll().Select(t => new { t.Id, t.DisplayName }).ToDictionary(t => t.Id, t => t);
            var eventRepository = CurrentAccountDbContext.GetEventRepository();
            var categories = new[]
            {
                EventCategory.ApplicationError,
                EventCategory.ComponentEvent
            };
            var events = eventRepository.QueryAllByAccount()
                .Where(t => t.OwnerId == id && categories.Contains(t.Category) && importances.Contains(t.Importance) && t.StartDate <= toDate && t.ActualDate >= fromDate)
                .GroupBy(t => t.EventTypeId)
                .Select(t => new
                {
                    Id = t.Key,
                    LastMessage = t.OrderByDescending(z => z.StartDate).FirstOrDefault().Message
                })
                .ToList();
            var model = new ComponentShowTimelinesGroupModel()
            {
                FromDate = fromDate,
                ToDate = toDate,
                Items = importances.Length > 0 ?
                    events.Select(t => new ComponentShowTimelinesGroupItemModel()
                    {
                        Action = "ForEventType",
                        Category = null,
                        OwnerId = id,
                        EventTypeId = t.Id,
                        Name = eventTypes[t.Id].DisplayName,
                        Comment = t.LastMessage,
                        Url = Url.Action("Show", "EventTypes", new { Id = t.Id })
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
            var repository = CurrentAccountDbContext.GetComponentRepository();
            var component = repository.GetById(id);
            var dispatcher = GetDispatcherClient();
            var response = dispatcher.GetComponentTotalState(CurrentUser.AccountId, component.Id, true);
            response.Check();
            return RedirectToAction("Show", new { id = component.Id });
        }

        [CanEditAllData]
        public ActionResult DisableAjax(Guid id)
        {
            var component = GetComponentById(id);

            var model = new DisableDialogAjaxModel()
            {
                Id = component.Id,
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
                date = MvcApplication.GetServerDateTime().AddHours(1);
            else if (model.Interval == DisableDialogAjaxModel.DisableInterval.Day)
                date = MvcApplication.GetServerDateTime().AddDays(1);
            else if (model.Interval == DisableDialogAjaxModel.DisableInterval.Week)
                date = MvcApplication.GetServerDateTime().AddDays(7);
            else
                date = model.Date;

            var client = GetDispatcherClient();
            var data = new SetComponentDisableRequestData()
            {
                Comment = model.Comment,
                ToDate = date,
                ComponentId = model.Id
            };
            client.SetComponentDisable(CurrentUser.AccountId, data).Check();

            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult EnableAjax(Guid id)
        {
            var client = GetDispatcherClient();
            client.SetComponentEnable(CurrentUser.AccountId, id).Check();
            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        public ActionResult DeleteAjax(Guid id)
        {
            var component = GetComponentById(id);

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
            client.DeleteComponent(CurrentUser.AccountId, model.Id).Check();
            return GetSuccessJsonResponse();
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        public ComponentsController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public ComponentsController() { }

    }
}