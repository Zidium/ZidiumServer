using System;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Limits;
using Zidium.Storage;
using Zidium.Api.Dto;

namespace Zidium.Core.AccountsDb
{
    public class LogService : ILogService
    {
        public LogService(IStorage storage, ITimeService timeService)
        {
            _storage = storage;
            _timeService = timeService;
        }

        private readonly IStorage _storage;
        private readonly ITimeService _timeService;

        public LogConfigForRead GetLogConfig(Guid componentId)
        {
            lock (LockObject.ForComponent(componentId))
            {
                var config = _storage.LogConfigs.GetOneOrNullByComponentId(componentId);

                if (config == null)
                {
                    var configForAdd = new LogConfigForAdd()
                    {
                        ComponentId = componentId,
                        Enabled = true,
                        LastUpdateDate = _timeService.Now(),
                        IsDebugEnabled = true,
                        IsTraceEnabled = true,
                        IsInfoEnabled = true,
                        IsWarningEnabled = true,
                        IsErrorEnabled = true,
                        IsFatalEnabled = true
                    };
                    _storage.LogConfigs.Add(configForAdd);
                    config = _storage.LogConfigs.GetOneByComponentId(configForAdd.ComponentId);
                }

                return config;
            }
        }

        public void SaveLogMessage(Guid componentId, SendLogRequestDataDto message)
        {
            // Проверка на наличие необходимых параметров
            FixLogData(message);

            // Проверим лимиты
            var checker = AccountLimitsCheckerManager.GetChecker();
            var size = message.GetSize();

            try
            {
                // Получим компонент
                var componentService = new ComponentService(_storage, _timeService);
                var component = componentService.GetComponentById(componentId);

                var log = ApiConverter.GetLog(component.Id, message);
                _storage.Logs.Add(log);
            }
            finally
            {
                checker.AddLogSizePerDay(_storage, size);
            }
        }

        public void SaveLogMessages(SendLogRequestDataDto[] messages)
        {
            // Проверка на наличие необходимых параметров
            if (messages == null)
            {
                throw new ParameterRequiredException("Request.Messages");
            }

            // Проверим лимиты
            var checker = AccountLimitsCheckerManager.GetChecker();
            var totalSize = messages.Sum(t => t.GetSize());

            try
            {
                var componentService = new ComponentService(_storage, _timeService);

                // Добавим все записи в одной транзакции
                foreach (var message in messages)
                {
                    FixLogData(message);
                }

                var logs = messages.Select(t => ApiConverter.GetLog(t.ComponentId.Value, t)).ToArray();
                _storage.Logs.Add(logs);
            }
            finally
            {
                checker.AddLogSizePerDay(_storage, totalSize);
            }
        }

        protected void FixLogData(SendLogRequestDataDto data)
        {
            if (data == null)
            {
                throw new ParameterRequiredException("Request.Message");
            }

            if (data.Message == null)
            {
                throw new ParameterRequiredException("Request.Message.Message"); //todo надо переименовать
            }

            if (data.Message.Length > 4000)
            {
                data.Message = data.Message.Substring(0, 4000);
            }

            if (data.Context != null && data.Context.Length > 255)
            {
                data.Context = data.Context.Substring(0, 255);
            }

            data.Message = data.Message.FixStringSymbols();

            if (data.Properties != null)
            {
                foreach (var property in data.Properties)
                {
                    if (property.Name == null)
                    {
                        throw new ParameterRequiredException("Property.Name");
                    }

                    if (property.Name.Length > 100)
                    {
                        property.Name = property.Name.Substring(0, 100);
                    }

                    property.Name = property.Name.FixStringSymbols();

                    if (property.Value != null)
                        property.Value = property.Value.FixStringSymbols();
                }
            }
        }

        public LogForRead[] GetLogs(Guid componentId, GetLogsRequestDataDto requestData)
        {
            var maxCount = requestData.MaxCount ?? 1000;
            if (maxCount > 1000)
            {
                maxCount = 1000;
            }
            else if (maxCount < 1)
            {
                maxCount = 1000;
            }

            var rows = _storage.Logs.Find(
                maxCount,
                componentId,
                requestData.From,
                requestData.To,
                requestData.Levels,
                requestData.Context,
                requestData.Message,
                requestData.PropertyName,
                requestData.PropertyValue);

            return rows;
        }

        public LogConfigForRead[] GetChangedConfigs(DateTime lastUpdateDate, Guid[] componentIds)
        {
            // из-за того, что мс отбрасываются, получется, что можно один и тот же конфиг грузить постоянно
            // чтобы этого избежать будет прибавлять 1 секунду
            lastUpdateDate = lastUpdateDate.AddSeconds(1);

            return _storage.LogConfigs.GetChanged(lastUpdateDate, componentIds);
        }
    }
}
