using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api;
using Zidium.Api.Dto;

namespace ApiAdapter
{
    public static class AdapterDataConverter
    {
        public static List<Zidium.Core.Api.EventImportance> ConvertToCore(IEnumerable<EventImportance> importances)
        {
            if (importances == null)
            {
                return null;
            }
            return importances.Select(x => ConvertToCore(x).Value).ToList();
        }

        public static Zidium.Core.Api.GetEventsRequestData ConvertToCore(GetEventsRequestDtoData filter)
        {
            if (filter == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetEventsRequestData()
            {   
                OwnerId = filter.OwnerId,
                From = filter.From,
                Importance = ConvertToCore(filter.Importance),
                To = filter.To,
                MaxCount = filter.MaxCount,
                SearthText = filter.SearchText,
                TypeSystemName = filter.TypeSystemName,
                Category = ConvertToCore(filter.Category)
            };
        }

        public static Zidium.Core.Api.AccessToken ConvertToCore(AccessTokenDto accessToken)
        {
            if (accessToken == null)
            {
                return null;
            }
            return new Zidium.Core.Api.AccessToken()
            {   
                AccountId = accessToken.AccountId,
                SecretKey = accessToken.SecretKey,
                ProgramName = accessToken.Program,
                IsLocalRequest = false
            };
        }

        public static Zidium.Core.Api.JoinEventData ConvertToCore(JoinEventDto data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.JoinEventData()
            {   
                EventId = data.EventId,
                ComponentId = data.ComponentId,
                JoinKey = data.JoinKey,
                Count = data.Count,
                Importance = ConvertToCore(data.Importance),
                JoinInterval = data.JoinInterval,
                Message = data.Message,
                Version = data.Version,
                StartDate = data.StartDate,
                TypeId = data.TypeId
            };
        }

        public static Zidium.Core.Api.GetEventByIdRequestData ConvertToCore(GetEventByIdRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetEventByIdRequestData()
            {
                EventId = data.EventId
            };
        }

        public static List<Zidium.Core.Api.JoinEventData> ConvertToCore(List<JoinEventDto> joinEventDatas)
        {
            if (joinEventDatas == null)
            {
                return new List<Zidium.Core.Api.JoinEventData>(0);
            }
            return joinEventDatas.Select(ConvertToCore).ToList();
        }

        public static Zidium.Core.Api.DataType ConvertToCore(DataType dataType)
        {
            if (dataType == DataType.Boolean)
            {
                return Zidium.Core.Api.DataType.Boolean;
            }
            if (dataType == DataType.DateTime)
            {
                return Zidium.Core.Api.DataType.DateTime;
            }
            if (dataType == DataType.Double)
            {
                return Zidium.Core.Api.DataType.Double;
            }
            if (dataType == DataType.Int32)
            {
                return Zidium.Core.Api.DataType.Int32;
            }
            if (dataType == DataType.Int64)
            {
                return Zidium.Core.Api.DataType.Int64;
            }
            if (dataType == DataType.Binary)
            {
                return Zidium.Core.Api.DataType.Binary;
            }
            if (dataType == DataType.String)
            {
                return Zidium.Core.Api.DataType.String;
            }
            if (dataType == DataType.Guid)
            {
                return Zidium.Core.Api.DataType.Guid;
            }
            if (dataType == DataType.Unknown)
            {
                return Zidium.Core.Api.DataType.Unknown;
            }
            throw new Exception("Неизвестное значение DataType: " + dataType);
        }

        public static Zidium.Core.Api.DataType ParseCoreDataType(string type)
        {
            if (type == "Boolean")
            {
                return Zidium.Core.Api.DataType.Boolean;
            }
            if (type == "DateTime")
            {
                return Zidium.Core.Api.DataType.DateTime;
            }
            if (type == "Double")
            {
                return Zidium.Core.Api.DataType.Double;
            }
            if (type == "Int32")
            {
                return Zidium.Core.Api.DataType.Int32;
            }
            if (type == "Int64")
            {
                return Zidium.Core.Api.DataType.Int64;
            }
            if (type == "Binary")
            {
                return Zidium.Core.Api.DataType.Binary;
            }
            if (type == "String")
            {
                return Zidium.Core.Api.DataType.String;
            }
            if (type == "Guid")
            {
                return Zidium.Core.Api.DataType.Guid;
            }
            return Zidium.Core.Api.DataType.Unknown;
        }

        public static Zidium.Core.Api.UnitTestResult? ConvertToCore(UnitTestResult? data)
        {
            if (data == null)
            {
                return null;
            }
            if (data == UnitTestResult.Unknown)
            {
                return Zidium.Core.Api.UnitTestResult.Unknown;
            }
            if (data == UnitTestResult.Success)
            {
                return Zidium.Core.Api.UnitTestResult.Success;
            }
            if (data == UnitTestResult.Warning)
            {
                return Zidium.Core.Api.UnitTestResult.Warning;
            }
            if (data == UnitTestResult.Alarm)
            {
                return Zidium.Core.Api.UnitTestResult.Alarm;
            }
            throw new Exception("Неизвестное значени UnitTestResult: " + data);
        }

        public static Zidium.Core.Api.GetEchoRequestData ConvertToCore(GetEchoRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetEchoRequestData()
            {
                Message = data.Message
            };
        }

        public static Zidium.Core.Api.GetComponentByIdRequestData ConvertToCore(GetComponentByIdRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetComponentByIdRequestData()
            {
                ComponentId = data.ComponentId
            };
        }

        public static Zidium.Core.Api.SetComponentEnableRequestData ConvertToCore(SetComponentEnableRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.SetComponentEnableRequestData()
            {
                ComponentId = data.ComponentId
            };
        }

        public static Zidium.Core.Api.SetComponentDisableRequestData ConvertToCore(SetComponentDisableRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.SetComponentDisableRequestData()
            {   
                ComponentId = data.ComponentId,
                Comment = data.Comment,
                ToDate = data.ToDate
            };
        }

        public static Zidium.Core.Api.GetComponentTotalStateRequestData ConvertToCore(GetComponentTotalStateRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetComponentTotalStateRequestData()
            {   
                ComponentId = data.ComponentId,
                Recalc = data.Recalc
            };
        }

        public static Zidium.Core.Api.GetComponentInternalStateRequestData ConvertToCore(GetComponentInternalStateRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetComponentInternalStateRequestData()
            {   
                ComponentId = data.ComponentId,
                Recalc = data.Recalc
            };
        }

        public static Zidium.Core.Api.ExtentionPropertyDto ConvertToCore(ExtentionPropertyDto property)
        {
            if (property == null)
            {
                return null;
            }
            var type = ParseCoreDataType(property.Type);
            return new Zidium.Core.Api.ExtentionPropertyDto()
            {   
                Name = property.Name,
                Value = property.Value,
                Type = type
            };
        }

        public static Zidium.Core.Api.GetComponentBySystemNameRequestData ConvertToCore(GetComponentBySystemNameRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetComponentBySystemNameRequestData()
            {   
                SystemName = data.SystemName,
                ParentId = data.ParentId
            };
        }

        public static Zidium.Core.Api.GetChildComponentsRequestData ConvertToCore(GetChildComponentsRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetChildComponentsRequestData()
            {   
                ComponentId = data.ComponentId
            };
        }

        public static List<Zidium.Core.Api.ExtentionPropertyDto> ConvertToCore(List<ExtentionPropertyDto> properties)
        {
            var result = new List<Zidium.Core.Api.ExtentionPropertyDto>();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var newProperty = ConvertToCore(property);
                    result.Add(newProperty);
                }
            }
            return result;
        }

