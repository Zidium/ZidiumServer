using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zidium.Core.InternalLogger;

namespace Zidium.Agent
{
    public class Worker : BackgroundService
    {
        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            DependencyInjection.SetLoggerFactory(loggerFactory);
            DependencyInjection.SetServicePersistent<InternalLoggerComponentMapping>(serviceProvider.GetRequiredService<InternalLoggerComponentMapping>());
        }

        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var application = new AgentService();

            try
            {
                application.Start(_configuration);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }

            var tcs = new TaskCompletionSource<bool>();
            stoppingToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            await tcs.Task;

            application.Stop();
        }
    }
}
