using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using NLog;
using Zidium.Core;
using Zidium.Core.Api;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;

namespace Zidium.Agent.AgentTasks
{
    public class SqlCheckProcessor : UnitTestProcessorBase
    {
        public SqlCheckProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestTypes.SqlTestType.Id;
        }

        protected override SendUnitTestResultRequestData GetResult(
            Guid accountId, 
            AccountDbContext accountDbContext, 
            UnitTest unitTest,
            ILogger logger,
            CancellationToken token)
        {
            return CheckSql(unitTest.SqlRule);
        }

        public static string GetConnectionStringWithTimeout(UnitTestSqlRule rule)
        {
            var builder = new SqlConnectionStringBuilder(rule.ConnectionString);
            builder.ConnectTimeout = rule.OpenConnectionTimeoutMs/1000;
            return builder.ConnectionString;
        }

        protected static UnitTestResult GetStatus(string text)
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

        public static SendUnitTestResultRequestData CheckSql(UnitTestSqlRule rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }
            SqlConnection connection = null;
            string error = "Неизвестная ошибка";
            try
            {
                if (rule.Provider != DatabaseProviderType.MsSql)
                {
                    throw new UserFriendlyException("Не поддерживается провайдер " + rule.Provider);
                }
                error = "Неверный формат строки соединения";
                var connString = GetConnectionStringWithTimeout(rule);
                connection = new SqlConnection(connString);

                error = "Не удалось открыть соединение";
                connection.Open();

                error = "Не удалось выполнить запрос";
                var command = new SqlCommand(rule.Query, connection);
                command.CommandTimeout = rule.CommandTimeoutMs/1000;
                var adapter = new SqlDataAdapter(command);
                var dataSet = new DataSet();
                adapter.Fill(dataSet);

                error = "Неверный формат ответа";
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
                string statusText = row[0].ToString();
                string message = row[1].ToString();
                var status = GetStatus(statusText);
                return new SendUnitTestResultRequestData()
                {
                    Message = message,
                    Result = status
                };
            }
            catch (Exception exception)
            {
                return new SendUnitTestResultRequestData()
                {
                    Message = error + ": " + exception.Message,
                    Result = UnitTestResult.Alarm
                };
            }
        }

        
    }
}
