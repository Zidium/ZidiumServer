using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb.Repositories
{
    internal class LogConfigRepository : ILogConfigRepository
    {
        protected AccountDbContext Context;

        public LogConfigRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public LogConfig GetByComponentId(Guid componentId)
        {
            return Context.LogConfigs.SingleOrDefault(x => x.ComponentId == componentId);
        }

        public LogConfig Add(LogConfig config)
        {
            Context.LogConfigs.Add(config);
            Context.SaveChanges();
            return config;
        }

        public List<LogConfig> GetChanged(DateTime lastUpdateDate, List<Guid> componentIds)
        {
            if (componentIds == null || componentIds.Count == 0)
            {
                return new List<LogConfig>(0);
            }

            return Context.LogConfigs
                .Where(x => componentIds.Contains(x.ComponentId) && x.LastUpdateDate > lastUpdateDate)
                .ToList();
        }
    }
}
