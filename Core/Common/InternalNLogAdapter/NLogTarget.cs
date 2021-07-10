using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Common;
using NLog.Targets;
using Zidium.Api;

namespace Zidium.Core.InternalNLogAdapter
{
    /// <summary>
    /// Специальный внутренний адаптер для NLog
    /// Подключает проект Api, а не пакет Api из Nuget
    /// Нужно для отладки и более удобного процесса разработки
    /// </summary>
    [Target("Zidium")]
    public class NLogTarget : Target
    {
        public NLogTarget()
        {
        }

        public NLogTarget(Guid? componentId = null)
        {
            _componentId = componentId;
        }

        private Guid? _componentId;

        protected override void Write(LogEventInfo logEvent)
        {
            var log = Component.Log.GetTaggedCopy(logEvent.LoggerName);

            string message;

            if (logEvent.Exception != null && logEvent.Message == "{0}")
                message = logEvent.Exception.Message;
            else
                message = logEvent.FormattedMessage;

            Dictionary<string, object> properties = null;

            if (logEvent.HasProperties)
            {
                properties = logEvent.Properties.ToDictionary(a => a.Key.ToString(), b => b.Value);
            }

            var level = GetLogLevel(logEvent.Level);

            log.Write(level, message, logEvent.Exception, properties);

            if (level <= Zidium.Api.Dto.LogLevel.Info)
                return;

            var errorData = Component.Client.ExceptionRender.CreateEventFromLog(Component, level, logEvent.Exception, message, properties);
            errorData.Add();
        }

        private IComponentControl _component;

        private IComponentControl Component
        {
            get
            {
                if (_component == null)
                {
                    _component = _componentId == null ? Client.Instance.GetDefaultComponentControl() : Client.Instance.GetComponentControl(_componentId.Value);
                }
                return _component;
            }
        }

        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            Client.Instance.WebLogManager.Flush();
            base.FlushAsync(asyncContinuation);
        }

        protected static Zidium.Api.Dto.LogLevel GetLogLevel(NLog.LogLevel level)
        {
            if (level.Ordinal >= NLog.LogLevel.Fatal.Ordinal)
                return Zidium.Api.Dto.LogLevel.Fatal;

            if (level.Ordinal >= NLog.LogLevel.Error.Ordinal)
                return Zidium.Api.Dto.LogLevel.Error;

            if (level.Ordinal >= NLog.LogLevel.Warn.Ordinal)
                return Zidium.Api.Dto.LogLevel.Warning;

            if (level.Ordinal >= NLog.LogLevel.Info.Ordinal)
                return Zidium.Api.Dto.LogLevel.Info;

            if (level.Ordinal >= NLog.LogLevel.Debug.Ordinal)
                return Zidium.Api.Dto.LogLevel.Debug;

            return Zidium.Api.Dto.LogLevel.Trace;
        }
    }
}
