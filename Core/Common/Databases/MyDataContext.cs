using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Text;

namespace Zidium.Core.Common
{
    public abstract class MyDataContext : DbContext
    {
        public string ConnectionString { get; protected set; }

        protected MyDataContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
            ConnectionString = connection.ConnectionString;
            MyInit();
        }

        protected MyDataContext(string connectionString) : base(connectionString)
        {
            var conString = ConfigurationManager.ConnectionStrings[connectionString];
            if (conString == null)
            {
                ConnectionString = connectionString;
            }
            else
            {
                ConnectionString = conString.ConnectionString;
            }
            MyInit();
        }

        protected void MyInit()
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
    }
}
