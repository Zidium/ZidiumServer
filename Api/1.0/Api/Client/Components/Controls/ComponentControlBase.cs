using System;
using System.Collections.Generic;

namespace Zidium.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Базовый класс IComponentControl
    /// </summary>
    public abstract class ComponentControlBase : IComponentControl
    {
        protected ILog LogInternal { get; set; }

        protected ComponentControlBase(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            ClientInternal = client;
        }

        
        #region Properties

        public IComponentTypeControl Type { get; protected set; }

        internal Client ClientInternal { get; set; }

        public IClient Client
        {
            get { return ClientInternal; }
        }

        public abstract bool IsRoot { get; }

        public abstract bool IsFolder { get; }
        
        public abstract string SystemName { get; }

        public abstract string Version { get; }

        public abstract ComponentInfo Info { get; }

        #endregion

        #region Components

        public abstract GetComponentByIdResponse GetParent();

        public abstract GetChildComponentsResponse GetChildComponents();

        public IComponentControl GetOrCreateChildComponentControl(string typeSystemName, string systemName)
        {
            var typeControl = Client.GetOrCreateComponentTypeControl(typeSystemName);
            return GetOrCreateChildComponentControl(typeControl, systemName);
        }

        public IComponentControl GetOrCreateChildComponentControl(
            IComponentTypeControl type, 
            string systemName)
        {
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return GetOrCreateChildComponentControl(new GetOrCreateComponentData(systemName, type));
        }

        public IComponentControl GetOrCreateChildComponentControl(
            IComponentTypeControl type, 
            string systemName, 
            string version)
        {
            var createData = new GetOrCreateComponentData(systemName, type)
            {
                Version = version
            };
            return GetOrCreateChildComponentControl(createData);
        }

        public abstract IComponentControl GetOrCreateChildComponentControl(GetOrCreateComponentData data);

        internal abstract IComponentControl GetOrCreateChildComponentControl(GetOrCreateComponentControlData data);

        public abstract UpdateComponentResponse Update(UpdateComponentData data);

        public abstract DeleteComponentResponse Delete();

        public IComponentControl GetOrCreateChildFolderControl(GetOrCreateFolderData data)
        {
            var type = Client.GetFolderComponentTypeControl();
            var componentData = new GetOrCreateComponentData(data.SystemName, type)
            {
                DisplayName = data.DisplayName,
                Version = null
            };
            componentData.Properties.CopyFrom(data.Properties);
            return GetOrCreateChildComponentControl(componentData);
        }

        public IComponentControl GetOrCreateChildFolderControl(string systemName)
        {
            var createData = new GetOrCreateFolderData(systemName);
            return GetOrCreateChildFolderControl(createData);
        }

        #endregion

        #region ApplicationError

        public ApplicationErrorData CreateApplicationError(string errorTypeSystemName)
        {
            if (errorTypeSystemName == null)
            {
                throw new ArgumentNullException("errorTypeSystemName");
            }
            var error = new ApplicationErrorData(this, errorTypeSystemName);
            error.TypeCode = HashHelper.GetInt32Dig5(errorTypeSystemName);
            PrepareEventAfterCreation(error);
            return error;
        }

        public ApplicationErrorData CreateApplicationError(string errorTypeSystemName, string message)
        {
            if (errorTypeSystemName == null)
            {
                throw new ArgumentNullException("errorTypeSystemName");
            }
            var error = new ApplicationErrorData(this, errorTypeSystemName)
            {
                Message = message
            };
            error.TypeCode = HashHelper.GetInt32Dig5(errorTypeSystemName);
            PrepareEventAfterCreation(error);
            return error;
        }

        public ApplicationErrorData CreateApplicationError(string errorTypeSystemName, Exception exception)
        {
            if (errorTypeSystemName == null)
            {
                throw new ArgumentNullException("errorTypeSystemName");
            }
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            var error = Client.ExceptionRender.GetApplicationErrorData(this, exception, errorTypeSystemName);
            PrepareEventAfterCreation(error);
            return error;
        }

        public ApplicationErrorData CreateApplicationError(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            var error = Client.ExceptionRender.GetApplicationErrorData(this, exception);
            PrepareEventAfterCreation(error);
            return error;
        }

        public AddEventResult AddApplicationError(string errorTypeSystemName)
        {
            var error = CreateApplicationError(errorTypeSystemName);
            return error.Add();
        }

        public AddEventResult AddApplicationError(string errorTypeSystemName, Exception exception)
        {
            var error = CreateApplicationError(errorTypeSystemName, exception);
            return error.Add();
        }

        public AddEventResult AddApplicationError(string errorTypeSystemName, string message)
        {
            var error = CreateApplicationError(errorTypeSystemName, message);
            return error.Add();
        }

        public AddEventResult AddApplicationError(Exception exception)
        {
            var error = CreateApplicationError(exception);
            return error.Add();
        }

        public SendEventResponse SendApplicationError(string errorTypeSystemName)
        {
            var error = CreateApplicationError(errorTypeSystemName);
            return error.Send();
        }

        public SendEventResponse SendApplicationError(string errorTypeSystemName, Exception exception)
        {
            var error = CreateApplicationError(errorTypeSystemName, exception);
            return error.Send();
        }

        public SendEventResponse SendApplicationError(Exception exception)
        {
            var error = CreateApplicationError(exception);
            return error.Send();
        }

        #endregion


        #region ComponentEvent

        public ComponentEventData CreateComponentEvent(string typeSystemName)
        {
            if (typeSystemName == null)
            {
                throw new ArgumentNullException("typeSystemName");
            }
            var componentEvent = new ComponentEventData(this, typeSystemName);
            PrepareEventAfterCreation(componentEvent);
            return componentEvent;
        }

        public ComponentEventData CreateComponentEvent(string typeSystemName, string message)
        {
            if (typeSystemName == null)
            {
                throw new ArgumentNullException("typeSystemName");
            }
            var componentEvent = new ComponentEventData(this, typeSystemName)
            {
                Message = message
            };
            PrepareEventAfterCreation(componentEvent);
            return componentEvent;
        }

        public AddEventResult AddComponentEvent(string typeSystemName)
        {
            if (typeSystemName == null)
            {
                throw new ArgumentNullException("typeSystemName");
            }
            var eventObj = CreateComponentEvent(typeSystemName);
            return eventObj.Add();
        }

        public AddEventResult AddComponentEvent(string typeSystemName, string message)
        {
            if (typeSystemName == null)
            {
                throw new ArgumentNullException("typeSystemName");
            }
            var eventObj = CreateComponentEvent(typeSystemName, message);
            return eventObj.Add();
        }

        public SendEventResponse SendComponentEvent(string typeSystemName)
        {
            if (typeSystemName == null)
            {
                throw new ArgumentNullException("typeSystemName");
            }
            return CreateComponentEvent(typeSystemName).Send();
        }

        public SendEventResponse SendComponentEvent(string typeSystemName, string message)
        {
            if (typeSystemName == null)
            {
                throw new ArgumentNullException("typeSystemName");
            }
            return CreateComponentEvent(typeSystemName, message).Send();
        }

        #endregion


        #region Events

        protected void PrepareEventAfterCreation(SendEventBase eventData)
        {
            if (eventData == null)
            {
                return;
            }
            try
            {
                var preparer = Client.EventPreparer;
                if (preparer != null)
                {
                    preparer.Prepare(eventData);
                }
            }
            catch (Exception exception)
            {
                ClientInternal.InternalLog.Error("Ошибка в PrepareEventAfterCreation: " + exception.Message, exception);
            }
        }

        #endregion


        #region UnitTests

        public abstract GetOrCreateUnitTestResponse GetOrCreateUnitTest(GetOrCreateUnitTestData data);

        public GetOrCreateUnitTestResponse GetOrCreateUnitTest(string typeSystemName, string instanceSystemName)
        {
            var type = Client.GetOrCreateUnitTestTypeControl(typeSystemName);
            var createData = new GetOrCreateUnitTestData(instanceSystemName)
            {
                UnitTestTypeControl = type
            };
            return GetOrCreateUnitTest(createData);
        }

        public GetOrCreateUnitTestResponse GetOrCreateUnitTest(IUnitTestTypeControl unitTestTypeControl, string systemName)
        {
            var createData = new GetOrCreateUnitTestData(systemName)
            {
                UnitTestTypeControl = unitTestTypeControl
            };
            return GetOrCreateUnitTest(createData);
        }

        // контролы

        protected IUnitTestTypeControl GetCustomUnitTestType()
        {
            return Client.GetOrCreateUnitTestTypeControl("CustomUnitTestType", "Пользовательская проверка");
        }

        public IUnitTestControl GetOrCreateUnitTestControl(string systemName)
        {
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            var type = GetCustomUnitTestType();
            var createData = new GetOrCreateUnitTestData(systemName)
            {
                UnitTestTypeControl = type
            };
            return GetOrCreateUnitTestControl(createData);
        }

        public IUnitTestControl GetOrCreateUnitTestControl(
            string systemName, 
            string displayName)
        {
            var type = GetCustomUnitTestType();
            var createData = new GetOrCreateUnitTestData(systemName)
            {
                DisplayName = displayName,
                UnitTestTypeControl = type
            };
            return GetOrCreateUnitTestControl(createData);
        }

        public IUnitTestControl GetOrCreateUnitTestControl(
            IUnitTestTypeControl unitTestTypeControl, 
            string systemName)
        {
            var createData = new GetOrCreateUnitTestData(systemName)
            {
                UnitTestTypeControl = unitTestTypeControl
            };
            return GetOrCreateUnitTestControl(createData);
        }

        public abstract IUnitTestControl GetOrCreateUnitTestControl(GetOrCreateUnitTestData data);

        #endregion


        public ILog Log
        {
            get
            {
                if (LogInternal == null)
                {
                    LogInternal = new Log(this);
                }
                return LogInternal;
            }
        }

        public abstract GetLogsResponse GetLogs(GetLogsFilter filter);

        public abstract GetLogConfigResponse GetWebLogConfig();

        public abstract GetComponentTotalStateResponse GetTotalState(bool recalc);

        public abstract GetComponentInternalStateResponse GetInternalState(bool recalc);

        public abstract void Dispose();

        public abstract WebLogConfig WebLogConfig { get; }
        
        public abstract bool IsFake();


        #region Метрики

        public abstract GetMetricsResponse GetMetrics();

        public abstract GetMetricsHistoryResponse GetMetricsHistory(GetMetricsHistoryFilter filter);

        public abstract SendMetricsResponse SendMetrics(List<SendMetricData> data);

        public abstract SendMetricResponse SendMetric(SendMetricData data);

        public SendMetricResponse SendMetric(string name, double? value, TimeSpan actualInterval)
        {
            var data = new SendMetricData()
            {
                Name = name,
                Value = value,
                ActualInterval = actualInterval
            };
            return SendMetric(data);
        }

        public SendMetricResponse SendMetric(string name, double? value)
        {
            var data = new SendMetricData()
            {
                Name = name,
                Value = value
            };
            return SendMetric(data);
        }

        public abstract GetMetricResponse GetMetric(string name);

        #endregion


        public abstract SetComponentEnableResponse Enable();

        public abstract SetComponentDisableResponse Disable(string comment);

        public abstract SetComponentDisableResponse Disable(string comment, DateTime date);
        
    }
}
