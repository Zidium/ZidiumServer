using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class LogConfigRepository : ILogConfigRepository
    {
        public LogConfigRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(LogConfigForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.LogConfigs.Add(new DbLogConfig()
                {
                    ComponentId = entity.ComponentId,
                    Enabled = entity.Enabled,
                    IsDebugEnabled = entity.IsDebugEnabled,
                    IsErrorEnabled = entity.IsErrorEnabled,
                    IsFatalEnabled = entity.IsFatalEnabled,
                    IsInfoEnabled = entity.IsInfoEnabled,
                    IsTraceEnabled = entity.IsTraceEnabled,
                    IsWarningEnabled = entity.IsWarningEnabled,
                    LastUpdateDate = entity.LastUpdateDate
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(LogConfigForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var config = DbGetOneById(entity.ComponentId);

                if (entity.LastUpdateDate.Changed())
                    config.LastUpdateDate = entity.LastUpdateDate.Get();

                if (entity.Enabled.Changed())
                    config.Enabled = entity.Enabled.Get();

                if (entity.IsDebugEnabled.Changed())
                    config.IsDebugEnabled = entity.IsDebugEnabled.Get();

                if (entity.IsTraceEnabled.Changed())
                    config.IsTraceEnabled = entity.IsTraceEnabled.Get();

                if (entity.IsInfoEnabled.Changed())
                    config.IsInfoEnabled = entity.IsInfoEnabled.Get();

                if (entity.IsWarningEnabled.Changed())
                    config.IsWarningEnabled = entity.IsWarningEnabled.Get();

                if (entity.IsErrorEnabled.Changed())
                    config.IsErrorEnabled = entity.IsErrorEnabled.Get();

                if (entity.IsFatalEnabled.Changed())
                    config.IsFatalEnabled = entity.IsFatalEnabled.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public LogConfigForRead GetOneByComponentId(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public LogConfigForRead GetOneOrNullByComponentId(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public LogConfigForRead[] GetChanged(DateTime lastUpdateDate, Guid[] componentIds)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                if (componentIds == null || componentIds.Length == 0)
                {
                    return new LogConfigForRead[0];
                }

                return contextWrapper.Context.LogConfigs.AsNoTracking()
                    .Where(x => componentIds.Contains(x.ComponentId) && x.LastUpdateDate > lastUpdateDate)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbLogConfig DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.LogConfigs.Find(id);
            }
        }

        private DbLogConfig DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Настройки лога {id} не найдены");

            return result;
        }

        private LogConfigForRead DbToEntity(DbLogConfig entity)
        {
            if (entity == null)
                return null;

            return new LogConfigForRead(entity.ComponentId, entity.LastUpdateDate, entity.Enabled, entity.IsDebugEnabled,
                entity.IsTraceEnabled, entity.IsInfoEnabled, entity.IsWarningEnabled, entity.IsErrorEnabled,
                entity.IsFatalEnabled);
        }
    }
}