        public static DataType ConvertToApi(Zidium.Core.Api.DataType dataType)
        {
            if (dataType == Zidium.Core.Api.DataType.Boolean)
            {
                return DataType.Boolean;
            }
            if (dataType == Zidium.Core.Api.DataType.DateTime)
            {
                return DataType.DateTime;
            }
            if (dataType == Zidium.Core.Api.DataType.Double)
            {
                return DataType.Double;
            }
            if (dataType == Zidium.Core.Api.DataType.Int32)
            {
                return DataType.Int32;
            }
            if (dataType == Zidium.Core.Api.DataType.Int64)
            {
                return DataType.Int64;
            }
            if (dataType == Zidium.Core.Api.DataType.Binary)
            {
                return DataType.Binary;
            }
            if (dataType == Zidium.Core.Api.DataType.String)
            {
                return DataType.String;
            }
            if (dataType == Zidium.Core.Api.DataType.Guid)
            {
                return DataType.Guid;
            }
            if (dataType == Zidium.Core.Api.DataType.Unknown)
            {
                return DataType.Unknown;
            }
            throw new Exception("Неизвестное значение DataType: " + dataType);
        }

        public static ExtentionPropertyDto ConvertToApi(Zidium.Core.Api.ExtentionPropertyDto property)
        {
            if (property == null)
            {
                return null;
            }
            return new ExtentionPropertyDto()
            {   
                Value = property.Value,
                Name = property.Name,
                Type = property.Type.ToString()
            };
        }

