using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class LimitDataRepository : ILimitDataRepository
    {
        public LimitDataRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(LimitDataForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.LimitDatas.Add(new DbLimitData()
                {
                    Id = entity.Id,
                    Type = entity.Type,
                    EndDate = entity.EndDate,
                    BeginDate = entity.BeginDate,
                    EventsRequests = entity.EventsRequests,
                    EventsSize = entity.EventsSize,
                    LogSize = entity.LogSize,
                    MetricsRequests = entity.MetricsRequests,
                    MetricsSize = entity.MetricsSize,
                    SmsCount = entity.SmsCount,
                    UnitTestsRequests = entity.UnitTestsRequests,
                    UnitTestsSize = entity.UnitTestsSize
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public LimitDataForRead[] Find(DateTime from, LimitDataType type)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.LimitDatas.AsNoTracking()
                    .Where(t => t.BeginDate >= from && t.Type == type)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public LimitDataForRead[] GetByType(LimitDataType type)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.LimitDatas.AsNoTracking()
                    .Where(t => t.Type == type)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public LimitDataForRead GetOneOrNullByDateAndType(DateTime date, LimitDataType type)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.LimitDatas.AsNoTracking()
                    .FirstOrDefault(t => t.BeginDate == date && t.Type == type));
            }
        }

        public void RemoveOld(DateTime date, LimitDataType type)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                using (var connection = contextWrapper.Context.CreateConnection())
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = 0;
                        command.CommandText = $"DELETE FROM {contextWrapper.Context.FormatTableName("LimitDatasForUnitTests")} WHERE {contextWrapper.Context.FormatColumnName("LimitDataId")} IN (SELECT {contextWrapper.Context.FormatColumnName("Id")} FROM {contextWrapper.Context.FormatTableName("LimitDatas")} WHERE {contextWrapper.Context.FormatColumnName("BeginDate")} < @BeginDate AND {contextWrapper.Context.FormatColumnName("Type")} = @Type)";

                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@BeginDate";
                        parameter.Value = date;
                        command.Parameters.Add(parameter);

                        parameter = command.CreateParameter();
                        parameter.ParameterName = "@Type";
                        parameter.Value = (int)type;
                        command.Parameters.Add(parameter);

                        command.ExecuteNonQuery();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandTimeout = 0;
                        command.CommandText = $"DELETE FROM {contextWrapper.Context.FormatTableName("LimitDatas")} WHERE {contextWrapper.Context.FormatColumnName("BeginDate")} < @BeginDate AND {contextWrapper.Context.FormatColumnName("Type")} = @Type";

                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@BeginDate";
                        parameter.Value = date;
                        command.Parameters.Add(parameter);

                        parameter = command.CreateParameter();
                        parameter.ParameterName = "@Type";
                        parameter.Value = (int)type;
                        command.Parameters.Add(parameter);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private DbLimitData DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.LimitDatas.Find(id);
            }
        }

        private DbLimitData DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Данные лимитов {id} не найдены");

            return result;
        }

        private LimitDataForRead DbToEntity(DbLimitData entity)
        {
            if (entity == null)
                return null;

            return new LimitDataForRead(entity.Id, entity.BeginDate, entity.EndDate, entity.Type, entity.EventsRequests,
                entity.LogSize, entity.UnitTestsRequests, entity.MetricsRequests, entity.EventsSize, entity.UnitTestsSize,
                entity.MetricsSize, entity.SmsCount);
        }
    }
}
