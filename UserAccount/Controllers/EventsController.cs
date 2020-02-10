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
using Zidium.UserAccount.Models.Events;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class EventsController : ContextController
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
            var model = new EventsListModel()
            {
                ComponentId = componentId,
                Color = color ?? new ColorStatusSelectorValue(),
                EventTypeId = eventTypeId,
                Category = category,
                Search = search,
                FromDate = fromDate,
                ToDate = toDate,
                VersionFrom = versionFrom
            };

            IQueryable<Event> eventsQuery;

            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            var eventRepository = CurrentAccountDbContext.GetEventRepository();

            if (componentId.HasValue)
            {
                eventsQuery = eventRepository.QueryAll(componentId.Value);
            }
            else if (unitTestId.HasValue)
            {
                model.UnitTest = CurrentAccountDbContext.GetUnitTestRepository().GetById(unitTestId.Value);
                eventsQuery = eventRepository.QueryAll(unitTestId.Value);
            }
            else if (metricId.HasValue)
            {
                model.Metric = CurrentAccountDbContext.GetMetricRepository().GetById(metricId.Value);
                eventsQuery = eventRepository.QueryAll(metricId.Value);
            }
            else
            {
                if (componentTypeId.HasValue)
                {
                    // поиск по типу компонента
                    var componentIds = componentRepository
                        .QueryAll()
                        .Where(t => t.ComponentTypeId == componentTypeId)
                        .Select(t => t.Id)
                        .ToArray();
                    eventsQuery = eventRepository.QueryAllByComponentId(componentIds);
                }
                else
                {
                    //  все события аккаунта
                    eventsQuery = eventRepository.QueryAllByAccount();
                }
            }

            if (eventTypeId.HasValue)
            {
                eventsQuery = eventsQuery.Where(t => t.EventTypeId == eventTypeId.Value);
            }

            if (category.HasValue)
            {
                eventsQuery = eventsQuery.Where(t => t.Category == category.Value);
            }
            else
            {
                var categories = new[]
                        {
                            EventCategory.ApplicationError, 
                            EventCategory.ComponentEvent
                        };
                eventsQuery = eventsQuery.Where(t => categories.Contains(t.Category));
            }

            if (model.Color.Checked)
            {
                var importanceLevels = color.GetSelectedEventImportances();
                eventsQuery = eventsQuery.Where(t => importanceLevels.Contains(t.Importance));
            }

            if (fromDate.HasValue)
                eventsQuery = eventsQuery.Where(t => t.ActualDate >= fromDate.Value);

            if (toDate.HasValue)
                eventsQuery = eventsQuery.Where(t => t.StartDate <= toDate.Value);

            if (!string.IsNullOrEmpty(search))
                eventsQuery = eventsQuery.Where(t => t.Message != null && t.Message.Contains(search));

            if (!string.IsNullOrEmpty(versionFrom))
            {
                var versionFromLong = VersionHelper.FromString(versionFrom) ?? -1;
                eventsQuery = eventsQuery.Where(t => (t.VersionLong ?? long.MaxValue) > versionFromLong);
            }

            eventsQuery = eventsQuery.OrderByDescending(t => t.StartDate);

            var eventsModel = eventsQuery.Select(t => new EventsListItemModel()
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
            });

            model.Events = eventsModel;

            model.Components = componentRepository
                .QueryAllWithDeleted()
                .ToArray()
                .Select(t => new EventsListComponentModel()
                {
                    Id = t.Id,
                    DisplayName = t.DisplayName,
                    SystemName = t.SystemName,
                    FullName = t.GetFullDisplayName()
                })
                .ToDictionary(t => t.Id, t => t);

            var eventTypeRepository = CurrentAccountDbContext.GetEventTypeRepository();
            model.EventTypes = eventTypeRepository
                .QueryAllWithDeleted()
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
                var xmlEvents = model.GetXmlEvents(CurrentAccountDbContext);
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
            model.Init(id, CurrentUser, CurrentAccountDbContext);
            return View(model);
        }

        protected void CheckEditingPermissions(EventType eventType)
        {
            if (eventType.IsSystem)
                throw new CantEditSystemObjectException(eventType.Id, Naming.EventType);
        }

        [CanEditAllData]
        public ActionResult ChangeImportance(Guid id)
        {
            var eventObj = GetEventById(id);
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
                var eventType = GetEventTypeById(model.EventTypeId);
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
                dispatcher.UpdateEventType(CurrentUser.AccountId, data).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
                return GetErrorJsonResponse(exception);
            }
        }

        public ActionResult GetEventRowProperties(Guid id)
        {
            var eventObj = GetEventById(id);
            return View(eventObj);
        }

        [CanEditAllData]
        public ActionResult Add(Guid componentId, string returnUrl)
        {
            var component = GetComponentById(componentId);
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
                var component = GetComponentById(model.ComponentId);
                var client = GetDispatcherClient();
                var response = client.SendEvent(CurrentUser.AccountId, new SendEventData()
                {
                    ComponentId = component.Id,
                    TypeSystemName = model.EventType,
                    TypeDisplayName = model.EventType,
                    Message = model.Message,
                    Importance = model.EventImportance,
                    JoinInterval = (model.JoinInterval != null ? model.JoinInterval.Value.TotalSeconds : (double?)null),
                    Category = EventCategory.ComponentEvent
                });
                response.Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
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
            model.Load(CurrentUser.AccountId, CurrentAccountDbContext);
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
                Mode  = mode ?? ErrorStatisticsModel.ShowMode.NotProcessed
            };
            model.LoadData(CurrentUser.AccountId, CurrentAccountDbContext);
            return View(model);
        }

        public ActionResult GetDefectControlHtml(Guid eventTypeId)
        {
            var eventType = GetEventTypeById(eventTypeId);
            var html = RazorHelper.GetViewAsString("~/Views/Defects/DefectControl.cshtml", eventType);
            return GetSuccessJsonResponse(new
            {
                html = html
            });
        }

        // Для unit-тестов

        public EventsController() { }

        public EventsController(Guid accountId, Guid userId) : base(accountId, userId) { }

    }
}