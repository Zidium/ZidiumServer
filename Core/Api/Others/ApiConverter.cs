using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core
{
    public static class ApiConverter
    {
        //public static string GetXmlValue(ExtentionPropertyValue value)
        //{
        //    if (value == null || value.Value == null)
        //    {
        //        return null;
        //    }
        //    if (value.DataType == DataType.Binary)
        //    {
        //        return Convert.ToBase64String((byte[])value.Value);
        //    }
        //    if (value.DataType == DataType.Boolean)
        //    {
        //        return XmlConvert.ToString((bool)value.Value);
        //    }
        //    if (value.DataType == DataType.DateTime)
        //    {
        //        return XmlConvert.ToString((DateTime)value.Value, XmlDateTimeSerializationMode.Local);
        //    }
        //    if (value.DataType == DataType.Double)
        //    {
        //        return XmlConvert.ToString((Double)value.Value);
        //    }
        //    if (value.DataType == DataType.Guid)
        //    {
        //        return XmlConvert.ToString((Guid)value.Value);
        //    }
        //    if (value.DataType == DataType.Int32)
        //    {
        //        return XmlConvert.ToString((Int32)value.Value);
        //    }
        //    if (value.DataType == DataType.Int64)
        //    {
        //        return XmlConvert.ToString((Int64)value.Value);
        //    }
        //    if (value.DataType == DataType.String)
        //    {
        //        return (string)value.Value;
        //    }
        //    if (value.DataType == DataType.Unknown)
        //    {
        //        return (string)value.Value;
        //    }
        //    throw new Exception("Неизвестное значение DataType: " + value.DataType);
        //}

        //public static ExtentionProperty GetExtentionPropertyFromXml(string name, string type, string value)
        //{
        //    if (string.IsNullOrEmpty(name))
        //    {
        //        throw new Exception("Не задано имя свойства");
        //    }
        //    var typeEnum = GetDataType(type);
        //    object valueObj = null;
        //    if (string.IsNullOrEmpty(value) == false)
        //    {
        //        if (typeEnum == DataType.Binary)
        //        {
        //            valueObj = Convert.FromBase64String(value);
        //        }
        //        else if (typeEnum == DataType.Boolean)
        //        {
        //            valueObj = XmlConvert.ToBoolean(value);
        //        }
        //        else if (typeEnum == DataType.DateTime)
        //        {
        //            valueObj = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Local);//todo точно такое?
        //        }
        //        else if (typeEnum == DataType.Double)
        //        {
        //            valueObj = XmlConvert.ToDouble(value);
        //        }
        //        else if (typeEnum == DataType.Int32)
        //        {
        //            valueObj = XmlConvert.ToInt32(value);
        //        }
        //        else if (typeEnum == DataType.Guid)
        //        {
        //            valueObj = XmlConvert.ToGuid(value);
        //        }
        //        else if (typeEnum == DataType.Int64)
        //        {
        //            valueObj = XmlConvert.ToInt64(value);
        //        }
        //        else if (typeEnum == DataType.String)
        //        {
        //            valueObj = value;
        //        }
        //        else if (typeEnum == DataType.Unknown)
        //        {
        //            valueObj = value;
        //        }
        //        else
        //        {
        //            throw new Exception("Неизвестное значение DataType: " + typeEnum);
        //        }
        //    }
        //    return new ExtentionProperty()
        //    {
        //        Name = name,
        //        Value = new ExtentionPropertyValue()
        //        {
        //            Value = valueObj,
        //            DataType = typeEnum
        //        }
        //    };
        //}

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

        public static SubscriptionInfo GetSubscriptionInfo(Subscription subscription)
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
            };
        }

        public static WebLogConfig GetLogConfig(LogConfig config)
        {
            return new WebLogConfig()
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

        public static LogRow GetLogRow(Log log)
        {
            if (log == null)
            {
                return null;
            }
            var logRow = new LogRow()
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
                logRow.Properties = GetExtentionProperties(log.Parameters);
            }
            return logRow;
        }

        public static Log GetLog(Guid componentId, SendLogData message)
        {
            if (message.Date == null)
            {
                message.Date = DateTime.Now;
            }
            if (message.Level == null)
            {
                message.Level = LogLevel.Info;
            }
            var log = new Log()
            {
                Id = Guid.NewGuid(),
                ComponentId = componentId,
                Date = DateTimeHelper.TrimMs(message.Date.Value), // Важно - убираем мс из даты, иначе будет некорректно работать просмотр лога
                Order = message.Order ?? 0,
                Level = message.Level.Value,
                Message = message.Message,
                Context = message.Context,
                ParametersCount = 0,
                Parameters = GetLogProperies(message.Properties)
            };
            if (message.Properties != null)
            {
                log.ParametersCount = message.Properties.Count;
            }
            return log;
        }

        public static MetricInfo GetMetricInfo(Metric metric)
        {
            if (metric == null)
                return null;

            return new MetricInfo()
            {
                ComponentId = metric.ComponentId,
                SystemName = metric.MetricType.SystemName,
                Value = metric.Value,
                BeginDate = metric.BeginDate,
                ActualDate = metric.ActualDate,
                Status = metric.Bulb.Status
            };
        }

        public static MetricInfo GetMetricInfo(
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
            return new MetricInfo()
            {
                ComponentId = metric.ComponentId,
                SystemName = metricType.SystemName,
                Value = metric.Value,
                BeginDate = metric.BeginDate,
                ActualDate = metric.ActualDate,
                Status = bulb.Status
            };
        }

        public static ComponentTypeInfo GetComponentTypeInfo(ComponentType componentType)
        {
            if (componentType == null)
            {
                return null;
            }
            return new ComponentTypeInfo()
            {
                Id = componentType.Id,
                SystemName = componentType.SystemName,
                DisplayName = componentType.DisplayName,
                IsSystem = componentType.IsSystem
            };
        }

        public static List<ComponentInfo> GetComponentInfoList(IEnumerable<Component> components)
        {
            var result = new List<ComponentInfo>();
            foreach (var component in components)
            {
                ComponentInfo info = GetComponentInfo(component);
                if (info != null)
                {
                    result.Add(info);
                }
            }
            return result;
        }

        public static ComponentInfo GetComponentInfo(Component component)
        {
            if (component == null)
            {
                return null;
            }
            var typeDto = GetComponentTypeInfo(component.ComponentType);
            return new ComponentInfo()
            {
                CreatedDate = component.CreatedDate,
                DisplayName = component.DisplayName,
                Id = component.Id,
                ParentId = component.ParentId,
                SystemName = component.SystemName,
                Type = typeDto,
                Version = component.Version,
                Properties = GetExtentionProperties(component.Properties)
            };
        }

        //public static string GetDbString(ExtentionPropertyValue value)
        //{
        //    if (value == null || value.Value == null)
        //    {
        //        return null;
        //    }
        //    if (value.DataType == DataType.Binary)
        //    {
        //        byte[] data = value;
        //        return Convert.ToBase64String(data);
        //    }
        //    if (value.DataType == DataType.Boolean)
        //    {
        //        bool data = value;
        //        return data.ToString(CultureInfo.InvariantCulture);
        //    }
        //    if (value.DataType == DataType.DateTime)
        //    {
        //        DateTime data = value;
        //        return data.ToString(CultureHelper.Russian);
        //    }
        //    if (value.DataType == DataType.Double)
        //    {
        //        double data = value;
        //        return data.ToString(CultureInfo.InvariantCulture);
        //    }
        //    if (value.DataType == DataType.Guid)
        //    {
        //        Guid data = value;
        //        return data.ToString();
        //    }
        //    if (value.DataType == DataType.Int32)
        //    {
        //        int data = value;
        //        return data.ToString(CultureInfo.InvariantCulture);
        //    }
        //    if (value.DataType == DataType.Int64)
        //    {
        //        long data = value;
        //        return data.ToString(CultureInfo.InvariantCulture);
        //    }
        //    if (value.DataType == DataType.String)
        //    {
        //        string data = value;
        //        return data;
        //    }
        //    if (value.DataType == DataType.Unknown)
        //    {
        //        string data = value;
        //        return data;
        //    }
        //    throw new Exception("Неизвестный DataType: " + value.DataType);
        //}

        //public static ExtentionPropertyValue GetExtentionPropertyValueFromDbString(DataType type, string dbStringValue)
        //{
        //    object value = null;
        //    if (dbStringValue != null)
        //    {
        //        if (type == DataType.Binary)
        //        {
        //            value = Convert.FromBase64String(dbStringValue);
        //        }
        //        else if (type == DataType.Boolean)
        //        {
        //            value = bool.Parse(dbStringValue);
        //        }
        //        else if (type == DataType.DateTime)
        //        {
        //            value = DateTime.Parse(dbStringValue, CultureHelper.Russian);
        //        }
        //        else if (type == DataType.Double)
        //        {
        //            value = Double.Parse(dbStringValue, CultureInfo.InvariantCulture);
        //        }
        //        else if (type == DataType.Guid)
        //        {
        //            value = Guid.Parse(dbStringValue);
        //        }
        //        else if (type == DataType.Int32)
        //        {
        //            value = Int32.Parse(dbStringValue, CultureInfo.InvariantCulture);
        //        }
        //        else if (type == DataType.Int64)
        //        {
        //            value = Int64.Parse(dbStringValue, CultureInfo.InvariantCulture);
        //        }
        //        else if (type == DataType.String)
        //        {
        //            value = dbStringValue;
        //        }
        //        else if (type == DataType.Unknown)
        //        {
        //            value = dbStringValue;
        //        }
        //        else
        //        {
        //            throw new Exception("Неизвестный DataType: " + type);
        //        }
        //    }
        //    return new ExtentionPropertyValue()
        //    {
        //        DataType = type,
        //        Value = value
        //    };
        //}

        public static ComponentProperty GetComponentProperty(ExtentionPropertyDto property)
        {
            return new ComponentProperty()
            {
                Id = Guid.NewGuid(),
                Name = property.Name,
                Value = property.Value,
                DataType = property.Type
            };
        }

        public static EventProperty GetEventProperty(ExtentionPropertyDto property)
        {
            return new EventProperty()
            {
                Id = Guid.NewGuid(),
                Name = property.Name,
                Value = property.Value,
                DataType = property.Type
            };
        }

        public static LogProperty GetLogProperty(ExtentionPropertyDto property)
        {
            if (property == null)
            {
                return null;
            }
            return new LogProperty()
            {
                Id = Guid.NewGuid(),
                DataType = property.Type,
                Name = property.Name,
                Value = property.Value
            };
        }

        public static ExtentionPropertyDto GetExtentionProperty(ComponentProperty property)
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

        public static ExtentionPropertyDto GetExtentionProperty(LogProperty property)
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

        public static ExtentionPropertyDto GetExtentionProperty(EventProperty property)
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

        public static List<ExtentionPropertyDto> GetExtentionProperties(IEnumerable<ComponentProperty> properties)
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

        public static List<ExtentionPropertyDto> GetExtentionProperties(IEnumerable<EventProperty> properties)
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

        public static List<ExtentionPropertyDto> GetExtentionProperties(IEnumerable<LogProperty> properties)
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

        public static List<ComponentProperty> GetComponentProperties(List<ExtentionPropertyDto> propertiesDto)
        {
            var properties = new List<ComponentProperty>();
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

        public static List<EventProperty> GetEventProperties(List<ExtentionPropertyDto> propertiesDto)
        {
            var properties = new List<EventProperty>();
            if (propertiesDto != null)
            {
                foreach (var propertyDto in propertiesDto)
                {
                    var property = GetEventProperty(propertyDto);
                    properties.Add(property);
                }
            }
            return properties;
        }

        public static List<LogProperty> GetLogProperies(List<ExtentionPropertyDto> propertiesDto)
        {
            var properties = new List<LogProperty>();
            if (propertiesDto == null)
            {
                return properties;
            }
            foreach (var propertyDto in propertiesDto)
            {
                var property = GetLogProperty(propertyDto);
                properties.Add(property);
            }
            return properties;
        }

        public static EventInfo GetEventInfo(Event eventObj, EventType eventType)
        {
            var result = new EventInfo()
            {
                OwnerId = eventObj.OwnerId,
                Id = eventObj.Id,
                Count = eventObj.Count,
                StartDate = eventObj.StartDate,
                EndDate = eventObj.GetEndDate(DateTime.Now),
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
                Properties = GetExtentionProperties(eventObj.Properties)
            };
            return result;
        }

        public static UnitTestTypeInfo GetUnitTestTypeInfo(IUnitTestTypeCacheReadObject unitTestType)
        {
            if (unitTestType == null)
            {
                return null;
            }
            return new UnitTestTypeInfo()
            {
                Id = unitTestType.Id,
                DisplayName = unitTestType.DisplayName,
                SystemName = unitTestType.SystemName,
                IsSystem = unitTestType.IsSystem,
                ActualTimeSecs = unitTestType.ActualTimeSecs,
                NoSignalColor = unitTestType.NoSignalColor
            };
        }

        public static StatusDataInfo GetStatusDataInfo(Bulb data)
        {
            if (data == null)
            {
                return null;
            }
            return new StatusDataInfo()
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

        public static StatusDataInfo GetStatusDataInfo(IBulbCacheReadObject data)
        {
            if (data == null)
            {
                return null;
            }
            return new StatusDataInfo()
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

        public static GetOrCreateUnitTestResponseData GetOrCreateUnitTestResponseData(IUnitTestCacheReadObject unitTest)
        {
            if (unitTest == null)
            {
                return null;
            }
            return new GetOrCreateUnitTestResponseData()
            {
                Id = unitTest.Id,
                TypeId = unitTest.TypeId,
                SystemName = unitTest.SystemName,
                DisplayName = unitTest.DisplayName
            };
        }

        public static AddPingUnitTestResponseData AddPingUnitTestResponseData(UnitTest unitTest)
        {
            if (unitTest == null || unitTest.PingRule == null)
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
                Host = unitTest.PingRule.Host,
                TimeoutMs = unitTest.PingRule.TimeoutMs,
                Attemps = unitTest.PingRule.Attemps
            };
        }

        public static AddHttpUnitTestResponseData AddHttpUnitTestResponseData(UnitTest unitTest)
        {
            if (unitTest == null || unitTest.HttpRequestUnitTest == null || unitTest.HttpRequestUnitTest.Rules == null)
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
                Rules = unitTest.HttpRequestUnitTest.Rules.Select(t => new AddHttpUnitTestRuleResponseData()
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
