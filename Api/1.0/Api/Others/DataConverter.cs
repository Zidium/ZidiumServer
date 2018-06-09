using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Zidium.Api.Dto;

namespace Zidium.Api.Common
{
    public static class DataConverter
    {
        public static string GetXmlValue(ExtentionPropertyValue value)
        {
            if (value == null || value.Value==null)
            {
                return null;
            }
            if (value.DataType == DataType.Binary)
            {
                return Convert.ToBase64String((byte[]) value.Value);
            }
            if (value.DataType == DataType.Boolean)
            {
                return XmlConvert.ToString((bool)value.Value);
            }
            if (value.DataType == DataType.DateTime)
            {
                return XmlConvert.ToString((DateTime)value.Value, XmlDateTimeSerializationMode.Local);
            }
            if (value.DataType == DataType.Double)
            {
                return XmlConvert.ToString((Double)value.Value);
            }
            if (value.DataType == DataType.Int32)
            {
                return XmlConvert.ToString((Int32)value.Value);
            }
            if (value.DataType == DataType.Int64)
            {
                return XmlConvert.ToString((Int64)value.Value);
            }
            if (value.DataType == DataType.String)
            {
                return (string)value.Value;
            }
            if (value.DataType == DataType.Guid)
            {
                return XmlConvert.ToString((Guid) value.Value);
            }
            if (value.DataType == DataType.Unknown)
            {
                return (string)value.Value;
            }
            throw new Exception("Неизвестное значение DataType: " + value.DataType);
        }

        public static ExtentionPropertyCollection GetExtentionPropertyCollection(List<ExtentionPropertyDto> propertyDtos)
        {
            if (propertyDtos == null)
            {
                return new ExtentionPropertyCollection();
            }
            var collection = new ExtentionPropertyCollection();
            foreach (var propertyDto in propertyDtos)
            {
                if (propertyDto != null && propertyDto.Name != null)
                {
                    var property = GetExtentionPropertyFromDto(propertyDto);
                    if (property != null)
                    {
                        collection.Add(property);
                    }
                }
            }
            return collection;
        }

        public static ExtentionProperty GetExtentionPropertyFromDto(ExtentionPropertyDto propertyDto)
        {
            if (propertyDto == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(propertyDto.Name))
            {
                return null;
            }
            var typeEnum = GetDataType(propertyDto.Type);
            object valueObj = null;
            if (string.IsNullOrEmpty(propertyDto.Value) == false)
            {
                if (typeEnum == DataType.Binary)
                {
                    valueObj = Convert.FromBase64String(propertyDto.Value);
                }
                else if (typeEnum == DataType.Boolean)
                {
                    valueObj = XmlConvert.ToBoolean(propertyDto.Value);
                }
                else if (typeEnum == DataType.DateTime)
                {
                    valueObj = XmlConvert.ToDateTime(propertyDto.Value, XmlDateTimeSerializationMode.Local);
                }
                else if (typeEnum == DataType.Double)
                {
                    valueObj = XmlConvert.ToDouble(propertyDto.Value);
                }
                else if (typeEnum == DataType.Int32)
                {
                    valueObj = XmlConvert.ToInt32(propertyDto.Value);
                }
                else if (typeEnum == DataType.Int64)
                {
                    valueObj = XmlConvert.ToInt64(propertyDto.Value);
                }
                else if (typeEnum == DataType.String)
                {
                    valueObj = propertyDto.Value;
                }
                else if (typeEnum == DataType.Guid)
                {
                    valueObj = XmlConvert.ToGuid(propertyDto.Value);
                }
                else if (typeEnum == DataType.Unknown)
                {
                    valueObj = propertyDto.Value;
                }
                else
                {
                    throw new Exception("Неизвестное значение DataType: " + typeEnum);
                }
            }
            return new ExtentionProperty(propertyDto.Name)
            {
                Value = new ExtentionPropertyValue()
                {
                    Value = valueObj,
                    DataType = typeEnum
                }
            };
        }

        public static StatusDataInfo GetStatusDataInfo(StateDataDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            return new StatusDataInfo()
            {
                Id = dto.Id,
                ActualDate = dto.ActualDate,
                EndDate = dto.EndDate,
                HasSignal = dto.HasSignal,
                Message = dto.Message,
                OwnerId = dto.OwnerId,
                StartDate = dto.StartDate,
                Status = dto.Status,
                DisableComment = dto.DisableComment,
                DisableToDate = dto.DisableToDate
            };
        }

