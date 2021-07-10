using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.Limits;
using Xunit;
using Zidium.Storage;
using Zidium.TestTools;
using Microsoft.EntityFrameworkCore;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Single.Tests
{
    public class TariffLimitTests : BaseTest
    {
        [Fact]
        public void StorageSizeTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var component = account.CreateTestApplicationComponent();
            var unitTest = TestHelper.CreateTestUnitTest(component.Id);

            // Подготовим по одному запросу: проверка, событие, лог, метрика

            var unitTestResultRequest = new SendUnitTestResultRequestDto()
            {
                Token = account.Token,
                Data = new SendUnitTestResultRequestDataDto()
                {
                    UnitTestId = unitTest.Id,
                    Result = UnitTestResult.Success,
                    Properties = new List<ExtentionPropertyDto>()
                }
            };
            unitTestResultRequest.Data.Properties.AddValue("Property", "Value");

            var eventRequest = new SendEventRequestDto()
            {
                Token = account.Token,
                Data = new SendEventRequestDataDto()
                {
                    ComponentId = component.Id,
                    Category = SendEventCategory.ApplicationError,
                    Importance = EventImportance.Alarm,
                    TypeSystemName = Guid.NewGuid().ToString(),
                    Properties = new List<ExtentionPropertyDto>()
                }
            };
            eventRequest.Data.Properties.AddValue("Property", "Value");

            var logRequest = new SendLogRequestDto()
            {
                Token = account.Token,
                Data = new SendLogRequestDataDto()
                {
                    ComponentId = component.Id,
                    Message = "Test",
                    Properties = new List<ExtentionPropertyDto>()
                }
            };
            logRequest.Data.Properties.AddValue("Property", "Value");

            var metricRequest = new SendMetricRequestDto()
            {
                Token = account.Token,
                Data = new SendMetricRequestDataDto()
                {
                    ComponentId = component.Id,
                    Name = Guid.NewGuid().ToString(),
                    Value = new Random().NextDouble()
                }
            };

            // Запомним текущий использованный лимит хранилища
            var limitsResponse = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.Token,
                Data = new GetAccountLimitsRequestData()
            });
            var usedLimits = limitsResponse.GetDataAndCheck();
            var prevStorageSize = usedLimits.UsedToday.StorageSize;

            // Проверим, что запросы успешно выполняются

            var apiCheckResultResponse = dispatcher.SendUnitTestResult(unitTestResultRequest);
            Assert.True(apiCheckResultResponse.Success);

            var eventResponse = dispatcher.SendEvent(eventRequest);
            Assert.True(eventResponse.Success);

            var logResponse = dispatcher.SendLog(logRequest);
            Assert.True(logResponse.Success);

            var metricResponse = dispatcher.SendMetric(metricRequest);
            Assert.True(metricResponse.Success);

            // Получим лимиты аккаунта
            limitsResponse = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.Token,
                Data = new GetAccountLimitsRequestData()
            });
            usedLimits = limitsResponse.GetDataAndCheck();

            // Проверим, что использованный лимит хранилища правильный
            var totalSize = unitTestResultRequest.Data.GetSize() +
                eventRequest.Data.GetSize() +
                logRequest.Data.GetSize() +
                metricRequest.Data.GetSize();
            Assert.Equal(prevStorageSize + totalSize, usedLimits.UsedToday.StorageSize);

        }

        [Fact]
        public void MaxArchiveDaysTest()
        {
            var now = DateTime.Now.Date;
            var checker = new AccountLimitsChecker(now);

            var storage = TestHelper.GetStorage();

            // Отправляем данные
            checker.AddLogSizePerDay(storage, 100000);
            checker.AddEventsSizePerDay(storage, 10000);
            checker.AddUnitTestsSizePerDay(storage, 1000);
            checker.AddMetricsSizePerDay(storage, 100);

            // TODO Override config data

            // Получим использованные лимиты и проверим, что размер хранилища правильный
            var usedLimits = checker.GetUsedOverallTariffLimit(storage, 30).Total;
            Assert.Equal(111100, usedLimits.StorageSize);

            // Прошёл один день
            now = now.AddDays(1);
            checker.NowOverride = now;

            // Сохраним данные
            checker.SaveData(storage);

            // Проверим размер хранилища
            usedLimits = checker.GetUsedOverallTariffLimit(storage, 30).Total;
            Assert.Equal(111100, usedLimits.StorageSize);

            // Проверим размер хранилища через 4 дня
            now = now.AddDays(4);
            checker.NowOverride = now;
            usedLimits = checker.GetUsedOverallTariffLimit(storage, 30).Total;
            Assert.Equal(111000, usedLimits.StorageSize);

            // Проверим размер хранилища через 5 дней
            now = now.AddDays(1);
            checker.NowOverride = now;
            usedLimits = checker.GetUsedOverallTariffLimit(storage, 30).Total;
            Assert.Equal(110000, usedLimits.StorageSize);

            // Проверим размер хранилища через 6 дней
            now = now.AddDays(1);
            checker.NowOverride = now;
            usedLimits = checker.GetUsedOverallTariffLimit(storage, 30).Total;
            Assert.Equal(100000, usedLimits.StorageSize);

            // Проверим размер хранилища через 7 дней
            now = now.AddDays(1);
            checker.NowOverride = now;
            usedLimits = checker.GetUsedOverallTariffLimit(storage, 30).Total;
            Assert.Equal(0, usedLimits.StorageSize);
        }

        [Fact]
        public void ArchiveDataTest()
        {
            var account = TestHelper.GetTestAccount();
            var today = DateTime.Now.Date;
            var storage = TestHelper.GetStorage();

            // 30 дней назад
            var date30 = today.AddDays(-30);
            var checker = new AccountLimitsChecker(date30);

            checker.AddLogSizePerDay(storage, 100);
            checker.NowOverride = date30.AddDays(1);
            checker.SaveData(storage);

            // 1 день назад
            var date1 = today.AddDays(-1);
            checker.NowOverride = date1;

            checker.AddLogSizePerDay(storage, 200);
            checker.NowOverride = date1.AddDays(1);
            checker.SaveData(storage);

            // Проверим, что сегодня есть архив за 30 дней
            checker.NowOverride = today;

            var archive = checker.GetUsedOverallTariffLimit(storage, 30).Archive;
            Assert.Equal(2, archive.Length);
            Assert.NotNull(archive.FirstOrDefault(t => t.Date == date30 && t.Info.LogSize == 100));
            Assert.NotNull(archive.FirstOrDefault(t => t.Date == date1 && t.Info.LogSize == 200));
        }

        [Fact]
        public void LimitDatasPer5MinLoadTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var unitTest1 = TestHelper.CreateTestUnitTest(component.Id);
            var unitTest2 = TestHelper.CreateTestUnitTest(component.Id);
            var storage = TestHelper.GetStorage();

            // Выровняем сдвиг времени по границе N минут
            var now = DateTime.Now.Date.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            var checker = new AccountLimitsChecker(now);

            // Отправим данные по первой проверке
            checker.AddUnitTestResultsPerDay(storage, unitTest1.Id);

            // Отправим запись лога
            checker.AddLogSizePerDay(storage, 100);

            // Прошло N минут
            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            // Отправим данные по второй проверке до лимита
            checker.AddUnitTestResultsPerDay(storage, unitTest2.Id);

            // Прошло N минут
            checker.NowOverride = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);

            // Сохраним данные
            var count = checker.SaveData(storage);
            Assert.Equal(2, count);

            // Перечитаем данные из базы
            var nowOverride = checker.NowOverride;
            checker = new AccountLimitsChecker(nowOverride);

            var usedLimits = checker.GetUsedTodayTariffLimit(storage);

            // Проверим, что лимит по первой проверке не потерялся
            Assert.Equal(1, usedLimits.UnitTestsResults.Where(t => t.UnitTestId == unitTest1.Id).Select(t => t.ApiChecksResults).First());

            // Проверим, что лимит по второй проверке не потерялся
            Assert.Equal(1, usedLimits.UnitTestsResults.Where(t => t.UnitTestId == unitTest2.Id).Select(t => t.ApiChecksResults).First());

            // Проверим, что лимит по логу не потерялся
            Assert.Equal(100, usedLimits.LogSize);

        }

        [Fact]
        public void LimitDatasPer5MinSaveTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var unitTest1 = TestHelper.CreateTestUnitTest(component.Id);
            var unitTest2 = TestHelper.CreateTestUnitTest(component.Id);
            var storage = TestHelper.GetStorage();

            // Запомним количество строк в истории
            int limitDatasCount;
            using (var accountContext = TestHelper.GetDbContext())
            {
                limitDatasCount = accountContext.LimitDatas.Count();
            }

            // Выровняем сдвиг времени по границе N минут
            var now = DateTime.Now.Date.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            var checker = new AccountLimitsChecker(now);

            // Добавим данные
            checker.AddUnitTestResultsPerDay(storage, unitTest1.Id);
            checker.AddUnitTestResultsPerDay(storage, unitTest2.Id);
            checker.AddEventsRequestsPerDay(storage);
            checker.AddEventsSizePerDay(storage, 100);

            // Прошло N минут
            var oldNow = now;
            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            // Вызовем сохранение
            var count = checker.SaveData(storage);
            Assert.Equal(1, count);

            // Проверим, что появилась новая строка в истории
            using (var accountContext = TestHelper.GetDbContext())
            {
                var rows = accountContext.LimitDatas;
                var limitDatasCount2 = rows.Count();
                Assert.True(limitDatasCount2 > limitDatasCount);

                // Получим последнюю строку
                var data = rows.OrderByDescending(t => t.BeginDate).FirstOrDefault();
                Assert.NotNull(data);

                // Проверим, как записались данные
                var beginDate = new DateTime(oldNow.Year, oldNow.Month, oldNow.Day, oldNow.Hour, (oldNow.Minute / AccountLimitsChecker.LimitDataTimeStep) * AccountLimitsChecker.LimitDataTimeStep, 0);
                Assert.Equal(beginDate, data.BeginDate);
                Assert.Equal(beginDate.AddMinutes(AccountLimitsChecker.LimitDataTimeStep), data.EndDate);
                Assert.Contains(data.UnitTestData, t => t.UnitTestId == unitTest1.Id && t.ResultsCount == 1);
                Assert.Contains(data.UnitTestData, t => t.UnitTestId == unitTest2.Id && t.ResultsCount == 1);
                Assert.Equal(1, data.EventsRequests);

                limitDatasCount = limitDatasCount2;
            }

            // Добавим ещё данные
            checker.AddUnitTestResultsPerDay(storage, unitTest2.Id);
            checker.AddEventsRequestsPerDay(storage);
            checker.AddEventsSizePerDay(storage, 100);

            // Прошло N минут
            oldNow = now;
            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            // Затребуем данные по лимитам
            checker.GetUsedTodayTariffLimit(storage);

            // Вызовем сохранение
            count = checker.SaveData(storage);
            Assert.Equal(1, count);

            // Проверим, что появилась новая строка в истории
            using (var accountContext = TestHelper.GetDbContext())
            {
                var rows = accountContext.LimitDatas;
                var limitDatasCount2 = rows.Count();
                Assert.True(limitDatasCount2 > limitDatasCount);

                // Получим последнюю строку
                var data = rows.OrderByDescending(t => t.BeginDate).FirstOrDefault();
                Assert.NotNull(data);

                // Проверим, как записались данные
                var beginDate = new DateTime(oldNow.Year, oldNow.Month, oldNow.Day, oldNow.Hour, (oldNow.Minute / AccountLimitsChecker.LimitDataTimeStep) * AccountLimitsChecker.LimitDataTimeStep, 0);
                Assert.Equal(beginDate, data.BeginDate);
                Assert.Equal(beginDate.AddMinutes(AccountLimitsChecker.LimitDataTimeStep), data.EndDate);
                Assert.DoesNotContain(data.UnitTestData, t => t.UnitTestId == unitTest1.Id);
                Assert.Contains(data.UnitTestData, t => t.UnitTestId == unitTest2.Id && t.ResultsCount == 1);
                Assert.Equal(1, data.EventsRequests);
                Assert.Equal(100, data.EventsSize);
            }

            // Перечитаем данные из базы
            checker = new AccountLimitsChecker(now);

            // Проверим сохранение пустой записи
            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            count = checker.SaveData(storage);
            Assert.Equal(1, count);

            // Проверим сохранение сразу 2 записей
            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            checker.AddUnitTestResultsPerDay(storage, unitTest1.Id);
            checker.AddUnitTestResultsPerDay(storage, unitTest2.Id);
            checker.AddEventsRequestsPerDay(storage);
            checker.AddEventsSizePerDay(storage, 100);

            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            checker.AddUnitTestResultsPerDay(storage, unitTest1.Id);
            checker.AddUnitTestResultsPerDay(storage, unitTest2.Id);
            checker.AddEventsRequestsPerDay(storage);
            checker.AddEventsSizePerDay(storage, 100);

            count = checker.SaveData(storage);
            Assert.Equal(2, count);
            count = checker.SaveData(storage);
            Assert.Equal(0, count);
        }

        [Fact]
        public void CrossTodayBoundTest()
        {
            var account = TestHelper.GetTestAccount();
            var storage = TestHelper.GetStorage();

            // Установим время на конец дня (23:59)
            var now = DateTime.Now.Date.AddMinutes(-1);
            var checker = new AccountLimitsChecker(now);

            // Отправим данные
            checker.AddLogSizePerDay(storage, 100);

            // Проверим, что использованный лимит за сегодня заполнен
            var usedLimits = checker.GetUsedTodayTariffLimit(storage);
            Assert.Equal(100, usedLimits.LogSize);

            // Установим время на начало следующего дня (00:01)
            now = now.AddMinutes(2);
            checker.NowOverride = now;

            // Проверим, что использованный лимит за сегодня пустой, так как начался новый день
            usedLimits = checker.GetUsedTodayTariffLimit(storage);
            Assert.Equal(0, usedLimits.LogSize);
        }

        [Fact]
        public void LimitDatasPer1DayLoadTest()
        {
            var account = TestHelper.GetTestAccount();
            var storage = TestHelper.GetStorage();

            // Подготовим данные за вчера и сохраним их

            var today = DateTime.Now.Date;
            var now = today.AddMinutes(1);
            var checker = new AccountLimitsChecker(now);

            checker.AddLogSizePerDay(storage, 101);
            checker.AddEventsSizePerDay(storage, 102);
            checker.AddUnitTestsSizePerDay(storage, 103);
            checker.AddMetricsSizePerDay(storage, 104);

            var yesterday = today;
            now = today.AddDays(1);
            checker.NowOverride = now;

            checker.AddLogSizePerDay(storage, 10);
            checker.AddEventsSizePerDay(storage, 20);
            checker.AddUnitTestsSizePerDay(storage, 30);
            checker.AddMetricsSizePerDay(storage, 40);

            now = now.AddMinutes(AccountLimitsChecker.LimitDataTimeStep);
            checker.NowOverride = now;

            checker.SaveData(storage);

            // Перечитаем данные из базы
            checker = new AccountLimitsChecker(now);

            // Получим использованные лимиты
            AccountUsedLimitsPerDayDataInfo usedOverallLimits;
            AccountUsedLimitsArchiveDataInfo[] usedArchiveLimits;

            var limits = checker.GetUsedOverallTariffLimit(storage, 30);
            usedOverallLimits = limits.Total;
            usedArchiveLimits = limits.Archive;

            // Проверим, что данные не потерялись
            Assert.Equal(111, usedOverallLimits.LogSize);
            Assert.Equal(122, usedOverallLimits.EventsSize);
            Assert.Equal(133, usedOverallLimits.UnitTestsSize);
            Assert.Equal(144, usedOverallLimits.MetricsSize);

            var yesterdayUsedLimits = usedArchiveLimits.FirstOrDefault(t => t.Date == yesterday);
            Assert.NotNull(yesterdayUsedLimits);
            Assert.Equal(101, yesterdayUsedLimits.Info.LogSize);
            Assert.Equal(102, yesterdayUsedLimits.Info.EventsSize);
            Assert.Equal(103, yesterdayUsedLimits.Info.UnitTestsSize);
            Assert.Equal(104, yesterdayUsedLimits.Info.MetricsSize);
        }

        [Fact]
        public void LimitDatasPer1DaySaveTest()
        {
            var account = TestHelper.GetTestAccount();
            var storage = TestHelper.GetStorage();

            // Установим время на начало дня
            var today = DateTime.Now.Date;
            var now = today.AddMinutes(1);
            var checker = new AccountLimitsChecker(now);

            // Отправим данные
            checker.AddLogSizePerDay(storage, 100);

            // Установим время на конец дня
            now = today.AddDays(1).AddMinutes(-1);
            checker.NowOverride = now;

            // Отправим данные
            checker.AddLogSizePerDay(storage, 200);

            // Вызовем сохранение
            checker.SaveData(storage);

            // Проверим, что записи за вчера пока нет
            using (var accountContext = TestHelper.GetDbContext())
            {
                var data1Day = accountContext.LimitDatas.FirstOrDefault(t => t.Type == LimitDataType.Per1Day && t.BeginDate == today);
                Assert.Null(data1Day);
            }

            // Начался новый день
            now = today.AddDays(1);
            checker.NowOverride = now;

            // Отправим данные
            checker.AddLogSizePerDay(storage, 50);

            // Вызовем сохранение
            checker.SaveData(storage);

            // Проверим, что запись за вчера появилась
            using (var accountContext = TestHelper.GetDbContext())
            {
                var data1Day = accountContext.LimitDatas.FirstOrDefault(t => t.Type == LimitDataType.Per1Day && t.BeginDate == today);
                Assert.NotNull(data1Day);
                Assert.Equal(300, data1Day.LogSize);
            }
        }

        [Fact]
        public void GetAccountLimitsTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = DispatcherHelper.GetDispatcherService();

            // Запомним начальные лимиты
            var response = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.Token,
                Data = new GetAccountLimitsRequestData()
            });
            Assert.True(response.Success);
            var baseAccountLimits = response.GetDataAndCheck();

            // Выполним по одному действию каждого типа
            var componentTypeResponse = dispatcher.GetOrCreateComponentType(new GetOrCreateComponentTypeRequestDto()
            {
                Token = account.Token,
                Data = new GetOrCreateComponentTypeRequestDataDto()
                {
                    SystemName = Guid.NewGuid().ToString()
                }
            });
            Assert.True(componentTypeResponse.Success);

            var componentResponse = dispatcher.GetOrCreateComponent(new GetOrCreateComponentRequestDto()
            {
                Token = account.Token,
                Data = new GetOrCreateComponentRequestDataDto()
                {
                    TypeId = componentTypeResponse.GetDataAndCheck().Id,
                    SystemName = Guid.NewGuid().ToString()
                }
            });
            Assert.True(componentResponse.Success);

            var logRequest = new SendLogRequestDto()
            {
                Token = account.Token,
                Data = new SendLogRequestDataDto()
                {
                    ComponentId = componentResponse.GetDataAndCheck().Component.Id,
                    Message = "Test",
                    Properties = new List<ExtentionPropertyDto>()
                }
            };
            logRequest.Data.Properties.AddValue("Property", "Value");
            var logResponse = dispatcher.SendLog(logRequest);
            Assert.True(logResponse.Success);

            var eventRequest = new SendEventRequestDto()
            {
                Token = account.Token,
                Data = new SendEventRequestDataDto()
                {
                    ComponentId = componentResponse.GetDataAndCheck().Component.Id,
                    Category = SendEventCategory.ApplicationError,
                    Importance = EventImportance.Alarm,
                    TypeSystemName = Guid.NewGuid().ToString(),
                    Properties = new List<ExtentionPropertyDto>()
                }
            };
            eventRequest.Data.Properties.AddValue("Property", "Value");
            var eventResponse = dispatcher.SendEvent(eventRequest);
            Assert.True(eventResponse.Success);

            var unitTypeResponse = dispatcher.GetOrCreateUnitTestType(new GetOrCreateUnitTestTypeRequestDto()
            {
                Token = account.Token,
                Data = new GetOrCreateUnitTestTypeRequestDataDto()
                {
                    SystemName = Guid.NewGuid().ToString()
                }
            });
            Assert.True(unitTypeResponse.Success);

            var apiCheckResponse = dispatcher.GetOrCreateUnitTest(new GetOrCreateUnitTestRequestDto()
            {
                Token = account.Token,
                Data = new GetOrCreateUnitTestRequestDataDto()
                {
                    ComponentId = componentResponse.GetDataAndCheck().Component.Id,
                    SystemName = Guid.NewGuid().ToString(),
                    UnitTestTypeId = unitTypeResponse.GetDataAndCheck().Id
                }
            });
            Assert.True(apiCheckResponse.Success);

            var unitTestResultRequest = new SendUnitTestResultRequestDto()
            {
                Token = account.Token,
                Data = new SendUnitTestResultRequestDataDto()
                {
                    UnitTestId = apiCheckResponse.GetDataAndCheck().Id,
                    Result = UnitTestResult.Success
                }
            };

            var apiCheckResultResponse = dispatcher.SendUnitTestResult(unitTestResultRequest);
            Assert.True(apiCheckResultResponse.Success);

            var countersRequest = new SendMetricRequestDto()
            {
                Token = account.Token,
                Data = new SendMetricRequestDataDto()
                {
                    ComponentId = componentResponse.GetDataAndCheck().Component.Id,
                    Name = Guid.NewGuid().ToString(),
                    Value = new Random().NextDouble()
                }
            };
            var countersResponse = dispatcher.SendMetric(countersRequest);
            Assert.True(countersResponse.Success);

            var sendSmsRequest = new SendSmsRequest()
            {
                Token = account.Token,
                Data = new SendSmsRequestData()
                {
                    Phone = "-",
                    Body = "-"
                }
            };
            var sendSmsResponse = dispatcher.SendSms(sendSmsRequest);
            Assert.True(sendSmsResponse.Success);

            // Заново получим лимиты аккаунта
            response = dispatcher.GetAccountLimits(new GetAccountLimitsRequest()
            {
                Token = account.Token,
                Data = new GetAccountLimitsRequestData()
            });
            Assert.True(response.Success);
            var accountLimits = response.GetDataAndCheck();

            // Теперь все использованные лимиты должны быть заполнены
            Assert.Equal(baseAccountLimits.UsedToday.EventRequests + 1, accountLimits.UsedToday.EventRequests);
            Assert.Equal(baseAccountLimits.UsedToday.EventsSize + eventRequest.Data.GetSize(), accountLimits.UsedToday.EventsSize);
            Assert.Equal(baseAccountLimits.UsedToday.LogSize + logRequest.Data.GetSize(), accountLimits.UsedToday.LogSize);
            Assert.Equal(baseAccountLimits.UsedToday.MetricsSize + countersRequest.Data.GetSize(), accountLimits.UsedToday.MetricsSize);
            Assert.Equal(baseAccountLimits.UsedToday.UnitTestsRequests + 1, accountLimits.UsedToday.UnitTestsRequests);
            Assert.Equal(baseAccountLimits.UsedToday.UnitTestsSize + unitTestResultRequest.Data.GetSize(), accountLimits.UsedToday.UnitTestsSize);
            Assert.Contains(accountLimits.UsedToday.UnitTestsResults, t => t.UnitTestId == apiCheckResponse.GetDataAndCheck().Id && t.ApiChecksResults == 1);
            Assert.Equal(baseAccountLimits.UsedToday.SmsCount + 1, accountLimits.UsedToday.SmsCount);

            var totalSize = eventRequest.Data.GetSize() + logRequest.Data.GetSize() + countersRequest.Data.GetSize() + unitTestResultRequest.Data.GetSize();
            Assert.Equal(baseAccountLimits.UsedToday.StorageSize + totalSize, accountLimits.UsedToday.StorageSize);
        }

        public TariffLimitTests()
        {
            // Перед каждым тестом очистим историю изменения лимитов
            using (var accountDbContext = TestHelper.GetDbContext())
            {
                foreach (var entity in accountDbContext.LimitDatasForUnitTests.ToArray())
                    accountDbContext.Entry(entity).State = EntityState.Deleted;

                foreach (var entity in accountDbContext.LimitDatas.ToArray())
                    accountDbContext.Entry(entity).State = EntityState.Deleted;

                accountDbContext.SaveChanges();
            }
        }

    }
}
