using System;
using System.Collections.Generic;

namespace Zidium.Core
{
    public static class DependencyInjection
    {
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

        private static Dictionary<Type, Type> _services = new Dictionary<Type, Type>();
    }
}
