using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Zidium.Api;
using Zidium.Api.XmlConfig;

namespace Zidium.TestTools
{
    public class TestAccountInfo
    {
        public string SecretKey { get; set; }

        public string SystemName { get; set; }

        public AccessToken Token { get; set; }

        public void SaveAllCaches()
        {
            // Сейчас нет способа через Api сохранить кеш аккаунта
            // Поэтому просто подождём 1 минуту, чтобы диспетчер сделал это по таймеру
            Thread.Sleep(TimeSpan.FromMinutes(1));
        }

        public Config Config = new Config();

        private IClient _client = null;

        public IClient GetClient()
        {
            if (_client == null)
            {
                Config.Access.AccountName = SystemName;
                Config.Access.SecretKey = SecretKey;
                Config.Logs.AutoCreateEvents.Disable = true; // To avoid side effects
                _client = new Client(Config);
            }
            return _client;
        }

        public IComponentControl CreateRandomComponentControl()
        {
            var client = GetClient();
            var root = client.GetRootComponentControl();
            var message = TestHelper.GetRandomGetOrCreateComponentData(client);
            var newComponent = root.GetOrCreateChildComponentControl(message);
            return newComponent;
        }

        // TODO Сделать нормальный способ управления настройками лога
        public void SetComponentLogConfigIsTraceEnabled(Guid componentId, bool value)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "UPDATE [dbo].[LogConfigs] SET [IsTraceEnabled] = @IsTraceEnabled, [LastUpdateDate] = GetDate() WHERE [ComponentId] = @ComponentId";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "IsTraceEnabled";
                    parameter.DbType = DbType.Boolean;
                    parameter.Value = value;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "ComponentId";
                    parameter.DbType = DbType.Guid;
                    parameter.Value = componentId;
                    command.Parameters.Add(parameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        // TODO Сделать нормальный способ управления настройками лога
        public void SetComponentLogConfigIsInfoEnabled(Guid componentId, bool value)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "UPDATE [dbo].[LogConfigs] SET [IsInfoEnabled] = @IsInfoEnabled, [LastUpdateDate] = GetDate() WHERE [ComponentId] = @ComponentId";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "IsInfoEnabled";
                    parameter.DbType = DbType.Boolean;
                    parameter.Value = value;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "ComponentId";
                    parameter.DbType = DbType.Guid;
                    parameter.Value = componentId;
                    command.Parameters.Add(parameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    var appDir = Tools.GetApplicationDir();

                    var builder = new ConfigurationBuilder()
                        .SetBasePath(appDir)
                        .AddJsonFile("appsettings.json", true);

                    var configuration = builder.Build();
                    _connectionString = configuration.GetConnectionString("Database");
                }
                return _connectionString;
            }
        }

        private static string _connectionString;

    }
}
