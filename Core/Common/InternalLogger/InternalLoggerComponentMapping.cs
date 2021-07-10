using System;
using System.Collections.Concurrent;

namespace Zidium.Core.InternalLogger
{
    public class InternalLoggerComponentMapping
    {
        public InternalLoggerComponentMapping(Guid? componentId)
        {
            _componentId = componentId;
        }

        private readonly Guid? _componentId;
        private readonly ConcurrentDictionary<string, Guid> _mappings = new ConcurrentDictionary<string, Guid>();

        public void MapLoggerToComponent(string categoryName, Guid componentId)
        {
            _mappings.TryAdd(categoryName, componentId);
        }

        public Guid? GetMapping(string categoryName)
        {
            if (_mappings.TryGetValue(categoryName, out var result))
                return result;

            return _componentId;
        }
    }
}
