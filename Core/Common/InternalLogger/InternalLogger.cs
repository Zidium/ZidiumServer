using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Zidium.Api;

namespace Zidium.Core.InternalLogger
{
    public class InternalLogger : ILogger
    {
        public InternalLogger(string categoryName, Guid? componentId, InternalLoggerProvider provider)
        {
            _categoryName = categoryName;
            _componentId = componentId;
            _provider = provider;
        }

        private readonly string _categoryName;
        private readonly Guid? _componentId;
        private readonly InternalLoggerProvider _provider;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var level = LogLevelHelper.GetLogLevel(logLevel);
            var message = formatter(state, exception);
            var context = _categoryName;

            Dictionary<string, object> properties = null;
            if (eventId.Id != 0 || !string.IsNullOrEmpty(eventId.Name))
            {
                properties = new Dictionary<string, object>
                {
                    {"EventId", eventId.ToString()}
                };
            }

            // Gather info about scopes, if any
            var scopeProvider = _provider.ScopeProvider;
            if (scopeProvider != null)
            {
                List<string> scopeInfo = null;
                scopeProvider.ForEachScope((value, loggingProps) =>
                {
                    if (scopeInfo == null)
                    {
                        scopeInfo = new List<string>();
                    }
                    scopeInfo.Add(value.ToString());
                }, state);

                if (scopeInfo != null && scopeInfo.Count > 0)
                    context += " => " + string.Join(" => ", scopeInfo);
            }

            var log = Component.Log.GetTaggedCopy(context);
            log.Write(level, message, exception, properties);

            if (level > Zidium.Api.Dto.LogLevel.Info)
            {
                var data = Component.Client.ExceptionRender.CreateEventFromLog(Component, level, exception, message, properties);
                data.Add();
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var level = LogLevelHelper.GetLogLevel(logLevel);
            return Component.Log.IsEnabled(level);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _provider.ScopeProvider.Push(state);
        }

        private IComponentControl _component;

        private IComponentControl Component
        {
            get
            {
                if (_component == null)
                {
                    _component = _componentId != null ? Client.Instance.GetComponentControl(_componentId.Value) : Client.Instance.GetDefaultComponentControl();
                }
                return _component;
            }
        }
    }
}