        public static List<ExtentionPropertyDto> ConvertToApi(List<Zidium.Core.Api.ExtentionPropertyDto> properties)
        {
            if (properties == null)
            {
                return null;
            }
            return properties.Select(ConvertToApi).ToList();
        } 

        public static List<EventDto> ConvertToApi(List<Zidium.Core.Api.EventInfo> eventInfos)
        {
            if (eventInfos == null)
            {
                return null;
            }
            return eventInfos.Select(ConvertToApi).ToList();
        }

        public static SendEventResponseDtoData ConvertToApi(Zidium.Core.Api.SendEventResponseData data)
        {
            if (data == null)
            {
                return null;
            }
            return new SendEventResponseDtoData()
            {  
                EventId = data.EventId,
                EventTypeId = data.EventTypeId
            };
        }

        public static EventDto ConvertToApi(Zidium.Core.Api.EventInfo info)
        {
            if (info == null)
            {
                return null;
            }
            return new EventDto()
            {   
                Id = info.Id,
                OwnerId = info.OwnerId,
                Count = info.Count,
                StartDate = info.StartDate,
                EndDate = info.EndDate,
                Importance = ConvertToApi(info.Importance),
                JoinKeyHash = info.JoinKeyHash,
                Message = info.Message,
                TypeId = info.TypeId,
                TypeDisplayName = info.TypeDisplayName,
                TypeSystemName = info.TypeSystemName,
                TypeCode = info.TypeCode,
                Version = info.Version,
                Category = ConvertToApi(info.Category),
                IsUserHandled = info.IsUserHandled,
                Properties = ConvertToApi(info.Properties)
            };
        }

        public static ComponentDto ConvertToApi(Zidium.Core.Api.ComponentInfo info)
        {
            if (info == null)
            {
                return null;
            }
            var result = new ComponentDto
            {   
                CreatedDate = info.CreatedDate,
                DisplayName = info.DisplayName,
                Id = info.Id,
                ParentId = info.ParentId,
                SystemName = info.SystemName,
                Type = ConvertToApi(info.Type),
                Version = info.Version,
                Properties = ConvertToApi(info.Properties)
            };

            return result;
        }

        public static List<ComponentDto> ConvertToApi(List<Zidium.Core.Api.ComponentInfo> infos)
        {
            if (infos == null)
            {
                return null;
            }
            return infos.Select(ConvertToApi).ToList();
        }

        public static Zidium.Core.Api.GetOrCreateComponentRequestData ConvertToCore(GetOrCreateComponentRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            var newMessage = new Zidium.Core.Api.GetOrCreateComponentRequestData
            {   
                ParentComponentId = data.ParentComponentId,
                DisplayName = data.DisplayName,
                SystemName = data.SystemName,
                TypeId = data.TypeId,
                Version = data.Version,
                Properties = ConvertToCore(data.Properties)
            };
            return newMessage;
        }

        public static Zidium.Core.Api.GetOrCreateComponentRequestData ConvertToCore(GetOrAddComponentRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            var newMessage = new Zidium.Core.Api.GetOrCreateComponentRequestData
            {   
                ParentComponentId = data.ParentComponentId,
                DisplayName = data.DisplayName,
                SystemName = data.SystemName,
                TypeId = data.TypeId,
                Version = data.Version,
                Properties = ConvertToCore(data.Properties)
            };
            return newMessage;
        }
        public static Zidium.Core.Api.GetComponentControlByIdRequestData ConvertToCore(GetComponentControlByIdRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            var newMessage = new Zidium.Core.Api.GetComponentControlByIdRequestData
            {
                ComponentId = data.ComponentId
            };
            return newMessage;
        }

