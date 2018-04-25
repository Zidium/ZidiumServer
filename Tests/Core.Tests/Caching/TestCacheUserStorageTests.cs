using System;
using System.Linq;
using System.Threading;
using Xunit;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Caching
{
    public class TestCacheUserStorageTests
    {
        /// <summary>
        /// Тест проверяет, что блокировки объектов выполняются корректно
        /// </summary>
        [Fact]
        void SynchTest()
        {
            var userCount = 1 * 1000;
            var threadCount = 30;
            var updateCount = 1 * 1000;
            var storageMaxCount = userCount / 10;

            // storageMaxCount = 50;

            var cache = new TestCacheUserStorage(storageMaxCount);
            cache.CreateUsers(userCount);

            // проверим, что все пользователи загружаются
            var users = cache.GetAllUsers();
            foreach (var user in users)
            {
                var user2 = cache.Find(new CacheRequest() { ObjectId = user.Id });
                Assert.False(ReferenceEquals(user, user2));
                Assert.Equal(user.Id, user2.Id);
                Assert.Equal(user.Name, user2.Name);
                Assert.Equal(user.Money, user2.Money);
            }

            // запустим на выполнение N потоков
            var threads = new Thread[threadCount];
            var hotUsers = users.Take(20).ToList();
            for (var i = 0; i < threadCount; i++)
            {
                var thread = new Thread(() =>
                {
                    for (var j = 0; j < updateCount; j++)
                    {
                        Assert.True(cache.Count <= cache.MaxCount);
                        var hotUser = RandomHelper.GetRandomItemFromList(hotUsers);
                        var simpleUser = RandomHelper.GetRandomItemFromList(users);
                        var dbUsers = new[] { hotUser, simpleUser };
                        foreach (var dbUser in dbUsers)
                        {
                            var request = new CacheRequest() { ObjectId = dbUser.Id };
                            var user = cache.Find(request);
                            Assert.NotNull(user);
                            Assert.Equal(user.Id, dbUser.Id);
                            Assert.Equal(user.Name, dbUser.Name);
                            using (var writeUser = cache.Write(request))
                            {
                                if (dbUser.Money2 == writeUser.Money)
                                {
                                    writeUser.Money++;
                                    dbUser.Money2++;
                                    writeUser.BeginSave();
                                }
                                else
                                {
                                    throw new Exception("dbUser.Money2 == writeUser.Money");
                                }
                            }
                        }
                    }
                });
                thread.Name = "my thread " + i;
                thread.Start();
                threads[i] = thread;
            }

            // ждем выполнения всех потоков
            foreach (var thread in threads)
            {
                thread.Join();
            }

            cache.SaveChanges();
            var changedCount = cache.GetChangedCount();

            long mySaveCount = users.Sum(x => x.SaveCount);
            long realSaveCount = cache.SaveCount;
            long maxSaveCount = threadCount * updateCount * 2;
            Assert.Equal(mySaveCount, realSaveCount);

            //Assert.Equal(0, changedCount);

            var sum = 0;
            foreach (var user in users)
            {
                var user2 = cache.Find(new CacheRequest() { ObjectId = user.Id });
                if (user2.Money != user.Money2)
                {
                    throw new Exception("user2.Money != user.Money2");
                }
                if (user.Money != user.Money2)
                {
                    throw new Exception("user.Money != user.Money2");
                }
                if (user.SaveCount > user.Money2)
                {
                    throw new Exception("user.SaveCount > user.Money2");
                }
                sum += user2.Money;
            }
            var validSum = updateCount * threadCount * 2;
            Assert.Equal(validSum, sum);
        }

        /// <summary>
        /// Тест проверяет наложение блокировок
        /// </summary>
        [Fact]
        public void LockTest()
        {
            var userCount = 1 * 1000;
            var storageMaxCount = userCount;

            var cache = new TestCacheUserStorage(storageMaxCount);
            cache.BeginUnloadCount = int.MaxValue;
            cache.StopUnloadCount = int.MaxValue;
            cache.CreateUsers(userCount);
            var users = cache.GetAllUsers();

            var request = new CacheRequest()
            {
                ObjectId = users.First().Id
            };

            CacheLock lockObj = null;

            // чтение потом запись
            var readObj1 = cache.Read(request);

            lockObj = ((TestCacheUserWriteObject)readObj1).Response.Lock;
            Assert.False(lockObj.IsEntered());
            using (var writeObj = cache.Write(request))
            {
                Assert.True(lockObj.IsEntered());
            }
            Assert.False(lockObj.IsEntered());

            // запись потом чтение
            using (var writeObj1 = cache.Write(request))
            {
                Assert.True(lockObj.IsEntered());
                var readObj2 = cache.Read(request);
                Assert.True(lockObj.IsEntered());
                Assert.True(lockObj.IsEntered());
            }
            Assert.False(lockObj.IsEntered());

            // чтение потом чтение
            using (var writeObj1 = cache.Write(request))
            {
                Assert.True(lockObj.IsEntered());
                Assert.Equal(1, lockObj.Count);
                using (var writeObj2 = cache.Write(request))
                {
                    Assert.True(lockObj.IsEntered());
                    Assert.Equal(2, lockObj.Count);
                    Assert.True(ReferenceEquals(writeObj1, writeObj2));
                }
                Assert.True(lockObj.IsEntered());
                Assert.Equal(1, lockObj.Count);
            }
            Assert.False(lockObj.IsEntered());
            Assert.Equal(0, lockObj.Count);
        }

        [Fact]
        public void LockTest2()
        {
            var userCount = 1 * 1000;
            var storageMaxCount = userCount;

            var cache = new TestCacheUserStorage(storageMaxCount);
            cache.BeginUnloadCount = int.MaxValue;
            cache.StopUnloadCount = int.MaxValue;
            cache.CreateUsers(userCount);
            var users = cache.GetAllUsers();

            var request = new CacheRequest()
            {
                ObjectId = users.First().Id
            };

            CacheLock lockObj = null;
            var thread1 = new Thread(() =>
            {
                var user1 = cache.Write(request);
                lockObj = user1.Response.Lock;
                Thread.Sleep(10 * 1000);
            });
            thread1.Start();

            // ждем блокировки
            TestHelper.WaitTrue(
                () => lockObj != null,
                TimeSpan.FromSeconds(2));

            Assert.True(lockObj.IsLocked);

            // убедимся, что заблокировать объект НЕ получится
            Assert.False(lockObj.TryEnter(TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public void HasChangesTest()
        {
            var userCount = 10;
            var storageMaxCount = userCount;
            var cache = new TestCacheUserStorage(storageMaxCount);
            cache.CreateUsers(userCount);
            
            // проверим, что все пользователи загружаются
            var users = cache.GetAllUsers();
            Assert.Equal(10, users.Count);

            // изменений нет
            foreach (var user in users)
            {
                using (var writeUser = cache.Write(user))
                {
                    Assert.False(writeUser.Response.HasChanges);
                }
            }

            // изменения есть, но они не закамичены
            foreach (var user in users)
            {
                using (var writeUser = cache.Write(user))
                {
                    writeUser.Name = writeUser.Name + "_chanegd";
                    Assert.False(writeUser.Response.HasChanges); // НЕТ! потому что НЕ закамичено
                    writeUser.BeginSave();
                }
            }

            cache.SaveChanges();

            // изменений нет
            foreach (var user in users)
            {
                using (var writeUser = cache.Write(user))
                {
                    Assert.False(writeUser.Response.HasChanges);
                }
            }

            
            foreach (var user in users)
            {
                using (var writeUser = cache.Write(user))
                {
                    // изменений нет
                    Assert.False(writeUser.Response.HasChanges);

                    // изменения есть
                    writeUser.Name = writeUser.Name + "_2";
                    Assert.False(writeUser.Response.HasChanges); // НЕТ! потому что НЕ закамичено
                }
            }

            // изменений нет (не было сохранения = writeUser.BeginSave();)
            foreach (var user in users)
            {
                using (var writeUser = cache.Write(user))
                {
                    Assert.False(writeUser.Response.HasChanges);
                    writeUser.Name = writeUser.Name + "_2";
                    Assert.False(writeUser.Response.HasChanges); // НЕТ! потому что НЕ закамичено
                    writeUser.WaitSaveChanges();
                    // изменения сохраняются сразу же
                    Assert.False(writeUser.Response.HasChanges);
                }
            }
        }

        /// <summary>
        /// Проверяет выгрузку объектов
        /// </summary>
        [Fact]
        void UnloadTest()
        {
            var userCount = 1000;
            var threadCount = 20;
            var processCount = 1000;
            var storageMaxCount = userCount;

            var cache = new TestCacheUserStorage(storageMaxCount);
            cache.LoadTime = TimeSpan.FromMilliseconds(25);
            cache.CreateUsers(userCount);
            cache.BeginUnloadCount = 500;
            cache.StopUnloadCount = 250;

            // проверим, что все пользователи загружаются
            var users = cache.GetAllUsers();

            // запустим на выполнение N потоков
            var threads = new Thread[threadCount];
            var hotUsers = users.Take(threadCount).ToList();
            var mainUsers = users.ToList();

            foreach (var user in hotUsers)
            {
                user.IsHot = true;
                mainUsers.Remove(user);
            }

            for (var i = 0; i < threadCount; i++)
            {
                var threadIndex = i;
                var thread = new Thread(() =>
                {
                    for (var j = 0; j < processCount; j++)
                    {
                        Assert.True(cache.Count <= cache.MaxCount);
                        var hotUser = hotUsers[threadIndex];
                        var mainUser = RandomHelper.GetRandomItemFromList(mainUsers);
                        var dbUsers = new[] { hotUser, mainUser };
                        foreach (var dbUser in dbUsers)
                        {
                            var request = new CacheRequest() { ObjectId = dbUser.Id };
                            var user = cache.Find(request);
                            Assert.NotNull(user);
                            Assert.Equal(user.Id, dbUser.Id);
                            Assert.Equal(user.Name, dbUser.Name);
                            if (dbUser.IsHot)
                            {
                                Assert.Equal(dbUser.LoadCount, 1);
                            }
                        }
                    }
                });
                thread.Name = "my thread " + i;
                thread.Start();
                threads[i] = thread;
            }

            // ждем выполнения всех потоков
            foreach (var thread in threads)
            {
                thread.Join();
            }

            var saved = cache.SaveChanges();
            Assert.Equal(0, saved);

            // горячие пользователи не должны были выгружаться
            foreach (var user in hotUsers)
            {
                Assert.Equal(1, user.LoadCount);
            }
        }
    }
}