        public static SendEventRequestDtoData GetSendEventRequestDtoData(SendEventData data)
        {
            if (data == null)
            {
                return null;
            }
            return new SendEventRequestDtoData()
            {
                Category = data.Category,
                ComponentId = data.ComponentId,
                Count = data.Count,
                Importance = data.Importance,
                JoinIntervalSeconds = GetSeconds(data.JoinInterval),
                JoinKey = data.JoinKey,
                Message = data.Message,
                StartDate = data.StartDate,
                TypeCode = data.TypeCode,
                TypeDisplayName = data.TypeDisplayName,
                TypeSystemName = data.TypeSystemName,
                Version = data.Version,
                Properties = GetExtentionPropertyDtos(data.Properties)
            };
        }

        public static SendEventResponseData GetsSendEventResponseDtoData(SendEventResponseDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new SendEventResponseData()
            {
                EventId = data.EventId,
                EventTypeId = data.EventTypeId
            };
        }
        
        public static SendEventRequestDtoData GetSendEventData(SendEventBase data)
        {
            return new SendEventRequestDtoData()
            {
                ComponentId = data.ComponentControl.Info.Id,
                Category = data.EventCategory,
                StartDate = data.StartDate,
                Importance = data.Importance,
                JoinIntervalSeconds = GetSeconds(data.JoinInterval),
                JoinKey = data.JoinKey,
                Message = data.Message,
                TypeDisplayName = data.TypeDisplayName,
                TypeSystemName = data.TypeSystemName,
                TypeCode = data.TypeCode,
                Version = data.Version,
                Properties = GetExtentionPropertyDtos(data.Properties),
                Count = data.Count
            };
        }

        public static SendMetricRequestDtoData GetSendMetricRequestDtoData(Guid componentId, SendMetricData data)
        {
            if (data == null)
            {
                return null;
            }
            return new SendMetricRequestDtoData()
            {
                ActualIntervalSecs = GetSeconds(data.ActualInterval),
                ComponentId = componentId,
                Name = data.Name,
                Value = data.Value
            };
        }

        public static List<SendMetricRequestDtoData> GetSendMetricRequestDtoDataList(Guid componentId, List<SendMetricData> data)
        {
            if (data == null)
            {
                return null;
            }
            return data.Select(x=>GetSendMetricRequestDtoData(componentId, x)).ToList();
        }

        public static UnitTestTypeInfo GetUnitTestTypeInfo(UnitTestTypeDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            return new UnitTestTypeInfo()
            {
                Id = dto.Id,
                DisplayName = dto.DisplayName,
                IsSystem = dto.IsSystem,
                SystemName = dto.SystemName
            };
        }

        public static UnitTestInfo GetUnitTestInfo(UnitTestDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            return new UnitTestInfo(dto);
        }

        public static GetMetricsHistoryRequestDtoData GetMetricsHistoryRequestDtoData(Guid componentId,
            GetMetricsHistoryFilter filter)
        {
            if (filter == null)
            {
                return null;
            }
            return new GetMetricsHistoryRequestDtoData()
            {
                ComponentId = componentId,
                From = filter.From,
                Name = filter.Name,
                MaxCount = filter.MaxCount,
                To = filter.To
            };
        }

        public static List<MetricInfo> GetMetricInfoList(List<MetricDto> dto)
        {
            if (dto == null)
            {
                return null;
            }
            return dto.Select(t => GetMetricInfo(t)).ToList(); // CF
        }

        public static MetricInfo GetMetricInfo(MetricDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            return new MetricInfo()
            {
                ActualDate = dto.ActualDate,
                BeginDate = dto.BeginDate,
                ComponentId = dto.ComponentId,
                Name = dto.Name,
                Status = dto.Status,
                Value = dto.Value
            };
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
            if (dataType == "Guid")
            {
                return DataType.Guid;
            }
            if (dataType == "Int32")
            {
                return DataType.Int32;
            }
            if (dataType == "Int64")
            {
                return DataType.Int64;
            }
            if (dataType == "String")
            {
                return DataType.String;
            }
            throw new Exception("Неизвестное значение DataType: " + dataType);
        }

        public static List<ExtentionPropertyDto> GetExtentionPropertyDtos(ExtentionPropertyCollection collection)
        {
            if (collection == null)
            {
                return null;
            }
            return collection.Select(x=>new ExtentionPropertyDto()
            {
                Name = x.Name,
                Value = GetXmlValue(x.Value),
                Type = x.Value.DataType.ToString()
            }).ToList();
        }

        public static TimeSpan? GetTimeSpanFromSeconds(double? seconds)
        {
            if (seconds == null)
            {
                return null;
            }
            return TimeSpan.FromSeconds(seconds.Value);
        }

        public static double? GetSeconds(TimeSpan? timeSpan)
        {
            if (timeSpan == null)
            {
                return null;
            }
            return Math.Round(timeSpan.Value.TotalSeconds);
        }