        public static Zidium.Core.Api.UpdateComponentRequestData ConvertToCore(UpdateComponentRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            var newMessage = new Zidium.Core.Api.UpdateComponentRequestData
            {   
                Id = data.Id,
                ParentId = data.ParentId,
                Version = data.Version,
                DisplayName = data.DisplayName,
                SystemName = data.SystemName,
                TypeId = data.TypeId,
                Properties = ConvertToCore(data.Properties)
            };
            return newMessage;
        }

        public static Zidium.Core.Api.DeleteComponentRequestData ConvertToCore(DeleteComponentRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            var newMessage = new Zidium.Core.Api.DeleteComponentRequestData
            {
                ComponentId = data.ComponentId
            };
            return newMessage;
        }

        public static ComponentControlDataDto ConvertToApi(Zidium.Core.Api.ComponentControlData controlData)
        {
            if (controlData == null)
            {
                return null;
            }
            return new ComponentControlDataDto()
            {   
                Component = ConvertToApi(controlData.Component),
                WebLogConfig = ConvertToApi(controlData.WebLogConfig)
            };
        }

        public static MonitoringStatus ConvertToApi(Zidium.Core.Api.MonitoringStatus status)
        {
            if (status == Zidium.Core.Api.MonitoringStatus.Unknown)
            {
                return MonitoringStatus.Unknown;
            }
            if (status == Zidium.Core.Api.MonitoringStatus.Disabled)
            {
                return MonitoringStatus.Disabled;
            }
            if (status == Zidium.Core.Api.MonitoringStatus.Success)
            {
                return MonitoringStatus.Success;
            }
            if (status == Zidium.Core.Api.MonitoringStatus.Warning)
            {
                return MonitoringStatus.Warning;
            }
            if (status == Zidium.Core.Api.MonitoringStatus.Alarm)
            {
                return MonitoringStatus.Alarm;
            }
            throw new Exception("неизвестное значение ComponentStatus: " + status);
        }

        public static EventCategory ConvertToApi(Zidium.Core.Api.EventCategory category)
        {
            if (category == Zidium.Core.Api.EventCategory.ApplicationError)
            {
                return EventCategory.ApplicationError;
            }
            if (category == Zidium.Core.Api.EventCategory.ComponentEvent)
            {
                return EventCategory.ComponentEvent;
            }
            if (category == Zidium.Core.Api.EventCategory.UnitTestStatus)
            {
                return EventCategory.UnitTestStatus;
            }
            if (category == Zidium.Core.Api.EventCategory.UnitTestResult)
            {
                return EventCategory.UnitTestResult;
            }
            if (category == Zidium.Core.Api.EventCategory.MetricStatus)
            {
                return EventCategory.MetricStatus;
            }
            //if (category == Zidium.Core.Api.EventCategory.MetricResult)
            //{
            //    return EventCategory.MetricResult;
            //}
            if (category == Zidium.Core.Api.EventCategory.ComponentEventsStatus)
            {
                return EventCategory.ComponentEventsStatus;
            }
            if (category == Zidium.Core.Api.EventCategory.ComponentUnitTestsStatus)
            {
                return EventCategory.ComponentUnitTestsStatus;
            }
            if (category == Zidium.Core.Api.EventCategory.ComponentMetricsStatus)
            {
                return EventCategory.ComponentMetricsStatus;
            }
            if (category == Zidium.Core.Api.EventCategory.ComponentChildsStatus)
            {
                return EventCategory.ComponentChildsStatus;
            }
            if (category == Zidium.Core.Api.EventCategory.ComponentInternalStatus)
            {
                return EventCategory.ComponentInternalStatus;
            }
            if (category == Zidium.Core.Api.EventCategory.ComponentExternalStatus)
            {
                return EventCategory.ComponentExternalStatus;
            }
            throw new Exception("неизвестное значение EventCategory: " + category);
        }

        public static Zidium.Core.Api.SendUnitTestResultRequestData ConvertToCore(SendUnitTestResultRequestDtoData message)
        {
            if (message == null)
            {
                return null;
            }
            return new Zidium.Core.Api.SendUnitTestResultRequestData()
            {  
                UnitTestId = message.UnitTestId,
                ActualIntervalSeconds = message.ActualIntervalSeconds,
                Message = message.Message,
                Result = ConvertToCore(message.Result),
                ReasonCode = message.ReasonCode,
                Properties = ConvertToCore(message.Properties)
            };
        }

