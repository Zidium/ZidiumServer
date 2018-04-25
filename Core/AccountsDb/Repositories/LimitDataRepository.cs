using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class LimitDataRepository : AccountBasedRepository<LimitData>, ILimitDataRepository
    {
        public LimitDataRepository(AccountDbContext context) : base(context) { }

        public LimitData Add(LimitData entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
            Context.LimitDatas.Add(entity);
            return entity;
        }

        public LimitData GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public LimitData GetByIdOrNull(Guid id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<LimitData> QueryAll()
        {
            return Context.LimitDatas;
        }

        public LimitData Update(LimitData entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(LimitData entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public void RemoveOld(DateTime date, LimitDataType type)
        {
            using (var connection = Context.CreateConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = "DELETE FROM dbo.LimitDatasForUnitTests WHERE LimitDataId IN (SELECT Id FROM dbo.LimitDatas WHERE BeginDate < @BeginDate AND Type = @Type)";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@BeginDate";
                    parameter.Value = date;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@Type";
                    parameter.Value = (int) type;
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = "DELETE FROM dbo.LimitDatas WHERE BeginDate < @BeginDate AND Type = @Type";

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

        public List<LimitData> GetByPeriod(DateTime fromDate, DateTime toDate)
        {
            return Context.LimitDatas
                .Where(x => x.BeginDate >= fromDate && x.BeginDate < toDate)
                .ToList();
        }
    }
}