        public static SendUnitTestResultRequestDtoData GetSendUnitTestResultRequestDtoData(Guid unitTestId, SendUnitTestResultData data)
        {
            return new SendUnitTestResultRequestDtoData()
            {
                UnitTestId = unitTestId,
                ActualIntervalSeconds = GetSeconds(data.ActualInterval),
                Message = data.Message,
                Result = data.Result,
                ReasonCode = data.ReasonCode,
                Properties = GetExtentionPropertyDtos(data.Properties)
            };
        }

        public static UpdateComponentRequestDtoData GetUpdateComponentRequestDtoData(Guid componentId, UpdateComponentData data)
        {
            if (data == null)
            {
                return null;
            }
            return new UpdateComponentRequestDtoData()
            {
                SystemName = data.SystemName,
                DisplayName = data.DisplayName,
                Id = componentId,
                ParentId = data.ParentId,
                TypeId = data.TypeId,
                Version = data.Version,
                Properties = GetExtentionPropertyDtos(data.Properties)
            };
        }

        public static List<EventInfo> GetEventInfoList(List<EventDto> dto)
        {
            if (dto == null)
            {
                return null;
            }
            return dto.Select(t => GetEventInfo(t)).ToList(); //CF
        } 

        public static EventInfo GetEventInfo(EventDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            return new EventInfo(dto);
        }

        public static List<JoinEventDto> GetJoinEventDtoList(List<JoinEventData> data)
        {
            if (data == null)
            {
                return null;
            }
            return data.Select(t => GetJoinEventDto(t)).ToList(); // CF
        }

        public static JoinEventDto GetJoinEventDto(JoinEventData data)
        {
            if (data == null)
            {
                return null;
            }
            return new JoinEventDto()
            {
                EventId = data.EventId,
                ComponentId = data.ComponentId,
                Count = data.Count,
                Importance = data.Importance,
                JoinInterval = GetSeconds(data.JoinInterval),
                JoinKey = data.JoinKey,
                Message = data.Message,
                StartDate = data.StartDate,
                TypeId = data.TypeId,
                Version = data.Version
            };
        }

        public static GetEventsRequestDtoData GetEventsRequestDtoData(GetEventsData data)
        {
            if (data == null)
            {
                return null;
            }
            return new GetEventsRequestDtoData()
            {
                Category = data.Category,
                OwnerId = data.OwnerId,
                From = data.From,
                Importance = data.Importance,
                MaxCount = data.MaxCount,
                SearchText = data.SearthText,
                To = data.To,
                TypeSystemName = data.TypeSystemName
            };
        }

        public static GetEventsRequestDtoData GetEventsRequestData(GetEventsData data)
        {
            if (data == null)
            {
                return null;
            }
            return new GetEventsRequestDtoData()
            {
                Category = data.Category,
                OwnerId = data.OwnerId,
                From = data.From,
                To = data.To,
                Importance = data.Importance,
                MaxCount = data.MaxCount,
                SearchText = data.SearthText,
                TypeSystemName = data.TypeSystemName
            };
        }

        public static GetEchoRequestDtoData GetEchoRequestDtoData(string message)
        {
            return new GetEchoRequestDtoData()
            {
                Message = message
            };
        }

        public static GetOrCreateComponentRequestDtoData GetOrCreateComponentDataDto(Guid parentComponentId, GetOrCreateComponentData data)
        {
            if (data == null)
            {
                return null;
            }
            var result = new GetOrCreateComponentRequestDtoData()
            {
                DisplayName = data.DisplayName,
                ParentComponentId = parentComponentId,
                Properties = GetExtentionPropertyDtos(data.Properties),
                SystemName = data.SystemName,
                Version = data.Version
            };
            if (data.ComponentTypeControl != null)
            {
                result.TypeId = data.ComponentTypeControl.Info.Id;
            }
            return result;
        }

        public static ComponentControlData GetComponentControlData(ComponentControlDataDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            return new ComponentControlData()
            {
                Component = GetComponentInfo(dto.Component),
                WebLogConfig = GetWebLogConfig(dto.WebLogConfig)
            };
        }

        public static GetComponentBySystemNameRequestDtoData GetComponentBySystemNameRequestDtoData(GetComponentBySystemNameData data)
        {
            if (data == null)
            {
                return null;
            }
            return new GetComponentBySystemNameRequestDtoData()
            {
                ParentId = data.ParentId,
                SystemName = data.SystemName
            };
        }

        public static ComponentInfo GetComponentInfo(ComponentDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            return new ComponentInfo(dto);
        }

        public static ComponentTypeInfo GetComponentTypeInfo(ComponentTypeDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            return new ComponentTypeInfo(dto);
        }

        public static List<ComponentInfo> GetComponentInfoList(List<ComponentDto> dto)
        {
            if (dto == null)
            {
                return null;
            }
            return dto.Select(t => GetComponentInfo(t)).ToList(); // CF
        }