        public static Zidium.Core.Api.SendUnitTestResultRequestData[] ConvertToCore(SendUnitTestResultRequestDtoData[] messages)
        {
            return messages.Select(ConvertToCore).ToArray();
        }

        public static Zidium.Core.Api.GetUnitTestStateRequestData ConvertToCore(GetUnitTestStateRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetUnitTestStateRequestData()
            {
                UnitTestId = data.UnitTestId
            };
        }

        public static Zidium.Core.Api.GetOrCreateUnitTestRequestData ConvertToCore(GetOrCreateUnitTestRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetOrCreateUnitTestRequestData()
            {    
                ComponentId = data.ComponentId,
                SystemName = data.SystemName,
                DisplayName = data.DisplayName,
                UnitTestTypeId = data.UnitTestTypeId
            };
        }

        public static Zidium.Core.Api.SetUnitTestEnableRequestData ConvertToCore(SetUnitTestEnableRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.SetUnitTestEnableRequestData()
            {
                UnitTestId = data.UnitTestId
            };
        }

        public static Zidium.Core.Api.SetUnitTestDisableRequestData ConvertToCore(SetUnitTestDisableRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.SetUnitTestDisableRequestData()
            {   
                UnitTestId = data.UnitTestId,
                Comment = data.Comment,
                ToDate = data.ToDate
            };
        }

