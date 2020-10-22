using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Text;

namespace Zidium.Storage.Ef
{
    internal abstract class DataContextBase : DbContext
    {
        public string ConnectionString { get; protected set; }

        public string DatabaseName { get; set; }

        protected DataContextBase(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
            ConnectionString = connection.ConnectionString;
            Prepare();
        }

        protected DataContextBase(string connectionString) : base(connectionString)
        {
            Prepare();
        }

        protected void Prepare()
        {
            // на сервере Zidium из-за openvpn отваливается периодически сеть
            // но было замечено что обычная SQL проверка с таймаутом выполнения 1 минуту всегда успешно выполнялась
            // попробуем установить такой же таймаут
            SetCommandTimeOut(TimeSpan.FromMinutes(1));
        }

        public abstract void Check();

        public abstract DbConnection CreateConnection();

        public void SetCommandTimeOut(TimeSpan timeOut)
        {
            // Get the ObjectContext related to this DbContext
            var objectContext = (this as IObjectContextAdapter).ObjectContext;

            // Sets the command timeout for all the commands
            objectContext.CommandTimeout = (int)timeOut.TotalSeconds;
        }

        public override int SaveChanges()
        {
            try
            {
                try
                {
                    return base.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var sb = new StringBuilder();
                    foreach (var failure in ex.EntityValidationErrors)
                    {
                        sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                        foreach (var error in failure.ValidationErrors)
                        {
                            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                            sb.AppendLine();
                        }
                    }

                    throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb, ex);
                }
            }
            catch (Exception exception)
            {
                exception.Data.Add("Database", DatabaseName);
                throw;
            }
        }

        public string FormatTableName(string tableName)
        {
            return Provider.Current().FormatSchemaName("dbo") + "." + Provider.Current().FormatTableName(tableName);
        }

        public string FormatColumnName(string columnName)
        {
            return Provider.Current().FormatColumnName(columnName);
        }
    }
}
