using System;
using System.Data;
using System.Threading;
using NLog;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Storage;
using Npgsql;
using Zidium.Api.Dto;
using Microsoft.Data.SqlClient;

namespace Zidium.Agent.AgentTasks
{
    public class SqlCheckProcessor : UnitTestProcessorBase
    {
        public SqlCheckProcessor(ILogger logger, CancellationToken cancellationToken, ITimeService timeService)
            : base(logger, cancellationToken, timeService)
        {
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.SqlTestType.Id;
        }

        protected override UnitTestExecutionInfo GetResult(
            IStorage storage,
            UnitTestForRead unitTest)
        {
            var rule = storage.UnitTestSqlRules.GetOneByUnitTestId(unitTest.Id);
            return CheckSql(rule);
        }

        public UnitTestExecutionInfo CheckSql(UnitTestSqlRuleForRead rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }
            try
            {
                var dataSet = new DataSet();

                if (rule.Provider == SqlRuleDatabaseProviderType.MsSql)
                {
                    FillMsSqlDataSet(rule, dataSet);
                }
                else if (rule.Provider == SqlRuleDatabaseProviderType.PostgreSql)
                {
                    FillPostrgesqlDataSet(rule, dataSet);
                }
                else
                {
                    throw new UserFriendlyException("Не поддерживается провайдер " + rule.Provider);
                }

                if (dataSet.Tables.Count == 0)
                {
                    throw new UserFriendlyException("Запрос не вернул ни одной таблицы");
                }
                if (dataSet.Tables.Count > 1)
                {
                    throw new UserFriendlyException("Запрос вернул более одной таблицы");
                }
                var table = dataSet.Tables[0];
                if (table.Rows.Count == 0)
                {
                    throw new UserFriendlyException("Таблица не содержит ни одной строки");
                }
                if (table.Rows.Count > 1)
                {
                    throw new UserFriendlyException("Таблица содержит более одной строки");
                }
                var row = table.Rows[0];
                if (table.Columns.Count != 2)
                {
                    throw new UserFriendlyException("Таблица должна содержать 2 столбца");
                }
                var statusText = row[0].ToString();
                var message = row[1].ToString();
                var status = GetStatus(statusText);
                return new UnitTestExecutionInfo()
                {
                    ResultRequest = new SendUnitTestResultRequestDataDto()
                    {
                        Message = message,
                        Result = status
                    }
                };
            }
            catch (Exception exception)
            {
                var sqlException = exception as SqlException;

                return new UnitTestExecutionInfo()
                {
                    ResultRequest = new SendUnitTestResultRequestDataDto()
                    {
                        Message = exception.Message,
                        Result = UnitTestResult.Alarm,
                    },
                    IsNetworkProblem = sqlException?.Number == -2 || sqlException?.Number == 11 // TIMEOUT_EXPIRED = -2, GENERAL_NETWORK_ERROR = 11
                };
            }
        }

        private void FillMsSqlDataSet(UnitTestSqlRuleForRead rule, DataSet dataSet)
        {
            string connString;
            try
            {
                connString = GetMsSqlConnectionStringWithTimeout(rule);
            }
            catch (Exception exception)
            {
                throw new UserFriendlyException(exception.Message);
            }

            using (var connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception exception)
                {
                    throw new UserFriendlyException(exception.Message);
                }

                using (var command = new SqlCommand(rule.Query, connection))
                {
                    command.CommandTimeout = rule.CommandTimeoutMs / 1000;
                    var adapter = new SqlDataAdapter(command);

                    try
                    {
                        adapter.Fill(dataSet);
                    }
                    catch (Exception exception)
                    {
                        throw new UserFriendlyException(exception.Message);
                    }
                }
            }
        }

        private void FillPostrgesqlDataSet(UnitTestSqlRuleForRead rule, DataSet dataSet)
        {
            string connString;
            try
            {
                connString = GetPostgreSqlConnectionStringWithTimeout(rule);
            }
            catch (Exception exception)
            {
                throw new UserFriendlyException(exception.Message);
            }

            using (var connection = new NpgsqlConnection(connString))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception exception)
                {
                    throw new UserFriendlyException(exception.Message);
                }

                using (var command = new NpgsqlCommand(rule.Query, connection))
                {
                    command.CommandTimeout = rule.CommandTimeoutMs / 1000;
                    var adapter = new NpgsqlDataAdapter(command);

                    try
                    {
                        adapter.Fill(dataSet);
                    }
                    catch (Exception exception)
                    {
                        throw new UserFriendlyException(exception.Message);
                    }
                }
            }
        }

        private string GetMsSqlConnectionStringWithTimeout(UnitTestSqlRuleForRead rule)
        {
            var builder = new SqlConnectionStringBuilder(rule.ConnectionString);
            builder.ConnectTimeout = rule.OpenConnectionTimeoutMs / 1000;
            return builder.ConnectionString;
        }

        private string GetPostgreSqlConnectionStringWithTimeout(UnitTestSqlRuleForRead rule)
        {
            var builder = new NpgsqlConnectionStringBuilder(rule.ConnectionString);
            builder.Timeout = rule.OpenConnectionTimeoutMs / 1000;
            return builder.ConnectionString;
        }

        private UnitTestResult GetStatus(string text)
        {
            if (string.Equals("unknown", text, StringComparison.InvariantCultureIgnoreCase))
            {
                return UnitTestResult.Unknown;
            }
            if (string.Equals("success", text, StringComparison.InvariantCultureIgnoreCase))
            {
                return UnitTestResult.Success;
            }
            if (string.Equals("warning", text, StringComparison.InvariantCultureIgnoreCase))
            {
                return UnitTestResult.Warning;
            }
            if (string.Equals("alarm", text, StringComparison.InvariantCultureIgnoreCase))
            {
                return UnitTestResult.Alarm;
            }
            throw new UserFriendlyException("Неизвестное значение статуса: " + text);
        }

    }
}
