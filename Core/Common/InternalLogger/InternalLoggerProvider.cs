using Microsoft.Extensions.Logging;

namespace Zidium.Core.InternalLogger
{
    [ProviderAlias("Internal")]
    public class InternalLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        public InternalLoggerProvider(InternalLoggerComponentMapping mappings)
        {
            _mappings = mappings;
        }

        private readonly InternalLoggerComponentMapping _mappings;
        private IExternalScopeProvider _scopeProvider;

        public ILogger CreateLogger(string categoryName)
        {
            var componentId = _mappings.GetMapping(categoryName);
            return new InternalLogger(categoryName, componentId, this);
        }

        public void Dispose()
        {
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        internal IExternalScopeProvider ScopeProvider
        {
            get
            {
                if (_scopeProvider == null)
                    _scopeProvider = new LoggerExternalScopeProvider();
                return _scopeProvider;
            }
        }
    }
}
