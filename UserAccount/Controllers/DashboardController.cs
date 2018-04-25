using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class DashboardController : ContextController
    {
        public ActionResult Index()
        {
            var model = new DashboardModel();

            // Компоненты - общая информация
            var componentRepository = CurrentAccountDbContext.GetComponentRepository();

            var components = componentRepository
                .QueryAll()
                .Include("ExternalStatus").
                Where(t =>
                    t.IsDeleted == false
                    && t.ComponentTypeId != SystemComponentTypes.Root.Id
                    && t.ComponentTypeId != SystemComponentTypes.Folder.Id)
                .ToArray();

            model.ComponentsTotal = new DashboardComponentsInfoModel(components);

            // Компоненты - разбивка по типам
            var componentGroups = components
                .GroupBy(t => t.ComponentType)
                .OrderBy(t => t.Key.DisplayName)
                .ToArray();

            model.ComponentsByTypes = componentGroups
                .Select(t => new DashboardComponentsInfoModel(t.ToArray(), t.Key))
                .ToArray();

            /*
            // События - сбор данных
            var events = new List<DashboardEventModel>();
            var listLockObject = new Object();
            using (var dbProcessor = new MultipleDataBaseProcessor())
            {
                dbProcessor.ForEachStorageDbForAccountId(CurrentUser.AccountId, data =>
                {
                    var eventRepository = data.StorageDbContext.GetEventRepository();
                    var eventsQuery = eventRepository.QueryAllByComponentId(components.Select(t => t.Id).ToArray(), DateTime.Now.AddMonths(-1));
                    eventsQuery = eventsQuery.Where(t => t.Category != EventCategory.ComponentStatus);
                    var eventsList = eventsQuery.Select(t => new DashboardEventModel()
                    {
                        ActualDate = t.ActualDate,
                        EventTypeId = t.EventTypeId,
                        Importance = t.Importance,
                        IsProcessed = t.IsUserHandled,
                        StartDate = t.StartDate
                    }).ToArray();
                    lock (listLockObject)
                    {
                        events.AddRange(eventsList);
                    }
                });
            }
            var actualEvents = events.Where(t => t.ActualDate >= DateTime.Now && t.IsProcessed == false).ToArray();

            // Типы событий - общая информация
            model.ActualEventTypesCount = actualEvents.GroupBy(t => t.EventTypeId).Count();
            model.ActualEventTypesAlertCount = actualEvents.Where(t => t.Importance == EventImportance.Alarm).GroupBy(t => t.EventTypeId).Count();
            model.ActualEventTypesWarningCount = actualEvents.Where(t => t.Importance == EventImportance.Warning).GroupBy(t => t.EventTypeId).Count();
            model.ActualEventTypesInfoCount = actualEvents.Where(t => t.Importance == EventImportance.Info).GroupBy(t => t.EventTypeId).Count();
            model.ActualEventTypesOtherCount = actualEvents.Where(t => t.Importance == EventImportance.Unknown).GroupBy(t => t.EventTypeId).Count();

            // Типы событий - сводка
            var alertEvents = events.Where(t => t.Importance == EventImportance.Alarm).ToArray();
            model.TypesByPeriodAlert = new DashboardEventTypesByPeriodModel(alertEvents);
            var warningEvents = events.Where(t => t.Importance == EventImportance.Warning).ToArray();
            model.TypesByPeriodWarning = new DashboardEventTypesByPeriodModel(warningEvents);
            var infoEvents = events.Where(t => t.Importance == EventImportance.Info).ToArray();
            model.TypesByPeriodInfo = new DashboardEventTypesByPeriodModel(infoEvents);
            var otherEvents = events.Where(t => t.Importance == EventImportance.Unknown).ToArray();
            model.TypesByPeriodOther = new DashboardEventTypesByPeriodModel(otherEvents);

            // События - общая информация
            model.ActualEventsCount = actualEvents.Count();
            model.ActualEventsAlertCount = actualEvents.Count(t => t.Importance == EventImportance.Alarm);
            model.ActualEventsWarningCount = actualEvents.Count(t => t.Importance == EventImportance.Warning);
            model.ActualEventsInfoCount = actualEvents.Count(t => t.Importance == EventImportance.Info);
            model.ActualEventsOtherCount = actualEvents.Count(t => t.Importance == EventImportance.Unknown);

            // События - сводка
            model.ByPeriodAlert = new DashboardEventsByPeriodModel(alertEvents);
            model.ByPeriodWarning = new DashboardEventsByPeriodModel(warningEvents);
            model.ByPeriodInfo = new DashboardEventsByPeriodModel(infoEvents);
            model.ByPeriodOther = new DashboardEventsByPeriodModel(otherEvents);

            // События - по типам
            var eventTypeRepository = CurrentAccountDbContext.GetEventTypeRepository();
            model.ActualEventsAlert = actualEvents.Where(t => t.Importance == EventImportance.Alarm).
                GroupBy(t => t.EventTypeId).Select(t => new
                {
                    EventType = eventTypeRepository.GetById(CurrentUser.AccountId, t.Key),
                    Count = t.Count()
                }).Where(t => t.EventType != null).Select(t => new DashboardEventsByTypeModel()
                {
                    EventType = t.EventType,
                    Count = t.Count
                }).OrderByDescending(t => t.Count).ToArray();

            model.ActualEventsWarning = actualEvents.Where(t => t.Importance == EventImportance.Warning).
                GroupBy(t => t.EventTypeId).Select(t => new
                {
                    EventType = eventTypeRepository.GetById(CurrentUser.AccountId, t.Key),
                    Count = t.Count()
                }).Where(t => t.EventType != null).Select(t => new DashboardEventsByTypeModel()
                {
                    EventType = t.EventType,
                    Count = t.Count
                }).OrderByDescending(t => t.Count).ToArray();
            */

            return View(model);
        }
    }
}