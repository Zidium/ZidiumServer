using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;

namespace Zidium.Agent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var appConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile("appsettings.prod.json", true, false)
                .AddUserSecrets(Assembly.GetEntryAssembly(), true)
                .Build();

            var application = new AgentService();

            try
            {
                application.Start(appConfiguration);
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Error(exception);
                throw;
            }

            var tcs = new TaskCompletionSource<bool>();
            stoppingToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            await tcs.Task;

            application.Stop();
        }
    }
}
