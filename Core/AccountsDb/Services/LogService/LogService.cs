﻿using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.DispatcherLayer;
using Zidium.Core.Limits;

namespace Zidium.Core.AccountsDb
{
    public class LogService : ILogService
    {
        protected DispatcherContext Context { get; set; }

        public LogService(DispatcherContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public LogConfig GetLogConfig(Guid accountId, Guid componentId)
        {
            lock (LockObject.ForComponent(componentId))
            {
                var componentService = Context.ComponentService;
                var component = componentService.GetComponentById(accountId, componentId);

                var accountDbContext = Context.GetAccountDbContext(accountId);
                var repository = accountDbContext.GetLogConfigRepository();
                var config = repository.GetByComponentId(componentId);

                if (config == null)
                {
                    config = new LogConfig()
                    {
                        Enabled = true,
                        ComponentId = component.Id,
                        LastUpdateDate = DateTime.Now,
                        IsDebugEnabled = true,
                        IsTraceEnabled = true,
                        IsInfoEnabled = true,
                        IsWarningEnabled = true,
                        IsErrorEnabled = true,
                        IsFatalEnabled = true
                    };
                    repository.Add(config);
                }

                return config;
            }
        }

        public void SaveLogMessage(Guid accountId, Guid componentId, SendLogData message)
        {
            // Проверка на наличие необходимых параметров
            FixLogData(message);

            // Проверим лимиты
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            var size = message.GetSize();

            bool canIncreaseSizeInStatictics = false;
            try
            {
                checker.CheckLogSizePerDay(accountDbContext, size);

                // Проверим лимит размера хранилища
                checker.CheckStorageSize(accountDbContext, size, out canIncreaseSizeInStatictics);

                // Получим компонент
                var componentService = Context.ComponentService;
                var component = componentService.GetComponentById(accountId, componentId);

                var logRepository = Context.DbContext.GetAccountDbContext(accountId).GetLogRepository();
                var log = ApiConverter.GetLog(component.Id, message);
                logRepository.Add(log);

                Context.SaveChanges();
            }
            finally
            {
                if (canIncreaseSizeInStatictics)
                    checker.AddLogSizePerDay(accountDbContext, size);
            }
        }

        public void SaveLogMessages(Guid accountId, SendLogData[] messages)
        {
            // Проверка на наличие необходимых параметров
            if (messages == null)
            {
                throw new ParameterRequiredException("Request.Messages");
            }

            // Проверим лимиты
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            var totalSize = messages.Sum(t => t.GetSize());

            bool canIncreaseSizeInStatictics = false;
            try
            {
                checker.CheckLogSizePerDay(accountDbContext, totalSize);

                // Проверим лимит размера хранилища
                checker.CheckStorageSize(accountDbContext, totalSize, out canIncreaseSizeInStatictics);

                var componentService = Context.ComponentService;
                var logRepository = Context.DbContext.GetAccountDbContext(accountId).GetLogRepository();

                // Добавим все записи в одной транзакции
                accountDbContext.Configuration.AutoDetectChangesEnabled = false;
                foreach (var message in messages)
                {
                    FixLogData(message);

                    var component = componentService.GetComponentById(accountId, message.ComponentId.Value);

                    var log = ApiConverter.GetLog(component.Id, message);
                    logRepository.Add(log);
                }
                accountDbContext.ChangeTracker.DetectChanges();

                Context.SaveChanges();
            }
            finally
            {
                accountDbContext.Configuration.AutoDetectChangesEnabled = true;
                if (canIncreaseSizeInStatictics)
                    checker.AddLogSizePerDay(accountDbContext, totalSize);
            }
        }

        protected void FixLogData(SendLogData data)
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
                }
            }
        }

        public List<Log> GetLogs(Guid accountId, Guid componentId, GetLogsRequestData requestData)
        {
            var componentService = Context.ComponentService;
            var component = componentService.GetComponentById(accountId, componentId);

            var accountDbContext = Context.GetAccountDbContext(accountId);
            var logRepository = accountDbContext.GetLogRepository();
            var maxCount = requestData.MaxCount ?? 1000;
            if (maxCount > 1000)
            {
                maxCount = 1000;
            }
            else if (maxCount < 1)
            {
                maxCount = 1000;
            }

            var rows = logRepository.Find(
                maxCount,
                component.Id,
                requestData.From,
                requestData.To,
                requestData.Levels,
                requestData.Context,
                requestData.Message,
                requestData.PropertyName,
                requestData.PropertyValue);

            return rows;
        }

        public List<LogConfig> GetChangedConfigs(Guid accountId, DateTime lastUpdateDate, List<Guid> componentIds)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = accountDbContext.GetLogConfigRepository();

            // из-за того, что мс отбрасываются, получется, что можно один и тот же конфиг грузить постоянно
            // чтобы этого избежать будет прибавлять 1 секунду
            lastUpdateDate = lastUpdateDate.AddSeconds(1);

            return repository.GetChanged(lastUpdateDate, componentIds);
        }
    }
}
