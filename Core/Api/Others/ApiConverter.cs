using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Core
{
    public static class ApiConverter
    {
        public static int? GetSeconds(TimeSpan? timeSpan)
        {
            if (timeSpan == null)
            {
                return null;
            }
            return (int)timeSpan.Value.TotalSeconds;
        }

        public static TimeSpan? FromSeconds(double? interval)
        {
            if (interval == null)
            {
                return null;
            }
            return TimeSpan.FromSeconds(interval.Value);
        }

        public static DataType GetDataType(string dataType)
        {
            if (dataType == "Unknown")
            {
                return DataType.Unknown;
            }
            if (dataType == "Binary")
            {
                return DataType.Binary;
            }
            if (dataType == "Boolean")
            {
                return DataType.Boolean;
            }
            if (dataType == "DateTime")
            {
                return DataType.DateTime;
            }
            if (dataType == "Double")
            {
                return DataType.Double;
            }
            if (dataType == "Int32")
            {
                return DataType.Int32;
            }
            if (dataType == "Int64")
            {
                return DataType.Int64;
            }
            if (dataType == "Guid")
            {
                return DataType.Guid;
            }
            if (dataType == "String")
            {
                return DataType.String;
            }
            throw new Exception("Неизвестное значение DataType: " + dataType);
        }

        public static SubscriptionInfo GetSubscriptionInfo(SubscriptionForRead subscription)
        {
            return new SubscriptionInfo()
            {
                Id = subscription.Id,
                UserId = subscription.UserId,
                ComponentTypeId = subscription.ComponentTypeId,
                IsEnabled = subscription.IsEnabled,
                NotifyBetterStatus = subscription.NotifyBetterStatus,
                Importance = subscription.Importance,
                DurationMinimumInSeconds = subscription.DurationMinimumInSeconds,
                ResendTimeInSeconds = subscription.ResendTimeInSeconds,
                Channel = subscription.Channel,
                SendOnlyInInterval = subscription.SendOnlyInInterval,
                SendIntervalFromHour = subscription.SendIntervalFromHour,
                SendIntervalFromMinute = subscription.SendIntervalFromMinute,
                SendIntervalToHour = subscription.SendIntervalToHour,
                SendIntervalToMinute = subscription.SendIntervalToMinute
            };
        }

        public static GetLogConfigResponseDataDto GetLogConfig(LogConfigForRead config)
        {
            return new GetLogConfigResponseDataDto()
            {
                ComponentId = config.ComponentId,
                Enabled = config.Enabled,
                LastUpdateDate = config.LastUpdateDate,
                IsDebugEnabled = config.IsDebugEnabled,
                IsTraceEnabled = config.IsTraceEnabled,
                IsInfoEnabled = config.IsInfoEnabled,
                IsWarningEnabled = config.IsWarningEnabled,
                IsErrorEnabled = config.IsErrorEnabled,
                IsFatalEnabled = config.IsFatalEnabled
            };
        }

        public static GetLogsResponseDataDto GetLogRow(LogForRead log, LogPropertyForRead[] properties)
        {
            if (log == null)
            {
                return null;
            }
            var logRow = new GetLogsResponseDataDto()
            {
                Id = log.Id,
                Date = log.Date,
                Order = log.Order,
                Level = log.Level,
                Message = log.Message,
                Context = log.Context
            };
            if (log.ParametersCount > 0)
            {
                logRow.Properties = GetExtentionProperties(properties);
            }
            return logRow;
        }

        public static LogForAdd GetLog(Guid componentId, SendLogRequestDataDto message)
        {
            if (message.Date == null)
            {
                message.Date = DateTime.UtcNow;
            }
            if (message.Level == null)
            {
                message.Level = LogLevel.Info;
            }
            var log = new LogForAdd()
            {
                Id = Ulid.NewUlid(),
                ComponentId = componentId,
                Date = DateTimeHelper.TrimMs(message.Date.Value), // Важно - убираем мс из даты, иначе будет некорректно работать просмотр лога
                Order = message.Order ?? 0,
                Level = message.Level.Value,
                Message = message.Message,
                Context = message.Context,
                ParametersCount = 0,
                Properties = GetLogProperies(message.Properties)
            };
            if (message.Properties != null)
            {
                log.ParametersCount = message.Properties.Count;
            }
            return log;
        }

        public static MetricDto GetMetricInfo(MetricForRead metric, MetricTypeForRead metricType, BulbForRead metricBulb)
        {
            if (metric == null)
                return null;

            return new MetricDto()
            {
                ComponentId = metric.ComponentId,
                Name = metricType.SystemName,
                Value = metric.Value,
                BeginDate = metric.BeginDate,
                ActualDate = metric.ActualDate,
                Status = metricBulb.Status
            };
        }

        public static MetricDto GetMetricInfo(
            IMetricCacheReadObject metric,
            IMetricTypeCacheReadObject metricType,
            IBulbCacheReadObject bulb)
        {
            if (metric == null)
            {
                return null;
            }
            if (metricType == null)
            {
                throw new ArgumentNullException("metricType");
            }
            if (bulb == null)
            {
                throw new ArgumentNullException("bulb");
            }
            return new MetricDto()
            {
                Id = metric.Id,
                ComponentId = metric.ComponentId,
                Name = metricType.SystemName,
                Value = metric.Value,
                BeginDate = metric.BeginDate,
                ActualDate = metric.ActualDate,
                Status = bulb.Status
            };
        }

        public static ComponentTypeDto GetComponentTypeInfo(ComponentTypeForRead componentType)
        {
            if (componentType == null)
            {
                return null;
            }
            return new ComponentTypeDto()
            {
                Id = componentType.Id,
                SystemName = componentType.SystemName,
                DisplayName = componentType.DisplayName,
                IsSystem = componentType.IsSystem,
                IsFolder = componentType.Id == SystemComponentType.Folder.Id,
                IsRoot = componentType.Id == SystemComponentType.Root.Id
            };
        }

        public static ComponentDto GetComponentInfo(ComponentForRead component, ComponentPropertyForRead[] properties, ComponentTypeForRead componentType)
        {
            if (component == null)
            {
                return null;
            }
            var typeDto = GetComponentTypeInfo(componentType);
            return new ComponentDto()
            {
                CreatedDate = component.CreatedDate,
                DisplayName = component.DisplayName,
                Id = component.Id,
                ParentId = component.ParentId,
                SystemName = component.SystemName,
                Type = typeDto,
                Version = component.Version,
                Properties = GetExtentionProperties(properties)
            };
        }

        public static ComponentPropertyForAdd GetComponentProperty(ExtentionPropertyDto property)
        {
            return new ComponentPropertyForAdd()
            {
                Id = Ulid.NewUlid(),
                Name = property.Name,
                Value = property.Value,
                DataType = property.Type
            };
        }

        public static EventPropertyForAdd GetEventProperty(ExtentionPropertyDto property)
        {
            return new EventPropertyForAdd()
            {
                Id = Ulid.NewUlid(),
                Name = property.Name,
                Value = property.Value,
                DataType = property.Type
            };
        }

        public static LogPropertyForAdd GetLogProperty(ExtentionPropertyDto property)
        {
            if (property == null)
            {
                return null;
            }
            return new LogPropertyForAdd()
            {
                Id = Ulid.NewUlid(),
                DataType = property.Type,
                Name = property.Name,
                Value = property.Value
            };
        }

        public static ExtentionPropertyDto GetExtentionProperty(ComponentPropertyForRead property)
        {
            if (property == null)
            {
                return null;
            }
            return new ExtentionPropertyDto()
            {
                Name = property.Name,
                Value = property.Value,
                Type = property.DataType
            };
        }

        public static ExtentionPropertyDto GetExtentionProperty(LogPropertyForRead property)
        {
            if (property == null)
            {
                return null;
            }
            return new ExtentionPropertyDto()
            {
                Name = property.Name,
                Value = property.Value,
                Type = property.DataType
            };
        }

        public static ExtentionPropertyDto GetExtentionProperty(EventPropertyForRead property)
        {
            if (property == null)
            {
                return null;
            }
            return new ExtentionPropertyDto()
            {
                Name = property.Name,
                Value = property.Value,
                Type = property.DataType
            };
        }

        public static List<ExtentionPropertyDto> GetExtentionProperties(IEnumerable<ComponentPropertyForRead> properties)
        {
            var dtoProperties = new List<ExtentionPropertyDto>();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var propertyDto = GetExtentionProperty(property);
                    dtoProperties.Add(propertyDto);
                }
            }
            return dtoProperties;
        }

        public static List<ExtentionPropertyDto> GetExtentionProperties(IEnumerable<EventPropertyForRead> properties)
        {
            var dtoProperties = new List<ExtentionPropertyDto>();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var propertyDto = GetExtentionProperty(property);
                    dtoProperties.Add(propertyDto);
                }
            }
            return dtoProperties;
        }

        public static List<ExtentionPropertyDto> GetExtentionProperties(IEnumerable<LogPropertyForRead> properties)
        {
            var dtoProperties = new List<ExtentionPropertyDto>();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var propertyDto = GetExtentionProperty(property);
                    dtoProperties.Add(propertyDto);
                }
            }
            return dtoProperties;
        }

        public static List<ComponentPropertyForAdd> GetComponentProperties(List<ExtentionPropertyDto> propertiesDto)
        {
            var properties = new List<ComponentPropertyForAdd>();
            if (propertiesDto != null)
            {
                foreach (var propertyDto in propertiesDto)
                {
                    var property = GetComponentProperty(propertyDto);
                    properties.Add(property);
                }
            }
            return properties;
        }

        public static EventPropertyForAdd[] GetEventProperties(List<ExtentionPropertyDto> propertiesDto)
        {
            return propertiesDto.Select(GetEventProperty).ToArray();
        }

        public static LogPropertyForAdd[] GetLogProperies(List<ExtentionPropertyDto> propertiesDto)
        {
            if (propertiesDto == null)
                return new LogPropertyForAdd[0];
            return propertiesDto.Select(GetLogProperty).ToArray();
        }

        public static EventDto GetEventInfo(EventForRead eventObj, EventTypeForRead eventType, EventPropertyForRead[] properties)
        {
            var result = new EventDto()
            {
                OwnerId = eventObj.OwnerId,
                Id = eventObj.Id,
                Count = eventObj.Count,
                StartDate = eventObj.StartDate,
                EndDate = eventObj.GetEndDate(DateTime.UtcNow),
                JoinKeyHash = eventObj.JoinKeyHash,
                Importance = eventObj.Importance,
                TypeId = eventObj.EventTypeId,
                TypeSystemName = eventType.SystemName,
                TypeDisplayName = eventType.DisplayName,
                TypeCode = eventType.Code,
                Message = eventObj.Message,
                Version = eventObj.Version,
                Category = eventObj.Category,
                IsUserHandled = eventObj.IsUserHandled,
                LastStatusEventId = eventObj.LastStatusEventId,
                Properties = GetExtentionProperties(properties)
            };
            return result;
        }

        public static UnitTestTypeDto GetUnitTestTypeInfo(IUnitTestTypeCacheReadObject unitTestType)
        {
            if (unitTestType == null)
            {
                return null;
            }
            return new UnitTestTypeDto()
            {
                Id = unitTestType.Id,
                DisplayName = unitTestType.DisplayName,
                SystemName = unitTestType.SystemName,
                IsSystem = unitTestType.IsSystem,
                ActualTimeSecs = unitTestType.ActualTimeSecs,
                NoSignalColor = unitTestType.NoSignalColor
            };
        }

        public static StateDataDto GetStateDataInfo(BulbForRead data)
        {
            if (data == null)
            {
                return null;
            }
            return new StateDataDto()
            {
                Id = data.Id,
                OwnerId = data.GetOwnerId(),
                ActualDate = data.ActualDate,
                EndDate = data.EndDate,
                Message = data.Message,
                HasSignal = data.HasSignal,
                StartDate = data.StartDate,
                Status = data.Status
            };
        }

        public static StateDataDto GetStateDataInfo(IBulbCacheReadObject data)
        {
            if (data == null)
            {
                return null;
            }
            return new StateDataDto()
            {
                Id = data.Id,
                OwnerId = data.GetOwnerId(),
                ActualDate = data.ActualDate,
                EndDate = data.EndDate,
                Message = data.Message,
                HasSignal = data.HasSignal,
                StartDate = data.StartDate,
                Status = data.Status,
                StatusEventId = data.StatusEventId
            };
        }

        public static UnitTestDto GetOrCreateUnitTestResponseData(IUnitTestCacheReadObject unitTest)
        {
            if (unitTest == null)
            {
                return null;
            }
            return new UnitTestDto()
            {
                Id = unitTest.Id,
                TypeId = unitTest.TypeId,
                SystemName = unitTest.SystemName,
                DisplayName = unitTest.DisplayName
            };
        }

        public static AddPingUnitTestResponseData AddPingUnitTestResponseData(UnitTestForRead unitTest, UnitTestPingRuleForRead rule)
        {
            if (unitTest == null || rule == null)
            {
                return null;
            }
            return new AddPingUnitTestResponseData()
            {
                Id = unitTest.Id,
                ComponentId = unitTest.ComponentId,
                SystemName = unitTest.SystemName,
                DisplayName = unitTest.DisplayName,
                PeriodSeconds = unitTest.PeriodSeconds ?? 0,
                ErrorColor = unitTest.ErrorColor,
                Host = rule.Host,
                TimeoutMs = rule.TimeoutMs,
                AttempMax = unitTest.AttempMax
            };
        }

        public static AddHttpUnitTestResponseData AddHttpUnitTestResponseData(UnitTestForRead unitTest, HttpRequestUnitTestRuleForRead[] rules)
        {
            if (unitTest == null || rules == null)
            {
                return null;
            }
            return new AddHttpUnitTestResponseData()
            {
                Id = unitTest.Id,
                ComponentId = unitTest.ComponentId,
                SystemName = unitTest.SystemName,
                DisplayName = unitTest.DisplayName,
                PeriodSeconds = unitTest.PeriodSeconds ?? 0,
                ErrorColor = unitTest.ErrorColor,
                AttempMax = unitTest.AttempMax,
                Rules = rules.Select(t => new AddHttpUnitTestRuleResponseData()
                {
                    SortNumber = t.SortNumber,
                    DisplayName = t.DisplayName,
                    Url = t.Url,
                    Method = t.Method,
                    ResponseCode = t.ResponseCode,
                    MaxResponseSize = t.MaxResponseSize,
                    SuccessHtml = t.SuccessHtml,
                    ErrorHtml = t.ErrorHtml,
                    TimeoutSeconds = t.TimeoutSeconds
                }).ToArray()
            };
        }
    }
}
