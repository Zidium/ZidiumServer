using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountDb;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.ExtentionProperties;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class EventTypesController : BaseController
    {
        public ActionResult Index(string importance = null, string category = null, string search = null)
        {
            var eventImportance = EnumHelper.StringToEnum<EventImportance>(importance);
            var eventCategory = EnumHelper.StringToEnum<EventCategory>(category);
            if (eventCategory == null)
            {
                eventCategory = EventCategory.ComponentEvent;
            }

            var eventTypes = GetStorage().EventTypes.Filter(
                eventImportance,
                eventCategory,
                search, 100);

            var model = new EventTypesListModel()
            {
                EventTypes = eventTypes,
                Category = eventCategory,
                Importance = eventImportance,
                Search = search
            };
            return View(model);
        }

        [CanEditAllData]
        public ActionResult Add()
        {
            var model = new EventTypeEditModel();
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(EventTypeEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var eventType = new EventTypeForAdd()
            {
                Id = Guid.NewGuid(),
                DisplayName = model.DisplayName,
                SystemName = model.SystemName,
                Category = EventCategory.ComponentEvent,
                JoinIntervalSeconds = TimeSpanHelper.GetSeconds(model.JoinInterval),
                OldVersion = model.OldVersion,
                ImportanceForOld = model.ImportanceForOld,
                ImportanceForNew = model.ImportanceForNew
            };

            GetStorage().EventTypes.Add(eventType);
            
            this.SetTempMessage(TempMessageType.Success, string.Format("Добавлен тип события <a href='{1}' class='alert-link'>{0}</a>", eventType.DisplayName, Url.Action("Show", new { id = eventType.Id })));
            return RedirectToAction("Index");
        }

        public ActionResult Show(Guid id, TimelineInterval? period)
        {
            var eventType = GetStorage().EventTypes.GetOneById(id);
            var model = new EventTypeShowModel()
            {
                Id = eventType.Id,
                DisplayName = eventType.DisplayName,
                SystemName = eventType.SystemName,
                Category = eventType.Category,
                Code = eventType.Code,
                JoinInterval = eventType.JoinInterval(),
                OldVersion = eventType.OldVersion,
                ImportanceForOld = eventType.ImportanceForOld,
                ImportanceForNew = eventType.ImportanceForNew,
                IsSystem = eventType.IsSystem,
                IsDeleted = eventType.IsDeleted,
                Created = eventType.CreateDate,
                Period = period ?? TimelineInterval.Day
            };
            return View(model);
        }

        [CanEditAllData]
        public ActionResult Edit(Guid id)
        {
            var eventType = GetStorage().EventTypes.GetOneById(id);
            CheckEditingPermissions(eventType);
            var model = new EventTypeEditModel()
            {
                Id = eventType.Id,
                DisplayName = eventType.DisplayName,
                SystemName = eventType.SystemName,
                Category = eventType.Category,
                JoinInterval = eventType.JoinInterval(),
                OldVersion = eventType.OldVersion,
                ImportanceForOld = eventType.ImportanceForOld,
                ImportanceForNew = eventType.ImportanceForNew,
                IsDeleted = eventType.IsDeleted
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EventTypeEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dispatcher = GetDispatcherClient();
            var data = new UpdateEventTypeRequestData()
            {
                EventTypeId = model.Id,
                DisplayName = model.DisplayName,
                SystemName = model.SystemName,
                OldVersion = model.OldVersion,
                JoinIntervalSeconds = TimeSpanHelper.GetSeconds(model.JoinInterval),
                ImportanceForOld = model.ImportanceForOld,
                ImportanceForNew = model.ImportanceForNew,
                UpdateActualEvents = true
            };
            var response = dispatcher.UpdateEventType(data);
            response.Check();

            this.SetTempMessage(TempMessageType.Success, "Тип события сохранён");
            return RedirectToAction("Show", new { id = model.Id });
        }

        protected void CheckEditingPermissions(EventTypeForRead eventType)
        {
            if (eventType.IsSystem)
                throw new UserFriendlyException("Нельзя изменять системный тип события");
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var eventType = GetStorage().EventTypes.GetOneById(id);
            CheckDeletingPermissions(eventType);
            var model = new DeleteConfirmationModel()
            {
                Id = id.ToString(),
                Title = "Удаление типа события",
                ModalMode = Request.IsAjaxRequest(),
                Message = "Вы действительно хотите удалить этот тип события?",
                ReturnUrl = Url.Action("Index")
            };
            return View("~/Views/Shared/Dialogs/DeleteConfirmation.cshtml", model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(DeleteConfirmationModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Shared/Dialogs/DeleteConfirmation.cshtml", model);
            var eventType = GetStorage().EventTypes.GetOneById(Guid.Parse(model.Id));
            CheckDeletingPermissions(eventType);
            var eventTypeForUpdate = eventType.GetForUpdate();
            eventTypeForUpdate.IsDeleted.Set(true);
            GetStorage().EventTypes.Update(eventTypeForUpdate);
            this.SetTempMessage(TempMessageType.Success, "Тип события удалён");
            return RedirectToAction("Index");
        }

        protected void CheckDeletingPermissions(EventTypeForRead eventType)
        {
            if (eventType.IsSystem)
                throw new UserFriendlyException("Нельзя удалять системный тип события");
            if (eventType.IsDeleted)
                throw new UserFriendlyException("Тип события уже удалён");
        }

        public JsonResult CheckSystemName(ComponentTypeEditModel model)
        {
            var eventType = GetStorage().EventTypes.GetOneOrNullBySystemName(model.SystemName);
            if (eventType != null && (model.Id == Guid.Empty || model.Id != eventType.Id))
                return Json("Тип события с таким системным именем уже существует");
            return Json(true);
        }

        public ActionResult GetEventTypesMiniList(string category = null, string search = null)
        {
            var eventCategory = EnumHelper.StringToEnum<EventCategory>(category);

            var query = GetStorage().EventTypes.Filter(null, eventCategory, search, 100);

            var eventTypes = query.Select(t => new EventTypesMiniListItemModel()
                {
                    Id = t.Id,
                    DisplayName = t.DisplayName
                }).ToArray();

            var model = new EventTypesMiniListModel()
            {
                EventTypes = eventTypes,
                IsLimited = eventTypes.Length == 100
            };

            return PartialView("EventTypesMiniList", model);
        }

        public PartialViewResult LastEventInfo(Guid id)
        {
            var storage = GetStorage();
            var lastEvent = storage.Events.GetLastEventByEndDate(id);

            if (lastEvent == null)
                return PartialView("NoLastEventInfo");

            var model = new EventTypeLastEventModel()
            {
                Id = lastEvent.Id,
                EndDate = lastEvent.EndDate,
                Message = lastEvent.Message
            };
            model.Properties = ExtentionPropertiesModel.Create(storage.EventProperties.GetByEventId(lastEvent.Id));

            // событие метрики
            if (lastEvent.Category.IsMetricCategory())
            {
                var metric = storage.Metrics.GetOneById(lastEvent.OwnerId);
                model.Metric = metric;
                model.MetricType = storage.MetricTypes.GetOneById(metric.MetricTypeId);
                model.Component = storage.Components.GetOneById(metric.ComponentId);
            }
            // событие проверки
            else if (lastEvent.Category.IsUnitTestCategory())
            {
                var unitTest = storage.UnitTests.GetOneById(lastEvent.OwnerId);
                model.Unittest = unitTest;
                model.Component = storage.Components.GetOneById(unitTest.ComponentId);
            }
            else // событие компонента
            {
                model.Component = storage.Components.GetOneById(lastEvent.OwnerId);
            }

            return PartialView(model);
        }

        // Для unit-тестов
        
        internal EventTypesController(Guid userId) : base(userId) { }

        public EventTypesController(ILogger<EventTypesController> logger) : base(logger)
        {
        }
    }
}