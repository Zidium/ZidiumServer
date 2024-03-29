﻿using System;
using Xunit;
using Zidium.Common;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Caching
{
    public class StatusDataCacheStorageTests : BaseTest
    {
        /// <summary>
        /// При добавлении объекта в контекст была проблема
        /// System.InvalidOperationException: A referential integrity constraint violation occurred: The property value(s) of 'StatusData.Id' on one end of a relationship do not match the property value(s) of 'StatusData.LastChildStatusDataId' on the other end.
        /// </summary>
        [Fact]
        public void AttachTest()
        {
            // интересная статья на эту тему
            // http://ethereal-developer.blogspot.ru/2014/11/a-referential-integrity-constraint.html
            var account = TestHelper.GetTestAccount();
            using (var accountDbContext = account.GetDbContext())
            {
                accountDbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                var child = new DbBulb()
                {
                    Id = Ulid.NewUlid(),
                    CreateDate = DateTime.Now
                };
                accountDbContext.Bulbs.Attach(child);

                var parent1 = new DbBulb()
                {
                    Id = Ulid.NewUlid(),
                    CreateDate = DateTime.Now,
                    LastChildBulbId = child.Id
                };
                accountDbContext.Bulbs.Attach(parent1);
               
                parent1.LastChildBulbId = Ulid.NewUlid();

                var parent2 = new DbBulb()
                {
                    Id = Ulid.NewUlid(),
                    CreateDate = DateTime.Now,
                    LastChildBulbId = child.Id
                };
                accountDbContext.Bulbs.Attach(parent2);
            }
        }
    }
}
