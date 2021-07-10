using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// TODO Refactor to Net Core DI
namespace Zidium
{
    public static class DependencyInjection
    {
        // Scoped

        public static T GetService<T>(params object[] args)
        {
            if (_services.TryGetValue(typeof(T), out var result))
            {
                return (T)Activator.CreateInstance(result, args);
            }

            throw new Exception($"Не найдена реализация для интерфейса {typeof(T).Name}");
        }

        public static void SetService<TInterface, TImplementation>()
            where TImplementation : TInterface
        {
            _services.Add(typeof(TInterface), typeof(TImplementation));
        }

        private static readonly Dictionary<Type, Type> _services = new Dictionary<Type, Type>();

        // Persistent

        public static T GetServicePersistent<T>()
        {
            if (_servicesPersistent.TryGetValue(typeof(T), out var result))
            {
                return (T)result;
            }

            throw new Exception($"Не найдена реализация для интерфейса {typeof(T).Name}");
        }

        public static void SetServicePersistent<TInterface>(object implementation)
        {
            _servicesPersistent.Add(typeof(TInterface), implementation);
        }

        private static readonly Dictionary<Type, object> _servicesPersistent = new Dictionary<Type, object>();

        // Logging

        public static ILogger<T> GetLogger<T>()
        {
            if (_loggerFactory == null)
                return NullLogger<T>.Instance;

            return _loggerFactory.CreateLogger<T>();
        }

        public static ILogger GetLogger(string name)
        {
            if (_loggerFactory == null)
                return NullLogger.Instance;

            return _loggerFactory.CreateLogger(name);
        }

        public static void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        private static ILoggerFactory _loggerFactory;
    }
}