        public static WebLogConfig GetWebLogConfig(WebLogConfigDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            return new WebLogConfig()
            {
                Enabled = dto.Enabled,
                IsDebugEnabled = dto.IsDebugEnabled,
                IsErrorEnabled = dto.IsErrorEnabled,
                IsFatalEnabled = dto.IsFatalEnabled,
                IsInfoEnabled = dto.IsInfoEnabled,
                IsTraceEnabled = dto.IsTraceEnabled,
                IsWarningEnabled = dto.IsWarningEnabled,
                LastUpdateDate = dto.LastUpdateDate,
                ComponentId = dto.ComponentId
            };
        }

        public static List<WebLogConfig> GetWebLogConfigList(List<WebLogConfigDto> dto)
        {
            if (dto == null)
            {
                return null;
            }
            return dto.Select(t => GetWebLogConfig(t)).ToList();
        }

        public static GetServerTimeResponseData GetServerTimeResponse(GetServerTimeResponseDtoData dto)
        {
            if (dto == null)
            {
                return null;
            }
            return new GetServerTimeResponseData()
            {
                Date = dto.Date
            };
        }

        public static GetOrCreateUnitTestRequestDtoData GetOrCreateUnitTestRequestDtoData(
            Guid componentId, 
            GetOrCreateUnitTestData data)
        {
            if (data == null)
            {
                return null;
            }
            return new GetOrCreateUnitTestRequestDtoData()
            {
                ComponentId = componentId,
                UnitTestTypeId = data.UnitTestTypeControl.Info.Id,
                SystemName = data.SystemName,
                DisplayName = data.DisplayName
            };
        }

        public static GetOrCreateUnitTestTypeRequestDtoData GetOrCreateUnitTestTypeRequestDtoData(GetOrCreateUnitTestTypeData data)
        {
            if (data == null)
            {
                return null;
            }
            return new GetOrCreateUnitTestTypeRequestDtoData()
            {
                SystemName = data.SystemName,
                DisplayName = data.DisplayName
            };
        }

        public static GetOrCreateComponentTypeRequestDtoData GetOrCreateComponentTypeRequestDtoData(GetOrCreateComponentTypeData data)
        {
            if (data == null)
            {
                return null;
            }
            return new GetOrCreateComponentTypeRequestDtoData()
            {
                SystemName = data.SystemName,
                DisplayName = data.DisplayName
            };
        }

        public static UpdateComponentTypeRequestDtoData GetUpdateComponentTypeRequestDtoData(UpdateComponentTypeData data)
        {
            if (data == null)
            {
                return null;
            }
            return new UpdateComponentTypeRequestDtoData()
            {
                SystemName = data.SystemName,
                DisplayName = data.DisplayName,
                Id = data.Id
            };
        }

        public static SendLogDto[] GetSendLogDtoList(SendLogData[] data)
        {
            if (data == null)
            {
                return null;
            }
            return data.Select(t => GetSendLogDto(t)).ToArray();
        }

        public static SendLogDto GetSendLogDto(SendLogData data)
        {
            if (data == null)
            {
                return null;
            }
            return new SendLogDto()
            {
                ComponentId = data.ComponentId,
                Context = data.Context,
                Date = data.Date,
                Order = data.Order,
                Level = data.Level,
                Message = data.Message,
                Properties = GetExtentionPropertyDtos(data.Properties)
            };
        }

        public static LogInfo GetLogInfo(LogDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            return new LogInfo()
            {
                Id = dto.Id,
                Date = dto.Date,
                Order = dto.Order,
                Level = dto.Level,
                Message = dto.Message,
                Context = dto.Context,
                Properties = GetExtentionPropertyCollection(dto.Properties)
            };
        }

        public static List<LogInfo> GetLogInfoList(List<LogDto> dto)
        {
            if (dto == null)
            {
                return null;
            }
            return dto.Select(t => GetLogInfo(t)).ToList();
        }

        public static GetLogsRequestDtoData GetLogsRequestDtoData(Guid componentId, GetLogsFilter data)
        {
            if (data == null)
            {
                return null;
            }
            return new GetLogsRequestDtoData()
            {
                ComponentId = componentId,
                From = data.From,
                To = data.To,
                Context = data.Context,
                MaxCount = data.MaxCount,
                Levels = data.Levels
            };
        }

        public static GetMetricsHistoryRequestDtoData GetCountersRequestData(Guid componentId, GetMetricsHistoryFilter data)
        {
            if (data == null)
            {
                return null;
            }
            return new GetMetricsHistoryRequestDtoData()
            {
                ComponentId = componentId,
                From = data.From,
                To = data.To,
                MaxCount = data.MaxCount
            };
        }
    }
}
