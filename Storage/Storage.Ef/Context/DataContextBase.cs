using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Zidium.Storage.Ef
{
    internal abstract class DataContextBase : DbContext
    {
        public string ConnectionString { get; protected set; }

        public bool ContextOwnsConnection { get; protected set; }

        public DbConnection Connection { get; protected set; }

        protected DataContextBase(DbConnection connection, bool contextOwnsConnection)
        {
            Connection = connection;
            ConnectionString = connection.ConnectionString;
            ContextOwnsConnection = contextOwnsConnection;
        }

        protected DataContextBase(string connectionString)
        {
            ConnectionString = connectionString;
        }

        // For migrations
        protected DataContextBase(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public abstract void Check();

        public virtual DbConnection CreateConnection()
        {
            if (Database.CurrentTransaction != null)
                return Database.GetDbConnection();
            return null;
        }

        public void ReleaseConnection(DbConnection connection)
        {
            if (connection != null && Database.CurrentTransaction == null)
                connection.Dispose();
        }

        public override int SaveChanges()
        {
            Validate();
            return base.SaveChanges();
        }

        public void Validate()
        {
            var entities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity);

            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(entity, validationContext, validateAllProperties: true);
            }
        }

        public string FormatTableName(string tableName)
        {
            var schemaName = Provider.Instance().FormatSchemaName("dbo");
            return (schemaName != null ? schemaName + "." : null) + Provider.Instance().FormatTableName(tableName);
        }

        public string FormatColumnName(string columnName)
        {
            return Provider.Instance().FormatColumnName(columnName);
        }
    }
}
