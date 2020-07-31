using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Xunit;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Tests
{
    public class WebLogsTests
    {
        /// <summary>
        /// Тест проверяет отображение лога, если не заполнен фильтр "дата"
        /// </summary>
        [Fact]
        public void IndexWithEmptyFromDateTest()
        {
            // Создадим N + 1 запись лога
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            for (var i = 1; i <= OutputRecordCount + 1; i++)
            {
                component.Log.Info(i.ToString());
            }

            component.Client.Flush();

            // Получим лог с незаполненным фильтром "дата"
            LogPartialModel model;
            var user = TestHelper.GetAccountAdminUser(account.Id);
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.Partial(new LogFiltersModel() { ComponentId = component.Info.Id }));
                model = Assert.IsType<LogPartialModel>(result.Model);
            }

            // Проверим, что мы получили последние N записей, а первую не получили
            Assert.Equal(OutputRecordCount, model.Items.Length);

            for (var i = 2; i <= OutputRecordCount + 1; i++)
            {
                Assert.True(model.Items.Any(t => t.Message == i.ToString()));
            }

            Assert.False(model.Items.Any(t => t.Message == "1"));

        }

        /// <summary>
        /// Тест проверяет отображение лога, если заполнен фильтр "дата"
        /// </summary>
        [Fact]
        public void IndexWithFilledFromDateTest()
        {
            // Создадим N + 1 запись лога
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            for (var i = 1; i <= OutputRecordCount + 1; i++)
            {
                component.Log.Info(i.ToString());
            }

            component.Client.Flush();

            // Получим лог с заполненным фильтром "дата"
            LogPartialModel model;
            var user = TestHelper.GetAccountAdminUser(account.Id);
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.Partial(new LogFiltersModel() { ComponentId = component.Info.Id, Date = DateTime.Now.AddDays(-1) }));
                model = Assert.IsType<LogPartialModel>(result.Model);
            }

            // Проверим, что мы получили первые N записей, а N + 1 не получили
            Assert.Equal(OutputRecordCount, model.Items.Length);

            for (var i = 1; i <= OutputRecordCount; i++)
            {
                Assert.True(model.Items.Any(t => t.Message == i.ToString()));
            }

            Assert.False(model.Items.Any(t => t.Message == (OutputRecordCount + 1).ToString()));

        }

        /// <summary>
        /// Тест проверяет получение предыдущих N записей
        /// </summary>
        [Fact]
        public void GetPreviousRecordsTest()
        {
            // Создадим N + 1 запись лога
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            for (var i = 1; i <= OutputRecordCount + 1; i++)
            {
                component.Log.Info(i.ToString());
            }

            component.Client.Flush();

            // Получим лог с незаполненным фильтром "дата"
            LogPartialModel model;
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var filters = new LogFiltersModel() { ComponentId = component.Info.Id };
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.Partial(filters));
                model = Assert.IsType<LogPartialModel>(result.Model);
            }

            // Проверим ссылку на предыдущие записи
            var previousRecords = model.Previous;
            Assert.NotNull(previousRecords);
            Assert.Equal(DateTimeHelper.TrimMs(model.Items[0].Date), DateTimeHelper.TrimMs(previousRecords.Date));
            Assert.Equal(model.Items[0].Order, previousRecords.Order);
            Assert.False(model.NoPreviousRecords);

            // Получим предыдущие записи
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.GetPreviousRecords(previousRecords.Date, previousRecords.Order, filters));
                model = Assert.IsType<LogPartialModel>(result.Model);
            }

            // Проверим, что получили запись 1
            Assert.Equal(1, model.Items.Length);
            Assert.Equal("1", model.Items[0].Message);
            Assert.True(model.MarkAsNew);

            // Проверим ссылку на предыдущие записи
            previousRecords = model.Previous;
            Assert.NotNull(previousRecords);
            Assert.Equal(DateTimeHelper.TrimMs(model.Items[0].Date), DateTimeHelper.TrimMs(previousRecords.Date));
            Assert.Equal(model.Items[0].Order, previousRecords.Order);
            Assert.False(model.NoPreviousRecords);

            // Ещё раз получим предыдущие записи
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.GetPreviousRecords(previousRecords.Date, previousRecords.Order, filters));
                model = Assert.IsType<LogPartialModel>(result.Model);
            }

            // Проверим, что больше предыдущих записей и ссылки нет
            Assert.Equal(0, model.Items.Length);
            Assert.Null(model.Previous);
            Assert.True(model.NoPreviousRecords);
        }

        /// <summary>
        /// Тест проверяет получение следующих N записей
        /// </summary>
        [Fact]
        public void GetNextRecords()
        {
            // Создадим N + 1 запись лога
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            for (var i = 1; i <= OutputRecordCount + 1; i++)
            {
                component.Log.Info(i.ToString());
            }

            component.Client.Flush();

            // Получим лог с заполненным фильтром "дата"
            LogPartialModel model;
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var filters = new LogFiltersModel() { ComponentId = component.Info.Id, Date = DateTime.Now.AddDays(-1) };
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.Partial(filters));
                model = Assert.IsType<LogPartialModel>(result.Model);
            }

            // Проверим ссылку на следующие записи
            var nextRecords = model.Next;
            Assert.NotNull(nextRecords);
            Assert.Equal(DateTimeHelper.TrimMs(model.Items.Last().Date), DateTimeHelper.TrimMs(nextRecords.Date));
            Assert.Equal(model.Items.Last().Order, nextRecords.Order);
            Assert.False(model.NoNextRecords);

            // Получим следующие записи
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.GetNextRecords(nextRecords.Date, nextRecords.Order, filters));
                model = Assert.IsType<LogPartialModel>(result.Model);
            }

            // Проверим, что получили запись N
            Assert.Equal(1, model.Items.Length);
            Assert.Equal((OutputRecordCount + 1).ToString(), model.Items[0].Message);
            Assert.True(model.MarkAsNew);

            // Проверим ссылку на следующие записи
            nextRecords = model.Next;
            Assert.NotNull(nextRecords);
            Assert.Equal(DateTimeHelper.TrimMs(model.Items.Last().Date), DateTimeHelper.TrimMs(nextRecords.Date));
            Assert.Equal(model.Items.Last().Order, nextRecords.Order);
            Assert.False(model.NoNextRecords);

            // Ещё раз получим следующие записи
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.GetNextRecords(nextRecords.Date, nextRecords.Order, filters));
                model = Assert.IsType<LogPartialModel>(result.Model);
            }

            // Проверим, что следующих записей нет, а ссылка есть
            Assert.Equal(0, model.Items.Length);
            var nextRecords2 = model.Next;
            Assert.NotNull(nextRecords2);
            Assert.Equal(DateTimeHelper.TrimMs(nextRecords.Date), DateTimeHelper.TrimMs(nextRecords2.Date));
            Assert.Equal(nextRecords.Order, nextRecords2.Order);
            Assert.True(model.NoNextRecords);

        }

        /// <summary>
        /// Тест проверяет фильтры лога
        /// </summary>
        [Fact]
        public void IndexFiltersTest()
        {
            // Добавим по две записи лога каждого уровня за три дня
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var now = DateTime.Now.Date;
            var context = Guid.NewGuid().ToString();

            var allLevels = new[]
            {
                Api.LogLevel.Trace,
                Api.LogLevel.Debug,
                Api.LogLevel.Info,
                Api.LogLevel.Warning,
                Api.LogLevel.Error,
                Api.LogLevel.Fatal
            };

            for (var i = -2; i <= 0; i++)
            {
                foreach (var level in allLevels)
                {
                    account.GetClient().ApiService.SendLog(new Api.SendLogData()
                    {
                        ComponentId = component.Info.Id,
                        Date = now.AddDays(i),
                        Level = level,
                        Message = "First",
                        Order = 0,
                        Context = context
                    });

                    account.GetClient().ApiService.SendLog(new Api.SendLogData()
                    {
                        ComponentId = component.Info.Id,
                        Date = now.AddDays(i),
                        Level = level,
                        Message = "Second",
                        Order = 1,
                        Context = context
                    });
                }
            }

            // Получим лог с заполненными фильтрами
            var user = TestHelper.GetAccountAdminUser(account.Id);

            var filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Date = now,
                LogLevel = LogLevel.Fatal,
                Context = context
            };

            LogPartialModel model;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.Partial(filters));
                model = Assert.IsType<LogPartialModel>(result.Model);
            }

            // Проверим, какие записи получили
            Assert.Equal(2, model.Items.Length);

            Assert.Equal(now, model.Items[0].Date);
            Assert.Equal(0, model.Items[0].Order);
            Assert.Equal(LogLevel.Fatal, model.Items[0].Level);
            Assert.Equal("First", model.Items[0].Message);

            Assert.Equal(now, model.Items[1].Date);
            Assert.Equal(1, model.Items[1].Order);
            Assert.Equal(LogLevel.Fatal, model.Items[1].Level);
            Assert.Equal("Second", model.Items[1].Message);

        }

        /// <summary>
        /// Тест проверяет поиск вперёд
        /// </summary>
        [Fact]
        public void FindNextRecordTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // Создадим 1 запись с текстом, потом N / 2 записей, ещё запись с текстом, и ещё N / 2 записей
            var text = Guid.NewGuid().ToString();

            component.Log.Info(text);

            for (var i = 2; i <= OutputRecordCount / 2 + 1; i++)
            {
                component.Log.Info(i.ToString());
            }

            component.Log.Info(text);

            for (var i = OutputRecordCount / 2 + 3; i <= OutputRecordCount + 2; i++)
            {
                component.Log.Info(i.ToString());
            }

            component.Client.Flush();

            // Получим записи лога
            var logRecords = component.GetLogs(new Api.GetLogsFilter()
            {
                Levels = new List<Api.LogLevel>() { Api.LogLevel.Info }
            }).Data.OrderBy(t => t.Date).ThenBy(t => t.Order).ToArray();

            Assert.Equal(OutputRecordCount + 2, logRecords.Length);

            // Выполним поиск после 1-й записи
            LogPartialModel model;
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Id = logRecords[0].Id,
                Text = text.ToUpper()
            };

            JsonResult jsonResult1;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                jsonResult1 = Assert.IsType<JsonResult>(controller.FindNextRecord(filters));
            }
            var success = (bool)jsonResult1.Data.GetType().GetProperty("success").GetValue(jsonResult1.Data);
            Assert.True(success);
            var jsonData = jsonResult1.Data.GetType().GetProperty("data").GetValue(jsonResult1.Data);
            Assert.NotNull(jsonData);
            var found = (bool) jsonData.GetType().GetProperty("found").GetValue(jsonData);
            Assert.True(found);
            var id = new Guid((string)jsonData.GetType().GetProperty("id").GetValue(jsonData));
            Assert.NotNull(id);

            filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Id = id,
                Text = text.ToUpper()
            };
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.Center(filters));
                model = Assert.IsType<LogPartialModel>(result.Model);
            }

            // Проверим, что получили записи с 2 до N + 1
            var foundRecord = model.Items[OutputRecordCount / 2];
            Assert.Equal(OutputRecordCount, model.Items.Length);
            Assert.Equal("2", model.Items.First().Message);
            Assert.Equal(text, foundRecord.Message);
            Assert.Equal((OutputRecordCount + 1).ToString(), model.Items.Last().Message);

            // Выполним поиск дальше
            filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Id = id,
                Text = text.ToUpper()
            };

            JsonResult jsonResult2;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                jsonResult2 = Assert.IsType<JsonResult>(controller.FindNextRecord(filters));
            }

            // Проверим, что больше ничего не найдено
            success = (bool)jsonResult2.Data.GetType().GetProperty("success").GetValue(jsonResult2.Data);
            Assert.True(success);
            jsonData = jsonResult2.Data.GetType().GetProperty("data").GetValue(jsonResult2.Data);
            Assert.NotNull(jsonData);
            found = (bool)jsonData.GetType().GetProperty("found").GetValue(jsonData);
            Assert.False(found);
            var message = (string)jsonData.GetType().GetProperty("message").GetValue(jsonData);
            Assert.NotNull(message);
            var id2 = (string)jsonData.GetType().GetProperty("id").GetValue(jsonData);
            Assert.Null(id2);

        }

        /// <summary>
        /// Тест проверяет поиск назад
        /// </summary>
        [Fact]
        public void FindPreviousRecordTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // Создадим N / 2 записей, потом 1 запись с текстом, ещё N / 2 записей, и ещё запись с текстом
            var text = Guid.NewGuid().ToString();

            for (var i = 1; i <= OutputRecordCount / 2; i++)
            {
                component.Log.Info(i.ToString());
            }

            component.Log.Info(text);

            for (var i = OutputRecordCount / 2 + 2; i <= OutputRecordCount + 1; i++)
            {
                component.Log.Info(i.ToString());
            }

            component.Log.Info(text);

            component.Client.Flush();

            // Получим записи лога
            var logRecords = component.GetLogs(new Api.GetLogsFilter()
            {
                Levels = new List<Api.LogLevel>() { Api.LogLevel.Info }
            }).Data.OrderBy(t => t.Date).ThenBy(t => t.Order).ToArray();

            Assert.Equal(OutputRecordCount + 2, logRecords.Length);

            // Выполним поиск перед последней записью
            LogPartialModel model;
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Id = logRecords.Last().Id,
                Text = text.ToUpper()
            };

            JsonResult jsonResult1;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                jsonResult1 = Assert.IsType<JsonResult>(controller.FindPreviousRecord(filters));
            }
            var success = (bool)jsonResult1.Data.GetType().GetProperty("success").GetValue(jsonResult1.Data);
            Assert.True(success);
            var jsonData = jsonResult1.Data.GetType().GetProperty("data").GetValue(jsonResult1.Data);
            Assert.NotNull(jsonData);
            var found = (bool)jsonData.GetType().GetProperty("found").GetValue(jsonData);
            Assert.True(found);
            var id = new Guid((string)jsonData.GetType().GetProperty("id").GetValue(jsonData));
            Assert.NotNull(id);

            filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Id = id,
                Text = text.ToUpper()
            };
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.Center(filters));
                model = Assert.IsType<LogPartialModel>(result.Model);
            }

            // Проверим, что получили записи с 1 до N
            var foundRecord = model.Items[OutputRecordCount / 2];
            Assert.Equal(OutputRecordCount, model.Items.Length);
            Assert.Equal("1", model.Items.First().Message);
            Assert.Equal(text, foundRecord.Message);
            Assert.Equal(OutputRecordCount.ToString(), model.Items.Last().Message);

            // Выполним поиск дальше
            filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Id = id,
                Text = text.ToUpper()
            };

            JsonResult jsonResult2;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                jsonResult2 = Assert.IsType<JsonResult>(controller.FindPreviousRecord(filters));
            }

            // Проверим, что больше ничего не найдено
            success = (bool)jsonResult2.Data.GetType().GetProperty("success").GetValue(jsonResult2.Data);
            Assert.True(success);
            jsonData = jsonResult2.Data.GetType().GetProperty("data").GetValue(jsonResult2.Data);
            Assert.NotNull(jsonData);
            found = (bool)jsonData.GetType().GetProperty("found").GetValue(jsonData);
            Assert.False(found);
            var message = (string)jsonData.GetType().GetProperty("message").GetValue(jsonData);
            Assert.NotNull(message);
            var id2 = (string)jsonData.GetType().GetProperty("id").GetValue(jsonData);
            Assert.Null(id2);

        }

        /// <summary>
        /// Тест проверяет итерацию поиска вперёд
        /// </summary>
        [Fact]
        public void FindNextIterationTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // Создадим N + 1 записей, потом 1 запись с текстом
            var searchIterationRecordCount = 10;
            var text = Guid.NewGuid().ToString();
            var date = DateTime.Now.AddMinutes(-searchIterationRecordCount - 1);

            for (var i = 1; i <= searchIterationRecordCount + 1; i++)
            {
                component.Client.ApiService.SendLog(new Api.SendLogData()
                {
                    ComponentId = component.Info.Id,
                    Level = Api.LogLevel.Info,
                    Message = i.ToString(),
                    Date = date
                });
                date = date.AddMinutes(1);
            }

            component.Client.ApiService.SendLog(new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = Api.LogLevel.Info,
                Message = text,
                Date = date
            });

            component.Client.Flush();

            // Получим записи лога
            var logRecords = component.GetLogs(new Api.GetLogsFilter()
            {
                Levels = new List<Api.LogLevel>() { Api.LogLevel.Info }
            }).Data.OrderBy(t => t.Date).ThenBy(t => t.Order).ToArray();

            Assert.Equal(searchIterationRecordCount + 2, logRecords.Length);

            // Выполним поиск с начала лога
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Date = logRecords[0].Date,
                Text = text.ToUpper()
            };

            JsonResult jsonResult1;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, searchIterationRecordCount))
            {
                jsonResult1 = Assert.IsType<JsonResult>(controller.FindNextRecord(filters));
            }

            // Проверим, что ничего не найдено, но можно искать дальше
            var success = (bool)jsonResult1.Data.GetType().GetProperty("success").GetValue(jsonResult1.Data);
            Assert.True(success);
            var jsonData = jsonResult1.Data.GetType().GetProperty("data").GetValue(jsonResult1.Data);
            Assert.NotNull(jsonData);
            var found = (bool)jsonData.GetType().GetProperty("found").GetValue(jsonData);
            Assert.False(found);
            var id = new Guid((string)jsonData.GetType().GetProperty("id").GetValue(jsonData));
            Assert.NotNull(id);

            // Проверим, что этот Id указывает на запись N
            Assert.Equal(logRecords[searchIterationRecordCount].Id, id);

            // Поищем дальше с указанного Id
            filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Date = logRecords[0].Date,
                Id = id,
                Text = text.ToUpper()
            };

            JsonResult jsonResult2;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, searchIterationRecordCount))
            {
                jsonResult2 = Assert.IsType<JsonResult>(controller.FindNextRecord(filters));
            }

            // Проверим, что запись найдена
            success = (bool)jsonResult2.Data.GetType().GetProperty("success").GetValue(jsonResult2.Data);
            Assert.True(success);
            jsonData = jsonResult2.Data.GetType().GetProperty("data").GetValue(jsonResult2.Data);
            Assert.NotNull(jsonData);
            found = (bool)jsonData.GetType().GetProperty("found").GetValue(jsonData);
            Assert.True(found);
            id = new Guid((string)jsonData.GetType().GetProperty("id").GetValue(jsonData));
            Assert.NotNull(id);

            // Проверим, что найдена именно N + 1 запись
            Assert.Equal(logRecords.Last().Id, id);

        }

        /// <summary>
        /// Тест проверяет итерацию поиска назад
        /// </summary>
        [Fact]
        public void FindPreviousIterationTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // Создадим 1 запись с текстом, потом N + 1 записей
            var searchIterationRecordCount = 10;
            var text = Guid.NewGuid().ToString();
            var date = DateTime.Now.AddMinutes(-searchIterationRecordCount - 1);

            component.Client.ApiService.SendLog(new Api.SendLogData()
            {
                ComponentId = component.Info.Id,
                Level = Api.LogLevel.Info,
                Message = text,
                Date = date
            });
            date = date.AddMinutes(1);

            for (var i = 1; i <= searchIterationRecordCount + 1; i++)
            {
                component.Client.ApiService.SendLog(new Api.SendLogData()
                {
                    ComponentId = component.Info.Id,
                    Level = Api.LogLevel.Info,
                    Message = i.ToString(),
                    Date = date
                });
                date = date.AddMinutes(1);
            }

            component.Client.Flush();

            // Получим записи лога
            var logRecords = component.GetLogs(new Api.GetLogsFilter()
            {
                Levels = new List<Api.LogLevel>() { Api.LogLevel.Info }
            }).Data.OrderBy(t => t.Date).ThenBy(t => t.Order).ToArray();

            Assert.Equal(searchIterationRecordCount + 2, logRecords.Length);

            // Выполним поиск с конца лога
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Date = logRecords.Last().Date,
                Text = text.ToUpper()
            };

            JsonResult jsonResult1;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, searchIterationRecordCount))
            {
                jsonResult1 = Assert.IsType<JsonResult>(controller.FindPreviousRecord(filters));
            }

            // Проверим, что ничего не найдено, но можно искать дальше
            var success = (bool)jsonResult1.Data.GetType().GetProperty("success").GetValue(jsonResult1.Data);
            Assert.True(success);
            var jsonData = jsonResult1.Data.GetType().GetProperty("data").GetValue(jsonResult1.Data);
            Assert.NotNull(jsonData);
            var found = (bool)jsonData.GetType().GetProperty("found").GetValue(jsonData);
            Assert.False(found);
            var id = new Guid((string)jsonData.GetType().GetProperty("id").GetValue(jsonData));
            Assert.NotNull(id);

            // Проверим, что этот Id указывает на запись 1
            Assert.Equal(logRecords[1].Id, id);

            // Поищем дальше с указанного Id
            filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Id = id,
                Date = logRecords.Last().Date,
                Text = text.ToUpper()
            };

            JsonResult jsonResult2;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, searchIterationRecordCount))
            {
                jsonResult2 = Assert.IsType<JsonResult>(controller.FindPreviousRecord(filters));
            }

            // Проверим, что запись найдена
            success = (bool)jsonResult2.Data.GetType().GetProperty("success").GetValue(jsonResult2.Data);
            Assert.True(success);
            jsonData = jsonResult2.Data.GetType().GetProperty("data").GetValue(jsonResult2.Data);
            Assert.NotNull(jsonData);
            found = (bool)jsonData.GetType().GetProperty("found").GetValue(jsonData);
            Assert.True(found);
            id = new Guid((string)jsonData.GetType().GetProperty("id").GetValue(jsonData));
            Assert.NotNull(id);

            // Проверим, что найдена именно запись 0
            Assert.Equal(logRecords[0].Id, id);

        }

        /// <summary>
        /// Тест проверяет поиск вперёд по свойствам
        /// </summary>
        [Fact]
        public void FindNextRecordByPropertyTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // Создадим N записей, потом 2 записи со свойством
            var text = Guid.NewGuid().ToString();

            for (var i = 1; i <= OutputRecordCount; i++)
            {
                component.Log.Info(i.ToString());
            }

            component.Log.Info((OutputRecordCount + 1).ToString(), new { PropName1 = text });
            component.Log.Info((OutputRecordCount + 2).ToString(), new { PropName2 = Guid.NewGuid() });

            component.Client.Flush();

            // Получим записи лога
            var logRecords = component.GetLogs(new Api.GetLogsFilter()
            {
                Levels = new List<Api.LogLevel>() { Api.LogLevel.Info }
            }).Data.OrderBy(t => t.Date).ThenBy(t => t.Order).ToArray();

            Assert.Equal(OutputRecordCount + 2, logRecords.Length);

            // Выполним поиск с начала лога по тексту свойства
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Date = logRecords[0].Date,
                Text = text.ToUpper()
            };

            JsonResult jsonResult;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                jsonResult = Assert.IsType<JsonResult>(controller.FindNextRecord(filters));
            }

            // Проверим, что нашли запись
            var success = (bool)jsonResult.Data.GetType().GetProperty("success").GetValue(jsonResult.Data);
            Assert.True(success);
            var jsonData = jsonResult.Data.GetType().GetProperty("data").GetValue(jsonResult.Data);
            Assert.NotNull(jsonData);
            var found = (bool)jsonData.GetType().GetProperty("found").GetValue(jsonData);
            Assert.True(found);
            var id = new Guid((string)jsonData.GetType().GetProperty("id").GetValue(jsonData));
            Assert.NotNull(id);

            // Проверим, что это именно запись N + 1
            Assert.Equal(logRecords[OutputRecordCount].Id, id);

            // Выполним поиск с начала лога по названию свойства
            filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Date = logRecords[0].Date,
                Text = "PROPNAME2"
            };

            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                jsonResult = Assert.IsType<JsonResult>(controller.FindNextRecord(filters));
            }

            // Проверим, что нашли запись
            success = (bool)jsonResult.Data.GetType().GetProperty("success").GetValue(jsonResult.Data);
            Assert.True(success);
            jsonData = jsonResult.Data.GetType().GetProperty("data").GetValue(jsonResult.Data);
            Assert.NotNull(jsonData);
            found = (bool)jsonData.GetType().GetProperty("found").GetValue(jsonData);
            Assert.True(found);
            id = new Guid((string)jsonData.GetType().GetProperty("id").GetValue(jsonData));
            Assert.NotNull(id);

            // Проверим, что это именно запись N + 2
            Assert.Equal(logRecords[OutputRecordCount + 1].Id, id);
        }

        /// <summary>
        /// Тест проверяет поиск назад по свойствам
        /// </summary>
        [Fact]
        public void FindPreviousRecordByPropertyTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // Создадим 2 записи со свойством, потом N записей
            var text = Guid.NewGuid().ToString();

            component.Log.Info("1", new { PropName1 = text });
            component.Log.Info("2", new { PropName2 = Guid.NewGuid() });

            for (var i = 1; i <= OutputRecordCount; i++)
            {
                component.Log.Info((i + 2).ToString());
            }

            component.Client.Flush();

            // Получим записи лога
            var logRecords = component.GetLogs(new Api.GetLogsFilter()
            {
                Levels = new List<Api.LogLevel>() { Api.LogLevel.Info }
            }).Data.OrderBy(t => t.Date).ThenBy(t => t.Order).ToArray();

            Assert.Equal(OutputRecordCount + 2, logRecords.Length);

            // Выполним поиск с конца лога по тексту свойства
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Text = text.ToUpper()
            };

            JsonResult jsonResult;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                jsonResult = Assert.IsType<JsonResult>(controller.FindPreviousRecord(filters));
            }

            // Проверим, что нашли запись
            var success = (bool)jsonResult.Data.GetType().GetProperty("success").GetValue(jsonResult.Data);
            Assert.True(success);
            var jsonData = jsonResult.Data.GetType().GetProperty("data").GetValue(jsonResult.Data);
            Assert.NotNull(jsonData);
            var found = (bool)jsonData.GetType().GetProperty("found").GetValue(jsonData);
            Assert.True(found);
            var id = new Guid((string)jsonData.GetType().GetProperty("id").GetValue(jsonData));
            Assert.NotNull(id);

            // Проверим, что это именно запись 0
            Assert.Equal(logRecords[0].Id, id);

            // Выполним поиск с конца лога по названию свойства
            filters = new LogFiltersModel()
            {
                ComponentId = component.Info.Id,
                Text = "PROPNAME2"
            };

            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                jsonResult = Assert.IsType<JsonResult>(controller.FindPreviousRecord(filters));
            }

            // Проверим, что нашли запись
            success = (bool)jsonResult.Data.GetType().GetProperty("success").GetValue(jsonResult.Data);
            Assert.True(success);
            jsonData = jsonResult.Data.GetType().GetProperty("data").GetValue(jsonResult.Data);
            Assert.NotNull(jsonData);
            found = (bool)jsonData.GetType().GetProperty("found").GetValue(jsonData);
            Assert.True(found);
            id = new Guid((string)jsonData.GetType().GetProperty("id").GetValue(jsonData));
            Assert.NotNull(id);

            // Проверим, что это именно запись 1
            Assert.Equal(logRecords[1].Id, id);
        }

        /// <summary>
        /// Тест проверяет ручное добавление записи лога
        /// </summary>
        [Fact]
        public void AddRecordTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // Получим форму добавления
            AddLogModel model;
            var user = TestHelper.GetAccountAdminUser(account.Id);
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.Add(component.Info.Id));
                model = Assert.IsType<AddLogModel>(result.Model);
            }

            // Заполним модель
            model.LogLevel = LogLevel.Fatal;
            model.Message = Guid.NewGuid().ToString();

            // Отправим данные
            JsonResult jsonResult;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                jsonResult = Assert.IsType<JsonResult>(controller.Add(model));
            }

            var success = (bool)jsonResult.Data.GetType().GetProperty("success").GetValue(jsonResult.Data);
            Assert.True(success);

            // Проверим, что запись лога появилась
            var logRecords = component.GetLogs(new Api.GetLogsFilter()
            {
                Levels = new List<Api.LogLevel>() { Api.LogLevel.Fatal}
            }).Data;

            Assert.True(logRecords.Any(t => t.Message == model.Message));
        }

        /// <summary>
        /// Тест проверяет изменение настроек Web-лога
        /// </summary>
        [Fact]
        public void EditLevelsTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // Получим форму настроек
            EditLogModel model;
            var user = TestHelper.GetAccountAdminUser(account.Id);
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                var result = Assert.IsType<PartialViewResult>(controller.Edit(component.Info.Id));
                model = Assert.IsType<EditLogModel>(result.Model);
            }

            Assert.Equal(component.WebLogConfig.IsTraceEnabled, model.IsTraceEnabled);
            Assert.Equal(component.WebLogConfig.IsDebugEnabled, model.IsDebugEnabled);
            Assert.Equal(component.WebLogConfig.IsInfoEnabled, model.IsInfoEnabled);
            Assert.Equal(component.WebLogConfig.IsWarningEnabled, model.IsWarningEnabled);
            Assert.Equal(component.WebLogConfig.IsErrorEnabled, model.IsErrorEnabled);
            Assert.Equal(component.WebLogConfig.IsFatalEnabled, model.IsFatalEnabled);


            // Заполним модель
            model.IsTraceEnabled = false;
            model.IsDebugEnabled = false;
            model.IsInfoEnabled = false;
            model.IsWarningEnabled = false;
            model.IsErrorEnabled = false;
            model.IsFatalEnabled = false;

            // Отправим данные
            JsonResult jsonResult;
            using (var controller = new LogsController(account.Id, user.Id, OutputRecordCount, SearchIterationRecordCount))
            {
                jsonResult = Assert.IsType<JsonResult>(controller.Edit(model));
            }

            var success = (bool)jsonResult.Data.GetType().GetProperty("success").GetValue(jsonResult.Data);
            Assert.True(success);

            // Проверим, что настройки поменялись
            using (var context = TestHelper.GetAccountDbContext(account.Id))
            {
                var config = context.LogConfigs.Find(component.Info.Id);
                Assert.NotNull(config);
                Assert.False(config.IsTraceEnabled);
                Assert.False(config.IsDebugEnabled);
                Assert.False(config.IsInfoEnabled);
                Assert.False(config.IsWarningEnabled);
                Assert.False(config.IsErrorEnabled);
                Assert.False(config.IsFatalEnabled);
            }
        }

        private static readonly int OutputRecordCount = 20;

        private static readonly int SearchIterationRecordCount = 100;

    }
}
