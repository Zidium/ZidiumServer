﻿using System;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Zidium.Storage.Ef
{
    internal class MsSqlAccountDbContext : AccountDbContext
    {
        public MsSqlAccountDbContext(
            DbConnection connection,
            Action<DbContextOptionsBuilder> optionsBuilderAction = null
            ) : base(connection, optionsBuilderAction)
        {
        }

        // For migrations
        public MsSqlAccountDbContext(DbContextOptions<MsSqlAccountDbContext> dbContextOptions) : base(dbContextOptions) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (Connection != null)
                optionsBuilder.UseSqlServer(Connection, t => t.CommandTimeout(60));
            else if (ConnectionString != null)
                optionsBuilder.UseSqlServer(ConnectionString, t => t.CommandTimeout(60));
        }

        public override DbConnection CreateConnection()
        {
            return base.CreateConnection() ?? new SqlConnection(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
