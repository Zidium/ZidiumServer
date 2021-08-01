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
using Zidium.UserAccount.Models.Controls;
using Zidium.UserAccount.Models.Events;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class EventsController : BaseController
    {
        /// <summary>
        /// Вывод списка событий
        /// </summary>
        public ActionResult Index(
            Guid? componentId = null,
            Guid? componentTypeId = null,
            ColorStatusSelectorValue color = null,
            Guid? eventTypeId = null,
            EventCategory? category = null,
            string search = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            Guid? unitTestId = null,
            Guid? metricId = null,
            string dataType = null,
            string versionFrom = null)
        {
            var storage = GetStorage();

            var model = new EventsListModel()
            {
                ComponentId = componentId,
                Color = color ?? new ColorStatusSelectorValue(),
                EventTypeId = eventTypeId,
                Category = category,
                Search = search,
                FromDate = fromDate,
                ToDate = toDate,
                VersionFrom = versionFrom,
                Storage = storage
            };

            Guid? ownerId = null;
            if (componentId.HasValue)
            {
                ownerId = componentId.Value;
            }
            else if (unitTestId.HasValue)
            {
                model.UnitTest = storage.UnitTests.GetOneById(unitTestId.Value);
                ownerId = unitTestId.Value;
            }
            else if (metricId.HasValue)
            {
                model.Metric = storage.Metrics.GetOneById(metricId.Value);
                ownerId = metricId.Value;
            }

            var categories = category.HasValue
                ? new[] { category.Value }
                : new[] { EventCategory.ApplicationError, EventCategory.ComponentEvent };

            var importanceLevels = model.Color.Checked ? color.GetSelectedEventImportances() : null;
            var maxCount = dataType != "xml" ? 100 : 10000;

            var events = storage.Events.Filter(
                ownerId,
                componentTypeId,
                eventTypeId,
                categories,
                importanceLevels,
                fromDate,
                toDate,
                search,
                versionFrom,
                maxCount
                );

            var eventsModel = events.Select(t => new EventsListItemModel()
            {
                Id = t.Id,
                Message = t.Message,
                OwnerId = t.OwnerId,
                Category = t.Category,
                EventTypeId = t.EventTypeId,
                StartDate = t.StartDate,
                Importance = t.Importance,
                EndDate = t.EndDate,
                ActualDate = t.ActualDate,
                Count = t.Count,
                JoinKey = t.JoinKeyHash
            }).ToArray();

            model.Events = eventsModel;

            var componentService = new ComponentService(storage);
            model.Components = storage.Components.GetMany(model.Events.Select(t => t.OwnerId).Distinct().ToArray())
                .Select(t => new EventsListComponentModel()
                {
                    Id = t.Id,
                    DisplayName = t.DisplayName,
                    SystemName = t.SystemName,
                    FullName = componentService.GetFullDisplayName(t)
                })
                .ToDictionary(t => t.Id, t => t);

            model.EventTypes = storage.EventTypes.GetMany(model.Events.Select(t => t.EventTypeId).Distinct().ToArray())
                .Select(t => new EventsListEventTypeModel()
                {
                    Id = t.Id,
                    DisplayName = t.DisplayName,
                    SystemName = t.SystemName,
                    Code = t.Code,
                    Category = t.Category
                })
                .ToDictionary(t => t.Id, t => t);

            if (dataType == "xml")
            {
                var xmlEvents = GetXmlEvents(model, GetStorage());
                return new ActionXmlFileResult(xmlEvents, "events.xml");
            }

            return View(model);
        }

        /// <summary>
        /// Вывод полной информации о событии
        /// </summary>
        public ActionResult Show(Guid id)
        {
            var model = new ShowEventModel();
            model.Init(id, CurrentUser, GetStorage());
            return View(model);
        }

        protected void CheckEditingPermissions(EventTypeForRead eventType)
        {
            if (eventType.IsSystem)
                throw new UserFriendlyException("Нельзя изменять системный тип события");
        }

        [CanEditAllData]
        public ActionResult ChangeImportance(Guid id)
        {
            var eventObj = GetStorage().Events.GetOneById(id);
            var model = new ChangeImportanceModel()
            {
                EventId = id,
                EventTypeId = eventObj.EventTypeId,
                Version = eventObj.Version,
                Importance = eventObj.Importance
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult ChangeImportance(ChangeImportanceModel model)
        {
            try
            {
                var eventType = GetStorage().EventTypes.GetOneById(model.EventTypeId);
                if (model.EventTypeId == Guid.Empty)
                {
                    throw new UserFriendlyException("Не удалось получить ИД типа события");
                }
                if (model.Importance == null)
                {
                    throw new UserFriendlyException("Не удалось получить важность");
                }
                if (model.Version != null && VersionHelper.FromString(model.Version) == null)
                {
                    throw new UserFriendlyException("Неверный формат версии");
                }
                var dispatcher = GetDispatcherClient();
                var data = new UpdateEventTypeRequestData()
                {
                    EventId = model.EventId,
                    EventTypeId = model.EventTypeId,
                    DisplayName = eventType.DisplayName,
                    ImportanceForNew = eventType.ImportanceForNew,
                    ImportanceForOld = eventType.ImportanceForOld,
                    OldVersion = eventType.OldVersion,
                    UpdateActualEvents = true
                };
                if (string.IsNullOrEmpty(model.Version))
                {
                    data.ImportanceForNew = model.Importance.Value;
                    data.OldVersion = null;
                }
                else
                {
                    data.ImportanceForOld = model.Importance.Value;
                    data.OldVersion = model.Version;
                }
                dispatcher.UpdateEventType(data).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception, Logger);
                return GetErrorJsonResponse(exception);
            }
        }

        public ActionResult GetEventRowProperties(Guid id)
        {
            var model = new GetEventRowPropertiesModel()
            {
                Event = GetStorage().Events.GetOneById(id),
                Properties = GetStorage().EventProperties.GetByEventId(id)
            };
            return View(model);
        }

        [CanEditAllData]
        public ActionResult Add(Guid componentId, string returnUrl)
        {
            var component = GetStorage().Components.GetOneById(componentId);
            var model = new AddEventModel()
            {
                ComponentId = component.Id,
                DisplayName = component.DisplayName,
                EventImportance = EventImportance.Success,
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult Add(AddEventModel model)
        {
            try
            {
                var client = GetDispatcherClient();
                var response = client.SendEvent(new SendEventRequestDataDto()
                {
                    ComponentId = model.ComponentId,
                    TypeSystemName = model.EventType,
                    TypeDisplayName = model.EventType,
                    Message = model.Message,
                    Importance = model.EventImportance,
                    JoinIntervalSeconds = (model.JoinInterval != null ? model.JoinInterval.Value.TotalSeconds : (double?)null),
                    Category = SendEventCategory.ComponentEvent
                });
                response.Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception, Logger);
                return GetErrorJsonResponse(exception);
            }
        }

        public ActionResult WormyApplications(int? top, Guid? componentTypeId, ReportPeriod? period)
        {
            var model = new WormyApplicationsModel()
            {
                Top = top ?? 10,
                ComponentTypeId = componentTypeId
            };
            if (period == null)
            {
                period = ReportPeriod.Day;
            }
            model.PeriodRange = ReportPeriodHelper.GetRange(period.Value);
            model.Load(GetStorage());
            return View(model);
        }

        public ActionResult ErrorStatictics(
            ReportPeriod? period,
            Guid? componentId,
            ErrorStatisticsModel.ShowMode? mode)
        {
            var model = new ErrorStatisticsModel()
            {
                ComponentId = componentId,
                Period = period ?? ReportPeriod.Day,
                Mode = mode ?? ErrorStatisticsModel.ShowMode.NotProcessed
            };
            model.LoadData(GetStorage());
            return View(model);
        }

        // TODO Convert to normal method
        public ActionResult GetDefectControlHtml(Guid eventTypeId)
        {
            var eventType = GetStorage().EventTypes.GetOneById(eventTypeId);
            var defect = eventType.DefectId != null ? GetStorage().Defects.GetOneById(eventType.DefectId.Value) : null;
            var lastChange = defect != null ? GetStorage().DefectChanges.GetLastByDefectId(defect.Id) : null;

            var model = new Models.Defects.DefectControlModel()
            {
                EventType = eventType,
                Defect = defect,
                LastChange = lastChange
            };

            var html = this.RenderViewAsync("~/Views/Defects/DefectControl.cshtml", model).Result;
            return GetSuccessJsonResponse(new
            {
                html = html
            });
        }

        // TODO Move to controller
        private EventXmlData[] GetXmlEvents(EventsListModel model, IStorage storage)
        {
            // загрузим все свойства
            var eventIdArray = model.Events.Select(x => x.Id).ToArray();

            var allProperties = storage.EventProperties.GetByEventIds(eventIdArray);

            var eventPropertiesGroups = allProperties
                .GroupBy(x => x.EventId)
                .ToDictionary(x => x.Key, x => x.ToArray());

            var events = new List<EventXmlData>();
            foreach (var item in model.Events)
            {
                // тип события
                EventsListEventTypeModel eventType = null;
                model.EventTypes.TryGetValue(item.EventTypeId, out eventType);
                if (eventType == null)
                {
                    continue;
                }

                // компонент
                EventsListComponentModel component = null;
                model.Components.TryGetValue(item.OwnerId, out component);
                if (component == null)
                {
                    continue;
                }

                var eventObj = new EventXmlData()
                {
                    Id = item.Id,
                    EventTypeId = eventType.Id,
                    Count = item.Count,
                    ActualDate = item.ActualDate,
                    EndDate = item.EndDate,
                    EventTypeDisplayName = eventType.DisplayName,
                    EventTypeSystemName = eventType.SystemName,
                    JoinKeyHash = item.JoinKey,
                    Message = item.Message,
                    Category = item.Category.ToString(),
                    StartDate = item.StartDate,
                    TypeCode = eventType.Code,
                    OwnerId = item.OwnerId,
                    ComponentId = component.Id,
                    ComponentSystemName = component.SystemName,
                    ComponentDisplayName = component.DisplayName
                };

                EventPropertyForRead[] eventProperties = null;
                if (eventPropertiesGroups.TryGetValue(eventObj.Id, out eventProperties))
                {
                    eventObj.Properties = eventProperties.Select(x => new EventPropertyXml()
                    {
                        Key = x.Name,
                        Type = x.DataType.ToString(),
                        Value = x.Value
                    }).ToArray();
                }
                events.Add(eventObj);
            }
            return events.ToArray();
        }

        // Для unit-тестов

        internal EventsController(Guid userId) : base(userId) { }

        public EventsController(ILogger<EventsController> logger) : base(logger)
        {
        }
    }
}