        public static Zidium.Core.Api.GetOrCreateUnitTestTypeRequestData ConvertToCore(GetOrCreateUnitTestTypeRequestDtoData message)
        {
            if (message == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetOrCreateUnitTestTypeRequestData()
            {   
                SystemName = message.SystemName,
                DisplayName = message.DisplayName
            };
        }

        public static GetServerTimeResponseDtoData ConvertToApi(Zidium.Core.Api.GetServerTimeResponseData data)
        {
            if (data == null)
            {
                return null;
            }
            return new GetServerTimeResponseDtoData()
            {
                Date = data.Date
            };
        }

        public static UnitTestDto ConvertToApi(Zidium.Core.Api.GetOrCreateUnitTestResponseData data)
        {
            if (data == null)
            {
                return null;
            }
            return new UnitTestDto()
            {   
                Id = data.Id,
                TypeId = data.TypeId,
                SystemName = data.SystemName,
                DisplayName = data.DisplayName
            };
        }

        public static StateDataDto ConvertToApi(Zidium.Core.Api.StatusDataInfo info)
        {
            if (info == null)
            {
                return null;
            }
            return new StateDataDto()
            {   
                Id = info.Id,
                OwnerId = info.OwnerId,
                ActualDate = info.ActualDate,
                EndDate = info.EndDate,
                Message = info.Message,
                HasSignal = info.HasSignal,
                StartDate = info.StartDate,
                Status = ConvertToApi(info.Status),
                DisableComment = info.DisableComment,
                DisableToDate = info.DisableToDate
            };
        }

        public static UnitTestTypeDto ConvertToApi(Zidium.Core.Api.UnitTestTypeInfo type)
        {
            if (type == null)
            {
                return null;
            }
            return new UnitTestTypeDto()
            {   
                Id = type.Id,
                DisplayName = type.DisplayName,
                SystemName = type.SystemName,
                IsSystem = type.IsSystem
            };
        }

        public static Zidium.Core.Api.GetLogConfigRequestData ConvertToCore(GetLogConfigRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetLogConfigRequestData()
            {
                ComponentId = data.ComponentId
            };
        }

        public static Zidium.Core.Api.GetChangedWebLogConfigsRequestData ConvertToCore(GetChangedWebLogConfigsRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            var result = new Zidium.Core.Api.GetChangedWebLogConfigsRequestData()
            {
                LastUpdateDate = data.LastUpdateDate,
                ComponentIds = new List<Guid>()
            };
            if (data.ComponentIds != null)
            {
                result.ComponentIds.AddRange(data.ComponentIds);
            }
            return result;
        }

        public static Zidium.Core.Api.GetComponentTypeRequestData ConvertToCore(GetComponentTypeRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetComponentTypeRequestData()
            {
                Id = data.Id,
                SystemName = data.SystemName
            };
        }

        public static Zidium.Core.Api.GetOrCreateComponentTypeRequestData ConvertToCore(GetOrCreateComponentTypeRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.GetOrCreateComponentTypeRequestData()
            {
                DisplayName = data.DisplayName,
                SystemName = data.SystemName
            };
        }

        public static Zidium.Core.Api.UpdateComponentTypeData ConvertToCore(UpdateComponentTypeRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            return new Zidium.Core.Api.UpdateComponentTypeData()
            {
                DisplayName = data.DisplayName,
                SystemName = data.SystemName,
                Id = data.Id
            };
        }

        public static ComponentTypeDto ConvertToApi(Zidium.Core.Api.ComponentTypeInfo type)
        {
            if (type == null)
            {
                return null;
            }
            return new ComponentTypeDto()
            {
                SystemName = type.SystemName,
                DisplayName = type.DisplayName,
                Id = type.Id,
                IsSystem = type.IsSystem
            };
        }

        public static Zidium.Core.Api.SendLogData[] ConvertToCore(SendLogDto[] messages)
        {
            if (messages == null)
            {
                return null;
            }
            return messages.Select(ConvertToCore).ToArray();
        }

        public static Zidium.Core.Api.SendLogData ConvertToCore(SendLogDto message)
        {
            if (message == null)
            {
                return null;
            }
            return new Zidium.Core.Api.SendLogData()
            {   
                ComponentId = message.ComponentId,
                Date = message.Date,
                Order = message.Order,
                Level = ConvertToCore(message.Level),
                Message = message.Message,
                Context = message.Context,
                Properties = ConvertToCore(message.Properties)
            };
        }

        public static WebLogConfigDto ConvertToApi(Zidium.Core.Api.WebLogConfig config)
        {
            if (config == null)
            {
                return null;
            }
            return new WebLogConfigDto()
            {   
                ComponentId = config.ComponentId,
                LastUpdateDate = config.LastUpdateDate,
                Enabled = config.Enabled,
                IsDebugEnabled = config.IsDebugEnabled,
                IsTraceEnabled = config.IsTraceEnabled,
                IsInfoEnabled = config.IsInfoEnabled,
                IsWarningEnabled = config.IsWarningEnabled,
                IsErrorEnabled = config.IsErrorEnabled,
                IsFatalEnabled = config.IsFatalEnabled
            };
        }

        public static List<LogDto> ConvertToApi(List<Zidium.Core.Api.LogRow> logRows)
        {
            if (logRows == null)
            {
                return null;
            }
            return logRows.Select(ConvertToApi).ToList();
        }

        public static LogDto ConvertToApi(Zidium.Core.Api.LogRow logRow)
        {
            if (logRow == null)
            {
                return null;
            }
            return new LogDto()
            {
                Date = logRow.Date,
                Order = logRow.Order,
                Id = logRow.Id,
                Level = ConvertToApi(logRow.Level),
                Message = logRow.Message,
                Context = logRow.Context,
                Properties = ConvertToApi(logRow.Properties)
            };
        }

        public static List<Zidium.Core.Api.LogLevel> ConvertToCore(List<LogLevel> levels)
        {
            if (levels == null)
            {
                return null;
            }
            return levels.Select(x=>ConvertToCore(x).Value).ToList();
        }

        public static Zidium.Core.Api.GetLogsRequestData ConvertToCore(GetLogsRequestDtoData filter)
        {
            if (filter == null)
            {
                return null;
            }
            var result = new Zidium.Core.Api.GetLogsRequestData()
            {
                ComponentId = filter.ComponentId,
                From = filter.From,
                To = filter.To,
                MaxCount = filter.MaxCount,
                Context = filter.Context,
                Levels = ConvertToCore(filter.Levels)
            };
            return result;
        }

        public static Zidium.Core.Api.SendEventData ConvertToCore(SendEventRequestDtoData sendEventDataDto)
        {
            if (sendEventDataDto == null)
            {
                return null;
            }
            var message = new Zidium.Core.Api.SendEventData()
            {
                ComponentId = sendEventDataDto.ComponentId,
                StartDate = sendEventDataDto.StartDate,
                Count = sendEventDataDto.Count,
                Message = sendEventDataDto.Message,
                TypeSystemName = sendEventDataDto.TypeSystemName,
                TypeDisplayName = sendEventDataDto.TypeDisplayName,
                TypeCode = sendEventDataDto.TypeCode,
                Importance = ConvertToCore(sendEventDataDto.Importance),
                JoinInterval = sendEventDataDto.JoinIntervalSeconds,
                JoinKey = sendEventDataDto.JoinKey,
                Category = ConvertToCore(sendEventDataDto.Category),
                Version = sendEventDataDto.Version,
                Properties = ConvertToCore(sendEventDataDto.Properties)
            };
            return message;
        }

        public static LogLevel ConvertToApi(Zidium.Core.Api.LogLevel level)
        {
            if (level == Zidium.Core.Api.LogLevel.Debug)
            {
                return LogLevel.Debug;
            }
            if (level == Zidium.Core.Api.LogLevel.Trace)
            {
                return LogLevel.Trace;
            }
            if (level == Zidium.Core.Api.LogLevel.Info)
            {
                return LogLevel.Info;
            }
            if (level == Zidium.Core.Api.LogLevel.Warning)
            {
                return LogLevel.Warning;
            }
            if (level == Zidium.Core.Api.LogLevel.Error)
            {
                return LogLevel.Error;
            }
            if (level == Zidium.Core.Api.LogLevel.Fatal)
            {
                return LogLevel.Fatal;
            }
            throw new Exception("Неизвестное значение level: " + level);
        }


        public static Zidium.Core.Api.LogLevel? ConvertToCore(LogLevel? level)
        {
            if (level == null)
            {
                return null;
            }
            if (level == LogLevel.Debug)
            {
                return Zidium.Core.Api.LogLevel.Debug;
            }
            if (level == LogLevel.Trace)
            {
                return Zidium.Core.Api.LogLevel.Trace;
            }
            if (level == LogLevel.Info)
            {
                return Zidium.Core.Api.LogLevel.Info;
            }
            if (level == LogLevel.Warning)
            {
                return Zidium.Core.Api.LogLevel.Warning;
            }
            if (level == LogLevel.Error)
            {
                return Zidium.Core.Api.LogLevel.Error;
            }
            if (level == LogLevel.Fatal)
            {
                return Zidium.Core.Api.LogLevel.Fatal;
            }
            throw new Exception("Неизвестное значение level: " + level);
        }

        public static Zidium.Core.Api.EventImportance? ConvertToCore(EventImportance? importance)
        {
            if (importance == null)
            {
                return null;
            }
            if (importance.Value == EventImportance.Unknown)
            {
                return Zidium.Core.Api.EventImportance.Unknown;
            }
            if (importance.Value == EventImportance.Success)
            {
                return Zidium.Core.Api.EventImportance.Success;
            }
            if (importance.Value == EventImportance.Warning)
            {
                return Zidium.Core.Api.EventImportance.Warning;
            }
            if (importance.Value == EventImportance.Alarm)
            {
                return Zidium.Core.Api.EventImportance.Alarm;
            }
            throw new Exception("Неизвестное значение EventImportance: " + importance);
        }

        public static EventImportance ConvertToApi(Zidium.Core.Api.EventImportance importance)
        {
            if (importance == Zidium.Core.Api.EventImportance.Unknown)
            {
                return EventImportance.Unknown;
            }
            if (importance == Zidium.Core.Api.EventImportance.Success)
            {
                return EventImportance.Success;
            }
            if (importance == Zidium.Core.Api.EventImportance.Warning)
            {
                return EventImportance.Warning;
            }
            if (importance == Zidium.Core.Api.EventImportance.Alarm)
            {
                return EventImportance.Alarm;
            }
            throw new Exception("Неизвестное значение EventImportance: " + importance);
        }

        public static Zidium.Core.Api.EventCategory? ConvertToCore(EventCategory? category)
        {
            if (category == null)
            {
                return null;
            }

            if (category == EventCategory.ApplicationError)
            {
                return Zidium.Core.Api.EventCategory.ApplicationError;
            }
            if (category == EventCategory.ComponentEvent)
            {
                return Zidium.Core.Api.EventCategory.ComponentEvent;
            }
            if (category == EventCategory.UnitTestStatus)
            {
                return Zidium.Core.Api.EventCategory.UnitTestStatus;
            }
            if (category == EventCategory.UnitTestResult)
            {
                return Zidium.Core.Api.EventCategory.UnitTestResult;
            }
            if (category == EventCategory.MetricStatus)
            {
                return Zidium.Core.Api.EventCategory.MetricStatus;
            }
            //if (category == EventCategory.MetricResult)
            //{
            //    return Zidium.Core.Api.EventCategory.MetricResult;
            //}
            if (category == EventCategory.ComponentEventsStatus)
            {
                return Zidium.Core.Api.EventCategory.ComponentEventsStatus;
            }
            if (category == EventCategory.ComponentUnitTestsStatus)
            {
                return Zidium.Core.Api.EventCategory.ComponentUnitTestsStatus;
            }
            if (category == EventCategory.ComponentMetricsStatus)
            {
                return Zidium.Core.Api.EventCategory.ComponentMetricsStatus;
            }
            if (category == EventCategory.ComponentChildsStatus)
            {
                return Zidium.Core.Api.EventCategory.ComponentChildsStatus;
            }
            if (category == EventCategory.ComponentInternalStatus)
            {
                return Zidium.Core.Api.EventCategory.ComponentInternalStatus;
            }
            if (category == EventCategory.ComponentExternalStatus)
            {
                return Zidium.Core.Api.EventCategory.ComponentExternalStatus;
            }
            throw new Exception("неизвестное значение EventCategory: " + category);
        }

        public static Zidium.Core.Api.EventCategory? ConvertToCore(SendEventCategory? category)
        {
            if (category == null)
            {
                return null;
            }

            if (category == SendEventCategory.ApplicationError)
            {
                return Zidium.Core.Api.EventCategory.ApplicationError;
            }
            if (category == SendEventCategory.ComponentEvent)
            {
                return Zidium.Core.Api.EventCategory.ComponentEvent;
            }
            throw new Exception("неизвестное значение EventCategory: " + category);
        }

        public static Zidium.Core.Api.SendMetricRequestData ConvertToCore(SendMetricRequestDtoData message)
        {
            if (message == null)
            {
                return null;
            }
            return new Zidium.Core.Api.SendMetricRequestData()
            {
                ComponentId = message.ComponentId,
                Name = message.Name,
                Value = message.Value,
                ActualIntervalSecs = message.ActualIntervalSecs
            };
        }


        public static Zidium.Core.Api.GetMetricsHistoryRequestData ConvertToCore(GetMetricsHistoryRequestDtoData filter)
        {
            if (filter == null)
            {
                return null;
            }
            var result = new Zidium.Core.Api.GetMetricsHistoryRequestData()
            {
                ComponentId = filter.ComponentId,
                Name = filter.Name,
                From = filter.From,
                To = filter.To,
                MaxCount = filter.MaxCount
            };
            return result;
        }

        public static Zidium.Core.Api.GetMetricsRequestData ConvertToCore(GetMetricsRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            var result = new Zidium.Core.Api.GetMetricsRequestData()
            {
                ComponentId = data.ComponentId
            };
            return result;
        }

        public static Zidium.Core.Api.GetMetricRequestData ConvertToCore(GetMetricRequestDtoData data)
        {
            if (data == null)
            {
                return null;
            }
            var result = new Zidium.Core.Api.GetMetricRequestData()
            {
                ComponentId = data.ComponentId,
                Name = data.Name
            };
            return result;
        }

        public static List<MetricDto> ConvertToApi(List<Zidium.Core.Api.MetricInfo> counters)
        {
            if (counters == null)
            {
                return null;
            }
            return counters.Select(ConvertToApi).ToList();
        }

        public static MetricDto ConvertToApi(Zidium.Core.Api.MetricInfo counter)
        {
            if (counter == null)
            {
                return null;
            }
            return new MetricDto()
            {
                ComponentId = counter.ComponentId,
                Name = counter.SystemName,
                Value = counter.Value,
                BeginDate = counter.BeginDate,
                ActualDate = counter.ActualDate,
                Status = ConvertToApi(counter.Status)
            };
        }
    }
}
