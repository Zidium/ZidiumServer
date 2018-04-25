using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.ComponentTreeDetails;
using Zidium.UserAccount.Models.ExtentionProperties;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ComponentTreeDetailsController : ContextController
    {
        /// <summary>
        /// Контейнер разделов детализации по компоненту
        /// </summary>
        public ActionResult ComponentDetails(Guid id)
        {
            var repository = CurrentAccountDbContext.GetComponentRepository();
            var component = repository.GetById(id);

            var model = new ComponentDetailsModel()
            {
                Id = component.Id,
                Name = component.DisplayName,
                CanEdit = CurrentUser.CanEditCommonData() && !component.IsRoot
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по компоненту - общее состояние
        /// </summary>
        public ActionResult ComponentDetailsState(Guid id)
        {
            var repository = CurrentAccountDbContext.GetComponentRepository();
            var component = repository.GetById(id);

            var eventsMiniStatus = ComponentsController.GetEventsMiniStatusModel(id, CurrentAccountDbContext);
            var unittestsMiniStatus = ComponentsController.GetUnittestsMiniStatusModel(CurrentUser.AccountId, id, CurrentAccountDbContext);
            var metricsMiniStatus = ComponentsController.GetMetricsMiniStatusModel(CurrentUser.AccountId, id, CurrentAccountDbContext);
            var childsMiniStatus = ComponentsController.GetChildsMiniStatusModel(CurrentUser.AccountId, id, CurrentAccountDbContext);

            var model = new ComponentDetailsStateModel()
            {
                Id = id,
                SystemName = component.SystemName,
                Status = component.ExternalStatus.Status,
                StatusEventId = component.ExternalStatus.StatusEventId,
                StatusDuration = component.ExternalStatus.GetDuration(MvcApplication.GetServerDateTime()),
                CanEdit = CurrentUser.CanEditCommonData() && !component.IsRoot,
                IsEnabled = component.Enable,
                EventsMiniStatus = eventsMiniStatus,
                UnittestsMiniStatus = unittestsMiniStatus,
                MetricsMiniStatus = metricsMiniStatus,
                ChildsMiniStatus = childsMiniStatus
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по компоненту - история
        /// </summary>
        public ActionResult ComponentDetailsHistory(Guid id)
        {
            var model = new ComponentDetailsHistoryModel()
            {
                Id = id
            };
            return PartialView(model);
        }

        /// <summary>
        /// Детализация по компоненту - проверки
        /// </summary>
        public ActionResult ComponentDetailsUnittests(Guid id)
        {
            var repository = CurrentAccountDbContext.GetUnitTestRepository();

            var unittests = repository.QueryAll()
                .Where(t => t.ComponentId == id)
                .Include("Type")
                .Include("Bulb")
                .ToArray();

            var now = MvcApplication.GetServerDateTime();

            var systemUnittests = unittests
                .Where(t => t.Type.IsSystem)
                .Select(t => new ComponentDetailsUnittestsModel.SystemUnittest()
                {
                    Id = t.Id,
                    Status = t.Bulb.Status,
                    StatusDuration = t.Bulb.GetDuration(now),
                    Name = t.DisplayName,
                    TypeName = t.Type.DisplayName,
                    LastResultDate = t.Bulb.EndDate,
                    LastResult = t.Bulb.GetUnitTestMessage(),
                    Interval = TimeSpan.FromSeconds(t.PeriodSeconds ?? 0),
                    IsEnabled = t.Enable
                })
                .OrderByDescending(t => t.Status)
                .ThenBy(t => t.Name)
                .ToArray();

            var userUnittests = unittests
                .Where(t => !t.Type.IsSystem)
                .Select(t => new ComponentDetailsUnittestsModel.UserUnittest()
                {
                    Id = t.Id,
                    Status = t.Bulb.Status,
                    StatusDuration = t.Bulb.GetDuration(now),
                    Name = t.DisplayName,
                    LastResultDate = t.Bulb.EndDate,
                    LastResult = t.Bulb.GetUnitTestMessage(),
                    ActualDate = t.Bulb.ActualDate,
                    ActualInterval = t.Bulb.ActualDate - now,
                    IsEnabled = t.Enable
                })
                .OrderByDescending(t => t.Status)
                .ThenBy(t => t.Name)
                .ToArray();

            var model = new ComponentDetailsUnittestsModel()
            {
                Id = id,
                CanEdit = CurrentUser.CanEditCommonData(),
                SystemUnittests = systemUnittests,
                UserUnittests = userUnittests
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по компоненту - события
        /// </summary>
        public ActionResult ComponentDetailsEvents(Guid id)
        {
            var model = new ComponentDetailsEventsModel()
            {
                Id = id
            };
            return PartialView(model);
        }

        /// <summary>
        /// Детализация по компоненту - метрики
        /// </summary>
        public ActionResult ComponentDetailsMetrics(Guid id)
        {
            var repository = CurrentAccountDbContext.GetMetricRepository();

            var metrics = repository.QueryAll()
                .Where(t => t.ComponentId == id)
                .Include("MetricType")
                .Include("Bulb")
                .ToArray();

            var now = MvcApplication.GetServerDateTime();

            var metricInfos = metrics
                .Select(t => new ComponentDetailsMetricsModel.MetricInfo()
                {
                    Id = t.Id,
                    Status = t.Bulb.Status,
                    StatusDuration = t.Bulb.GetDuration(now),
                    Name = t.MetricType.DisplayName,
                    LastResultDate = t.Bulb.EndDate,
                    LastResult = t.Value,
                    ActualDate = t.Bulb.ActualDate,
                    ActualInterval = t.Bulb.ActualDate - now,
                    IsEnabled = t.Enable,
                    HasSignal = t.Bulb.HasSignal
                })
                .OrderByDescending(t => t.Status)
                .ThenBy(t => t.Name)
                .ToArray();

            var model = new ComponentDetailsMetricsModel()
            {
                Id = id,
                CanEdit = CurrentUser.CanEditCommonData(),
                Metrics = metricInfos
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по компоненту - информация
        /// </summary>
        public ActionResult ComponentDetailsInfo(Guid id)
        {
            var component = GetComponentById(id);
            var model = new ComponentDetailsInfoModel()
            {
                ComponentId = id,
                SystemName = component.SystemName,
                TypeId = component.ComponentTypeId,
                TypeName = component.ComponentType.DisplayName,
                CreateTime = component.CreatedDate
            };
            return PartialView(model);
        }

        /// <summary>
        /// Детализация по компоненту - лог
        /// </summary>
        public ActionResult ComponentDetailsLog(Guid id, LogLevel? level, int? count)
        {
            level = level ?? LogLevel.Trace;
            count = count ?? 10;
            const int maxCount = 500;
            if (count > maxCount)
            {
                count = maxCount;
            }

            var model = new ComponentDetailsLogModel()
            {
                ComponentId = id,
                Level = level.Value,
                Count = count.Value
            };
            var levels = new[]
            {
                LogLevel.Trace,
                LogLevel.Debug,
                LogLevel.Info,
                LogLevel.Warning,
                LogLevel.Error,
                LogLevel.Fatal
            };
            var levelFilter = levels.Where(x => x >= level.Value).ToArray();

            var messages = CurrentAccountDbContext.GetLogRepository()
                .GetLastRecords(id, null, levelFilter, null, count.Value)
                .ToArray();

            model.Messages = messages
                .OrderBy(x => x.Date)
                .ThenBy(x => x.Order)
                .Select(x => new ComponentDetailsLogModel.LogMessage()
                {
                    Id = x.Id,
                    Level = x.Level,
                    Message = x.Message,
                    Time = x.Date,
                    HasProperties = x.ParametersCount > 0
                }).ToArray();

            return PartialView(model);
        }

        public PartialViewResult ShowTimelinesEventsPartial(Guid id, TimelineInterval interval, bool all = false)
        {
            var toDate = MvcApplication.GetServerDateTime();
            var fromDate = TimelineHelper.IntervalToStartDate(toDate, interval);

            var eventTypeRepository = CurrentAccountDbContext.GetEventTypeRepository();
            var eventTypes = eventTypeRepository.QueryAll().Select(t => new { t.Id, t.DisplayName }).ToDictionary(t => t.Id, t => t);
            var eventRepository = CurrentAccountDbContext.GetEventRepository();

            var query = eventRepository.QueryAllByAccount()
                .Where(t => t.OwnerId == id && t.StartDate <= toDate && t.ActualDate >= fromDate);

            if (all)
                query = query.Where(t => t.Category == EventCategory.ApplicationError || t.Category == EventCategory.ComponentEvent);
            else
                query = query.Where(t => t.Category == EventCategory.ApplicationError);

            var events = query
                .GroupBy(t => t.EventTypeId)
                .Select(t => new
                {
                    Id = t.Key,
                    Importance = t.Max(z => z.Importance),
                    Count = t.Sum(z => z.Count),
                    LastMessage = t.OrderByDescending(z => z.StartDate).FirstOrDefault().Message
                })
                .ToArray();

            var model = new ComponentShowTimelinesGroupModel()
            {
                FromDate = fromDate,
                ToDate = toDate,
                HideUptime = true,
                Items = events
                    .Select(t => new
                    {
                        Id = t.Id,
                        Importance = t.Importance,
                        Count = t.Count,
                        Name = eventTypes[t.Id].DisplayName,
                        LastMessage = t.LastMessage
                    })
                    .OrderByDescending(t => t.Importance)
                    .ThenByDescending(t => t.Count)
                    .ThenBy(t => t.Name)
                    .Take(MaxEventTimelineCount)
                    .Select(t => new ComponentShowTimelinesGroupItemModel()
                    {
                        Action = "ForEventType",
                        Category = null,
                        OwnerId = id,
                        EventTypeId = t.Id,
                        Importance = t.Importance,
                        Name = t.Name,
                        Count = t.Count,
                        Comment = t.LastMessage,
                        Url = Url.Action("Show", "EventTypes", new { Id = t.Id })
                    })
                    .ToArray()
            };

            return PartialView("~/Views/Components/ShowTimelinesPartialGroup.cshtml", model);
        }

        /// <summary>
        /// Контейнер разделов детализации по проверке
        /// </summary>
        public ActionResult UnittestDetails(Guid id)
        {
            var repository = CurrentAccountDbContext.GetUnitTestRepository();
            var unittest = repository.GetById(id);

            var model = new UnittestDetailsModel()
            {
                Id = unittest.Id,
                Name = unittest.DisplayName,
                CanEdit = CurrentUser.CanEditCommonData()
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по проверке - общее состояние
        /// </summary>
        public ActionResult UnittestDetailsState(Guid id)
        {
            var unitTestRepository = CurrentAccountDbContext.GetUnitTestRepository();
            var unittest = unitTestRepository.GetById(id);

            var model = new UnittestDetailsStateModel()
            {
                Id = id,
                Status = unittest.Bulb.Status,
                StatusEventId = unittest.Bulb.StatusEventId,
                StatusDuration = unittest.Bulb.GetDuration(MvcApplication.GetServerDateTime()),
                CanRun = unittest.IsSystemType,
                CanEdit = CurrentUser.CanEditCommonData(),
                IsEnabled = unittest.Enable,
                TypeId = unittest.TypeId,
                LastExecutionDate = unittest.Bulb.EndDate,
                LastExecutionResult = unittest.Bulb.GetUnitTestMessage()
            };

            if (model.TypeId == SystemUnitTestTypes.HttpUnitTestType.Id)
                model.RuleData = unittest.HttpRequestUnitTest?.Rules.FirstOrDefault()?.Url;
            else if (model.TypeId == SystemUnitTestTypes.PingTestType.Id)
                model.RuleData = unittest.PingRule?.Host;
            else if (model.TypeId == SystemUnitTestTypes.DomainNameTestType.Id)
                model.RuleData = unittest.DomainNamePaymentPeriodRule?.Domain;
            else if (model.TypeId == SystemUnitTestTypes.SslTestType.Id)
                model.RuleData = unittest.SslCertificateExpirationDateRule?.Url;
            else if (model.TypeId == SystemUnitTestTypes.SqlTestType.Id)
                model.RuleData = unittest.SqlRule?.Query;

            var eventRepository = CurrentAccountDbContext.GetEventRepository();
            var lastExecutionResultEvent = eventRepository.QueryAll(unittest.Id).Where(t => t.Category == EventCategory.UnitTestResult).OrderByDescending(t => t.StartDate).FirstOrDefault();

            if (lastExecutionResultEvent != null && lastExecutionResultEvent.Properties.Count > 0)
            {
                model.LastExecutionResultProperties = ExtentionPropertiesModel.Create(lastExecutionResultEvent);
            }

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по проверке - настройки
        /// </summary>
        public ActionResult UnitTestDetailsSettings(Guid id)
        {
            var repository = CurrentAccountDbContext.GetUnitTestRepository();
            var unittest = repository.GetById(id);

            var model = new UnittestDetailsSettingsModel()
            {
                Id = unittest.Id,
                TypeName = unittest.Type.DisplayName,
                Name = unittest.DisplayName,
                IsSystem = unittest.IsSystemType,
                ExecutionInterval = unittest.PeriodSeconds != null ? TimeSpan.FromSeconds(unittest.PeriodSeconds.Value) : (TimeSpan?)null,
                ActualInterval = unittest.ActualTimeSecs != null ? TimeSpan.FromSeconds(unittest.ActualTimeSecs.Value) : (TimeSpan?)null,
                TypeId = unittest.TypeId,
            };

            if (model.TypeId == SystemUnitTestTypes.HttpUnitTestType.Id)
            {
                model.HttpUrl = unittest.HttpRequestUnitTest?.Rules.FirstOrDefault()?.Url;
                model.HttpTimeout = TimeSpan.FromSeconds(unittest.HttpRequestUnitTest?.Rules.FirstOrDefault()?.TimeoutSeconds ?? 0);
            }
            else if (model.TypeId == SystemUnitTestTypes.PingTestType.Id)
            {
                model.PingHost = unittest.PingRule?.Host;
                model.PingTimeout = TimeSpan.FromMilliseconds(unittest.PingRule?.TimeoutMs ?? 0);
            }
            else if (model.TypeId == SystemUnitTestTypes.DomainNameTestType.Id)
                model.DomainName = unittest.DomainNamePaymentPeriodRule?.Domain;
            else if (model.TypeId == SystemUnitTestTypes.SslTestType.Id)
                model.SslHost = unittest.SslCertificateExpirationDateRule?.Url;
            else if (model.TypeId == SystemUnitTestTypes.SqlTestType.Id)
                model.SqlQuery = unittest.SqlRule?.Query;

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по проверке - история
        /// </summary>
        public ActionResult UnittestDetailsHistory(Guid id)
        {
            var unitTestRepository = CurrentAccountDbContext.GetUnitTestRepository();
            var unittest = unitTestRepository.GetById(id);

            var model = new UnittestDetailsHistoryModel()
            {
                Id = unittest.Id
            };

            var eventRepository = CurrentAccountDbContext.GetEventRepository();
            var statusEvents = eventRepository.QueryAll(unittest.Id).Where(t => t.Category == EventCategory.UnitTestStatus).OrderByDescending(t => t.StartDate).Take(10).ToArray();

            var now = MvcApplication.GetServerDateTime();
            model.Statuses = statusEvents.Select(t => new UnittestDetailsHistoryModel.StatusModel()
            {
                EventId = t.Id,
                StartDate = t.StartDate,
                Duration = t.GetDuration(now),
                Message = t.GetUnitTestMessage(),
                Importance = t.Importance,
                Count = t.Count
            }).ToArray();

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по проверке - результаты
        /// </summary>
        public ActionResult UnittestDetailsResults(Guid id)
        {
            var unitTestRepository = CurrentAccountDbContext.GetUnitTestRepository();
            var unittest = unitTestRepository.GetById(id);

            var model = new UnittestDetailsResultsModel()
            {
                Id = unittest.Id
            };

            var eventRepository = CurrentAccountDbContext.GetEventRepository();
            var resultEvents = eventRepository.QueryAll(unittest.Id).Where(t => t.Category == EventCategory.UnitTestResult).OrderByDescending(t => t.StartDate).Take(10).ToArray();

            model.Results = resultEvents.Select(t => new UnittestDetailsResultsModel.ResultModel()
            {
                Date = t.StartDate,
                Importance = t.Importance,
                Message = t.GetUnitTestMessage(),
                EventId = t.Id
            }).ToArray();

            return PartialView(model);
        }

        /// <summary>
        /// Контейнер разделов детализации по метрике
        /// </summary>
        public ActionResult MetricDetails(Guid id)
        {
            var repository = CurrentAccountDbContext.GetMetricRepository();
            var metric = repository.GetById(id);

            var model = new MetricDetailsModel()
            {
                Id = metric.Id,
                Name = metric.MetricType.DisplayName,
                CanEdit = CurrentUser.CanEditCommonData()
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по метрике - общее состояние
        /// </summary>
        public ActionResult MetricDetailsState(Guid id)
        {
            var repository = CurrentAccountDbContext.GetMetricRepository();
            var metric = repository.GetById(id);
            var now = MvcApplication.GetServerDateTime();

            var model = new MetricDetailsStateModel()
            {
                Id = metric.Id,
                Status = metric.Bulb.Status,
                StatusEventId = metric.Bulb.StatusEventId,
                StatusDuration = metric.Bulb.GetDuration(now),
                LastResultDate = metric.Bulb.EndDate,
                Value = metric.Value,
                ActualDate = metric.Bulb.ActualDate,
                ActualInterval = metric.Bulb.ActualDate - now,
                IsEnabled = metric.Enable,
                HasSignal = metric.Bulb.HasSignal,
                CanEdit = CurrentUser.CanEditCommonData()
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по метрике - история
        /// </summary>
        public ActionResult MetricDetailsHistory(Guid id)
        {
            var metricRepository = CurrentAccountDbContext.GetMetricRepository();
            var metric = metricRepository.GetById(id);

            var model = new MetricDetailsHistoryModel()
            {
                Id = metric.Id
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по метрике - Значения
        /// </summary>
        public ActionResult MetricDetailsValues(Guid id)
        {
            var metricRepository = CurrentAccountDbContext.GetMetricRepository();
            var metric = metricRepository.GetById(id);

            var metricHistoryRepository = CurrentAccountDbContext.GetMetricHistoryRepository();
            var rows = metricHistoryRepository
                .QueryAllByMetricType(metric.ComponentId, metric.MetricTypeId)
                .OrderByDescending(t => t.BeginDate)
                .Take(10);

            var model = new MetricDetailsValuesModel()
            {
                Id = metric.Id,
                Data = rows.Select(t => new MetricDetailsValuesModel.History()
                {
                    Date = t.BeginDate,
                    Value = t.Value,
                    Color = t.Color,
                    HasSignal = t.HasSignal
                }).ToArray()
            };

            return PartialView(model);
        }

        /// <summary>
        /// Контейнер разделов детализации по событиям
        /// </summary>
        public ActionResult EventsDetails(Guid id)
        {
            var repository = CurrentAccountDbContext.GetComponentRepository();
            var component = repository.GetById(id);

            var model = new EventsDetailsModel()
            {
                Id = component.Id
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по событиям - История
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EventsDetailsHistory(Guid id)
        {
            var model = new EventsDetailsHistoryModel()
            {
                Id = id
            };

            return PartialView(model);
        }

        public static int MaxEventTimelineCount = 10;

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        public ComponentTreeDetailsController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public ComponentTreeDetailsController() { }

    }

}