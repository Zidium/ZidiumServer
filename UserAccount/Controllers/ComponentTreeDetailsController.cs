using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.ComponentTreeDetails;
using Zidium.UserAccount.Models.ExtentionProperties;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ComponentTreeDetailsController : BaseController
    {
        /// <summary>
        /// Контейнер разделов детализации по компоненту
        /// </summary>
        public ActionResult ComponentDetails(Guid id)
        {
            var component = GetStorage().Components.GetOneById(id);

            var model = new ComponentDetailsModel()
            {
                Id = component.Id,
                Name = component.DisplayName,
                CanEdit = CurrentUser.CanEditCommonData() && !component.IsRoot()
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по компоненту - общее состояние
        /// </summary>
        public ActionResult ComponentDetailsState(Guid id)
        {
            var storage = GetStorage();
            var component = storage.Components.GetOneById(id);
            var bulb = storage.Bulbs.GetOneById(component.ExternalStatusId);
            var showInfo = storage.Gui.GetComponentShow(id, Now());

            var eventsMiniStatus = this.GetEventsMiniStatusModel(id, showInfo);
            var unittestsMiniStatus = this.GetUnittestsMiniStatusModel(id, showInfo);
            var metricsMiniStatus = this.GetMetricsMiniStatusModel(id, showInfo);
            var childsMiniStatus = this.GetChildsMiniStatusModel(id, showInfo);

            var model = new ComponentDetailsStateModel()
            {
                Id = id,
                SystemName = component.SystemName,
                Status = bulb.Status,
                StatusEventId = bulb.StatusEventId,
                StatusDuration = bulb.GetDuration(Now()),
                CanEdit = CurrentUser.CanEditCommonData() && !component.IsRoot(),
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
            var storage = GetStorage();

            var unittests = storage.UnitTests.GetByComponentId(id);
            var unittestTypes = storage.UnitTestTypes.GetMany(unittests.Select(t => t.TypeId).Distinct().ToArray()).ToDictionary(a => a.Id, b => b);
            var bulbs = storage.Bulbs.GetMany(unittests.Select(t => t.StatusDataId).ToArray()).ToDictionary(a => a.Id, b => b);

            var data = unittests.Select(t => new
            {
                UnitTest = t,
                UnitTestType = unittestTypes[t.TypeId],
                Bulb = bulbs[t.StatusDataId]
            }).ToArray();

            var now = Now();

            var systemUnittests = data
                .Where(t => t.UnitTestType.IsSystem)
                .Select(t => new ComponentDetailsUnittestsModel.SystemUnittest()
                {
                    Id = t.UnitTest.Id,
                    Status = t.Bulb.Status,
                    StatusDuration = t.Bulb.GetDuration(now),
                    Name = t.UnitTest.DisplayName,
                    TypeName = t.UnitTestType.DisplayName,
                    LastResultDate = t.Bulb.EndDate,
                    LastResult = t.Bulb.GetUnitTestMessage(),
                    Interval = TimeSpan.FromSeconds(t.UnitTest.PeriodSeconds ?? 0),
                    IsEnabled = t.UnitTest.Enable
                })
                .OrderByDescending(t => t.Status)
                .ThenBy(t => t.Name)
                .ToArray();

            var userUnittests = data
                .Where(t => !t.UnitTestType.IsSystem)
                .Select(t => new ComponentDetailsUnittestsModel.UserUnittest()
                {
                    Id = t.UnitTest.Id,
                    Status = t.Bulb.Status,
                    StatusDuration = t.Bulb.GetDuration(now),
                    Name = t.UnitTest.DisplayName,
                    LastResultDate = t.Bulb.EndDate,
                    LastResult = t.Bulb.GetUnitTestMessage(),
                    ActualDate = t.Bulb.ActualDate,
                    ActualInterval = t.Bulb.ActualDate - now,
                    IsEnabled = t.UnitTest.Enable
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
            var storage = GetStorage();

            var metrics = storage.Metrics.GetByComponentId(id);
            var metricTypes = storage.MetricTypes.GetMany(metrics.Select(t => t.MetricTypeId).Distinct().ToArray()).ToDictionary(a => a.Id, b => b);
            var bulbs = storage.Bulbs.GetMany(metrics.Select(t => t.StatusDataId).ToArray()).ToDictionary(a => a.Id, b => b);

            var data = metrics.Select(t => new
            {
                Metric = t,
                MetricType = metricTypes[t.MetricTypeId],
                Bulb = bulbs[t.StatusDataId]
            }).ToArray();

            var now = Now();

            var metricInfos = data
                .Select(t => new ComponentDetailsMetricsModel.MetricInfo()
                {
                    Id = t.Metric.Id,
                    Status = t.Bulb.Status,
                    StatusDuration = t.Bulb.GetDuration(now),
                    Name = t.MetricType.DisplayName,
                    LastResultDate = t.Bulb.EndDate,
                    LastResult = t.Metric.Value,
                    ActualDate = t.Bulb.ActualDate,
                    ActualInterval = t.Bulb.ActualDate - now,
                    IsEnabled = t.Metric.Enable,
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
            var storage = GetStorage();
            var component = storage.Components.GetOneById(id);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var model = new ComponentDetailsInfoModel()
            {
                ComponentId = id,
                SystemName = component.SystemName,
                TypeId = component.ComponentTypeId,
                TypeName = componentType.DisplayName,
                CreateTime = component.CreatedDate
            };
            return PartialView(model);
        }

        /// <summary>
        /// Детализация по компоненту - лог
        /// </summary>
        public ActionResult ComponentDetailsLog(Guid id, Api.Dto.LogLevel? level, int? count)
        {
            level = level ?? Api.Dto.LogLevel.Trace;
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
                Api.Dto.LogLevel.Trace,
                Api.Dto.LogLevel.Debug,
                Api.Dto.LogLevel.Info,
                Api.Dto.LogLevel.Warning,
                Api.Dto.LogLevel.Error,
                Api.Dto.LogLevel.Fatal
            };
            var levelFilter = levels.Where(x => x >= level.Value).ToArray();

            var messages = GetStorage().Logs
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
            var toDate = Now();
            var fromDate = TimelineHelper.IntervalToStartDate(toDate, interval);

            var storage = GetStorage();

            EventCategory[] categories;
            if (all)
                categories = new[] { EventCategory.ApplicationError, EventCategory.ComponentEvent };
            else
                categories = new[] { EventCategory.ApplicationError };

            var events = storage.Events.Filter(id, categories, fromDate, toDate);
            var eventTypes = storage.EventTypes.GetMany(events.Select(t => t.EventTypeId).Distinct().ToArray()).ToDictionary(t => t.Id, t => t);

            var data = events
                .GroupBy(t => t.EventTypeId)
                .Select(t => new
                {
                    Id = t.Key,
                    Importance = t.Max(z => z.Importance),
                    Count = t.Sum(z => z.Count),
                    LastMessage = t.OrderByDescending(z => z.StartDate).FirstOrDefault()?.Message
                })
                .ToArray();

            var model = new ComponentShowTimelinesGroupModel()
            {
                FromDate = fromDate,
                ToDate = toDate,
                HideUptime = true,
                Items = data
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
            var unittest = GetStorage().UnitTests.GetOneById(id);

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
            var storage = GetStorage();
            var unittest = storage.UnitTests.GetOneById(id);
            var bulb = storage.Bulbs.GetOneById(unittest.StatusDataId);

            var model = new UnittestDetailsStateModel()
            {
                Id = id,
                Status = bulb.Status,
                StatusEventId = bulb.StatusEventId,
                StatusDuration = bulb.GetDuration(Now()),
                CanRun = unittest.IsSystemType(),
                CanEdit = CurrentUser.CanEditCommonData(),
                IsEnabled = unittest.Enable,
                TypeId = unittest.TypeId,
                LastExecutionDate = bulb.EndDate,
                LastExecutionResult = bulb.GetUnitTestMessage()
            };

            if (model.TypeId == SystemUnitTestType.HttpUnitTestType.Id)
            {
                var rules = storage.HttpRequestUnitTestRules.GetByUnitTestId(unittest.Id);
                model.RuleData = rules.FirstOrDefault()?.Url;
            }
            else if (model.TypeId == SystemUnitTestType.PingTestType.Id)
            {
                var rule = storage.UnitTestPingRules.GetOneOrNullByUnitTestId(unittest.Id);
                model.RuleData = rule?.Host;
            }
            else if (model.TypeId == SystemUnitTestType.TcpPortTestType.Id)
            {
                var rule = storage.UnitTestTcpPortRules.GetOneOrNullByUnitTestId(unittest.Id);
                model.RuleData = rule?.Host + ":" + rule?.Port;
            }
            else if (model.TypeId == SystemUnitTestType.DomainNameTestType.Id)
            {
                var rule = storage.DomainNamePaymentPeriodRules.GetOneOrNullByUnitTestId(unittest.Id);
                model.RuleData = rule?.Domain;
            }
            else if (model.TypeId == SystemUnitTestType.SslTestType.Id)
            {
                var rule = storage.UnitTestSslCertificateExpirationDateRules.GetOneOrNullByUnitTestId(unittest.Id);
                model.RuleData = rule?.Url;
            }
            else if (model.TypeId == SystemUnitTestType.SqlTestType.Id)
            {
                var rule = storage.UnitTestSqlRules.GetOneOrNullByUnitTestId(unittest.Id);
                model.RuleData = rule?.Query;
            }
            else if (model.TypeId == SystemUnitTestType.VirusTotalTestType.Id)
            {
                var rule = storage.UnitTestVirusTotalRules.GetOneOrNullByUnitTestId(unittest.Id);
                model.RuleData = rule?.Url;
            }

            var lastExecutionResultEvent = storage.Events.GetLastEvent(unittest.Id, EventCategory.UnitTestResult);

            if (lastExecutionResultEvent != null)
            {
                {
                    var properties = storage.EventProperties.GetByEventId(lastExecutionResultEvent.Id);
                    if (properties.Length > 0)
                        model.LastExecutionResultProperties = ExtentionPropertiesModel.Create(properties);
                }
            }

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по проверке - настройки
        /// </summary>
        public ActionResult UnitTestDetailsSettings(Guid id)
        {
            var storage = GetStorage();
            var unittest = storage.UnitTests.GetOneById(id);
            var unitTestType = storage.UnitTestTypes.GetOneById(unittest.TypeId);

            var model = new UnittestDetailsSettingsModel()
            {
                Id = unittest.Id,
                TypeName = unitTestType.DisplayName,
                Name = unittest.DisplayName,
                IsSystem = unittest.IsSystemType(),
                ExecutionInterval = unittest.PeriodSeconds != null ? TimeSpan.FromSeconds(unittest.PeriodSeconds.Value) : (TimeSpan?)null,
                ActualInterval = unittest.ActualTimeSecs != null ? TimeSpan.FromSeconds(unittest.ActualTimeSecs.Value) : (TimeSpan?)null,
                TypeId = unittest.TypeId,
                NoSignalColor = unittest.NoSignalColor ?? unitTestType.NoSignalColor ?? ObjectColor.Red
            };

            if (model.TypeId == SystemUnitTestType.HttpUnitTestType.Id)
            {
                var rules = storage.HttpRequestUnitTestRules.GetByUnitTestId(unittest.Id);
                model.HttpUrl = rules.FirstOrDefault()?.Url;
                model.HttpTimeout = TimeSpan.FromSeconds(rules.FirstOrDefault()?.TimeoutSeconds ?? 0);
            }
            else if (model.TypeId == SystemUnitTestType.PingTestType.Id)
            {
                var rule = storage.UnitTestPingRules.GetOneOrNullByUnitTestId(unittest.Id);
                model.PingHost = rule?.Host;
                model.PingTimeout = TimeSpan.FromMilliseconds(rule?.TimeoutMs ?? 0);
            }
            else if (model.TypeId == SystemUnitTestType.TcpPortTestType.Id)
            {
                var rule = storage.UnitTestTcpPortRules.GetOneOrNullByUnitTestId(unittest.Id);
                model.PingHost = rule?.Host;
                model.TcpPort = rule?.Port ?? 0;
            }
            else if (model.TypeId == SystemUnitTestType.VirusTotalTestType.Id)
            {
                var rule = storage.UnitTestVirusTotalRules.GetOneOrNullByUnitTestId(unittest.Id);
                model.HttpUrl = rule.Url;
            }
            else if (model.TypeId == SystemUnitTestType.DomainNameTestType.Id)
            {
                var rule = storage.DomainNamePaymentPeriodRules.GetOneOrNullByUnitTestId(unittest.Id);
                model.DomainName = rule?.Domain;
            }
            else if (model.TypeId == SystemUnitTestType.SslTestType.Id)
            {
                var rule = storage.UnitTestSslCertificateExpirationDateRules.GetOneOrNullByUnitTestId(unittest.Id);
                model.SslHost = rule?.Url;
            }
            else if (model.TypeId == SystemUnitTestType.SqlTestType.Id)
            {
                var rule = storage.UnitTestSqlRules.GetOneOrNullByUnitTestId(unittest.Id);
                model.SqlQuery = rule?.Query;
            }

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по проверке - история
        /// </summary>
        public ActionResult UnittestDetailsHistory(Guid id)
        {
            var model = new UnittestDetailsHistoryModel()
            {
                Id = id
            };

            var statusEvents = GetStorage().Events.GetLastEvents(id, EventCategory.UnitTestStatus, 10);

            var now = Now();
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
            var model = new UnittestDetailsResultsModel()
            {
                Id = id
            };

            var resultEvents = GetStorage().Events.GetLastEvents(id, EventCategory.UnitTestResult, 10);

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
            var storage = GetStorage();
            var metric = storage.Metrics.GetOneById(id);
            var metricType = storage.MetricTypes.GetOneById(metric.MetricTypeId);

            var model = new MetricDetailsModel()
            {
                Id = metric.Id,
                Name = metricType.DisplayName,
                CanEdit = CurrentUser.CanEditCommonData()
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по метрике - общее состояние
        /// </summary>
        public ActionResult MetricDetailsState(Guid id)
        {
            var storage = GetStorage();
            var metric = storage.Metrics.GetOneById(id);
            var bulb = storage.Bulbs.GetOneById(metric.StatusDataId);
            var now = Now();

            var model = new MetricDetailsStateModel()
            {
                Id = metric.Id,
                Status = bulb.Status,
                StatusEventId = bulb.StatusEventId,
                StatusDuration = bulb.GetDuration(now),
                LastResultDate = bulb.EndDate,
                Value = metric.Value,
                ActualDate = bulb.ActualDate,
                ActualInterval = bulb.ActualDate - now,
                IsEnabled = metric.Enable,
                HasSignal = bulb.HasSignal,
                CanEdit = CurrentUser.CanEditCommonData()
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по метрике - история
        /// </summary>
        public ActionResult MetricDetailsHistory(Guid id)
        {
            var model = new MetricDetailsHistoryModel()
            {
                Id = id
            };

            return PartialView(model);
        }

        /// <summary>
        /// Детализация по метрике - Значения
        /// </summary>
        public ActionResult MetricDetailsValues(Guid id)
        {
            var storage = GetStorage();
            var metric = storage.Metrics.GetOneById(id);

            var rows = storage.MetricHistory.GetLast(metric.ComponentId, metric.MetricTypeId, 10);

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
            var model = new EventsDetailsModel()
            {
                Id = id
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
        internal ComponentTreeDetailsController(Guid userId) : base(userId) { }

        public ComponentTreeDetailsController(ILogger<ComponentTreeDetailsController> logger) : base(logger)
        {
        }
    }

}