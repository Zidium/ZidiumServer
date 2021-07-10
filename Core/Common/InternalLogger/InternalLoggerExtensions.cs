using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Zidium.Core.InternalLogger
{
    public static class InternalLoggerExtensions
    {
        public static ILoggingBuilder AddInternalLog(this ILoggingBuilder builder, Guid? componentId = null)
        {
            builder.AddConfiguration();
            builder.Services.AddSingleton(new InternalLoggerComponentMapping(componentId));
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, InternalLoggerProvider>());
            return builder;
        }
    }
}
