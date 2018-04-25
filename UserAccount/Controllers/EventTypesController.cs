using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;
using Zidium.UserAccount.Models.ExtentionProperties;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class EventTypesController : ContextController
    {
        public ActionResult Index(string importance = null, string category = null, string search = null, int? showDeleted = null)
        {
            var eventImportance = EnumHelper.StringToEnum<EventImportance>(importance);
            var eventCategory = EnumHelper.StringToEnum<EventCategory>(category);
            if (eventCategory == null)
            {
                eventCategory = EventCategory.ComponentEvent;
            }

            var repository = CurrentAccountDbContext.GetEventTypeRepository();
            var eventTypes = repository.QueryAllWithDeleted();

            if (eventImportance.HasValue)
                eventTypes = eventTypes.Where(t => t.ImportanceForNew == eventImportance);

            eventTypes = eventTypes.Where(t => t.Category == eventCategory);

            if (!string.IsNullOrEmpty(search))
                eventTypes = eventTypes.Where(t => t.DisplayName.Contains(search) || t.SystemName.Contains(search) || t.Id.ToString() == search);

            if (showDeleted != 1)
                eventTypes = eventTypes.Where(t => t.IsDeleted == false);

            eventTypes = eventTypes.OrderBy(t => t.DisplayName);

            var model = new EventTypesListModel()
            {
                EventTypes = eventTypes,
                Category = eventCategory,
                Importance = eventImportance,
                Search = search,
                ShowDeleted = showDeleted == 1
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

            var repository = CurrentAccountDbContext.GetEventTypeRepository();
            var eventType = new EventType()
            {
                DisplayName = model.DisplayName,
                SystemName = model.SystemName,
                Category = EventCategory.ComponentEvent,
                JoinIntervalSeconds = TimeSpanHelper.GetSeconds(model.JoinInterval),
                OldVersion = model.OldVersion,
                ImportanceForOld = model.ImportanceForOld,
                ImportanceForNew = model.ImportanceForNew
            };
            repository.Add(eventType);
            this.SetTempMessage(TempMessageType.Success, string.Format("Добавлен тип события <a href='{1}' class='alert-link'>{0}</a>", eventType.DisplayName, Url.Action("Show", new { id = eventType.Id })));
            return RedirectToAction("Index");
        }

        public ActionResult Show(Guid id, TimelineInterval? period)
        {
            var repository = CurrentAccountDbContext.GetEventTypeRepository();
            var eventType = repository.GetById(id);
            var model = new EventTypeShowModel()
            {
                Id = eventType.Id,
                DisplayName = eventType.DisplayName,
                SystemName = eventType.SystemName,
                Category = eventType.Category,
                Code = eventType.Code,
                JoinInterval = eventType.JoinInterval,
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
            var repository = CurrentAccountDbContext.GetEventTypeRepository();
            var eventType = repository.GetById(id);
            CheckEditingPermissions(eventType);
            var model = new EventTypeEditModel()
            {
                Id = eventType.Id,
                DisplayName = eventType.DisplayName,
                SystemName = eventType.SystemName,
                Category = eventType.Category,
                JoinInterval = eventType.JoinInterval,
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
            var response = dispatcher.UpdateEventType(CurrentUser.AccountId, data);
            response.Check();

            this.SetTempMessage(TempMessageType.Success, "Тип события сохранён");
            return RedirectToAction("Show", new { id = model.Id });
        }

        protected void CheckEditingPermissions(EventType eventType)
        {
            if (eventType.IsSystem)
                throw new CantEditSystemObjectException(eventType.Id, Naming.EventType);
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var repository = CurrentAccountDbContext.GetEventTypeRepository();
            var eventType = repository.GetById(id);
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
            var repository = CurrentAccountDbContext.GetEventTypeRepository();
            var eventType = repository.GetById(Guid.Parse(model.Id));
            CheckDeletingPermissions(eventType);
            repository.Remove(eventType);
            this.SetTempMessage(TempMessageType.Success, "Тип события удалён");
            return RedirectToAction("Index");
        }

        protected void CheckDeletingPermissions(EventType eventType)
        {
            if (eventType.IsSystem)
                throw new CantDeleteSystemObjectException(eventType.Id, Naming.EventType);
            if (eventType.IsDeleted)
                throw new AlreadyDeletedException(eventType.Id, Naming.EventType);
        }

        public JsonResult CheckSystemName(ComponentTypeEditModel model)
        {
            var repository = CurrentAccountDbContext.GetEventTypeRepository();
            var eventType = repository.GetOneOrNullBySystemName(model.SystemName);
            if (eventType != null && (model.Id == Guid.Empty || model.Id != eventType.Id))
                return Json("Тип события с таким системным именем уже существует", JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEventTypesMiniList(string category = null, string search = null)
        {
            var eventCategory = EnumHelper.StringToEnum<EventCategory>(category);

            var repository = CurrentAccountDbContext.GetEventTypeRepository();
            var query = repository.QueryAll();

            if (eventCategory.HasValue)
                query = query.Where(t => t.Category == eventCategory);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.DisplayName.Contains(search) || t.SystemName.Contains(search) || t.Id.ToString().Contains(search));

            query = query.OrderBy(t => t.DisplayName).Take(100);

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
            var eventRepository = CurrentAccountDbContext.GetEventRepository();
            var lastEvent = eventRepository.GetLastEventByEndDate(id);

            if (lastEvent == null)
                return PartialView("NoLastEventInfo");

            var model = new EventTypeLastEventModel()
            {
                Id = lastEvent.Id,
                EndDate = lastEvent.EndDate,
                Message = lastEvent.Message
            };
            model.Properties = ExtentionPropertiesModel.Create(lastEvent);

            // событие метрики
            if (lastEvent.Category.IsMetricCategory())
            {
                var metricRepository = CurrentAccountDbContext.GetMetricRepository();
                var metric = metricRepository.GetById(lastEvent.OwnerId);
                model.Metric = metric;
                model.Component = metric.Component;
            }
            // событие проверки
            else if (lastEvent.Category.IsUnitTestCategory())
            {
                var unitTestRepository = CurrentAccountDbContext.GetUnitTestRepository();
                var unitTest = unitTestRepository.GetById(lastEvent.OwnerId);
                model.Unittest = unitTest;
                model.Component = unitTest.Component;
            }
            else // событие компонента
            {
                var componentRepository = CurrentAccountDbContext.GetComponentRepository();
                model.Component = componentRepository.GetById(lastEvent.OwnerId);
            }

            return PartialView(model);
        }

        // Для unit-тестов
       
        public EventTypesController() { }

        public EventTypesController(Guid accountId, Guid userId) : base(accountId, userId) { }

    }
}