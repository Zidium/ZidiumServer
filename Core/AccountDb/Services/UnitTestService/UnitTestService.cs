using System;
using System.Collections.Generic;
using Zidium.Api.Dto;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common.Helpers;
using Zidium.Core.Limits;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class UnitTestService : IUnitTestService
    {
        public UnitTestService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        private EventForAdd CreateUnitTestResultEvent(
            IUnitTestCacheReadObject unitTest,
            SendUnitTestResultRequestData data,
            DateTime processDate)
        {
            data.Message = data.Message ?? string.Empty;

            if (data.Message.Length > 255)
                data.Message = data.Message.Substring(0, 255);

            var importance = EventImportanceHelper.Get(data.Result ?? UnitTestResult.Unknown);

            var cache = new AccountCache(unitTest.AccountId);
            var unittestType = cache.UnitTestTypes.Read(unitTest.TypeId);

            var actualInterval = unitTest.ActualTime  // сначала берем из настроек проверки в ЛК
                ?? TimeSpanHelper.FromSeconds(unittestType.ActualTimeSecs) // потом из настроек типа проверки в ЛК
                ?? TimeSpanHelper.FromSeconds(data.ActualIntervalSeconds) // потом из сообщения
                ?? UnitTestHelper.GetDefaultActualTime();

            DateTime actualDate;
            try
            {
                actualDate = processDate + actualInterval;
            }
            catch
            {
                actualDate = DateTimeHelper.InfiniteActualDate;
            }

            var joinKeyHash = data.ReasonCode ?? 0;

            var result = new EventForAdd()
            {
                Id = Guid.NewGuid(),
                Count = 1,
                JoinKeyHash = joinKeyHash,
                Importance = importance,
                OwnerId = unitTest.Id,
                ActualDate = actualDate,
                CreateDate = DateTime.Now,
                LastUpdateDate = processDate,
                StartDate = processDate,
                EndDate = processDate,
                Message = data.Message,
                EventTypeId = SystemEventType.UnitTestResult.Id,
                Category = EventCategory.UnitTestResult
            };
            if (data.Properties != null)
            {
                result.Properties = ApiConverter.GetEventProperties(data.Properties);
                foreach (var property in result.Properties)
                {
                    property.EventId = result.Id;
                }
            }
            return result;
        }

        private Guid Create(
            Guid accountId,
            Guid componentId,
            Guid unitTestTypeId,
            string systemName,
            string displayName,
            Guid? newId)
        {
            var now = DateTime.Now;
            var component = _storage.Components.GetOneById(componentId);

            var unitTestId = newId ?? Guid.NewGuid();

            var bulbService = new BulbService(_storage);
            var statusDataId = bulbService.CreateBulb(
                accountId,
                now,
                EventCategory.UnitTestStatus,
                unitTestId,
                "Проверка создана, ждем результата выполнения");

            using (var transaction = _storage.BeginTransaction())
            {
                var unitTest = new UnitTestForAdd()
                {
                    Id = unitTestId,
                    SystemName = systemName,
                    DisplayName = displayName,
                    ComponentId = componentId,
                    Enable = true,
                    ParentEnable = component.ParentEnable,
                    TypeId = unitTestTypeId,
                    CreateDate = now,
                    StatusDataId = statusDataId
                };

                // чтобы не получилось так, что период равен нулю и начнется непрерывное выполнение проверки
                if (SystemUnitTestType.IsSystem(unitTestTypeId))
                {
                    if (unitTestTypeId == SystemUnitTestType.DomainNameTestType.Id)
                    {
                        // для доменной проверки период задается системой, пользователь НЕ может его менять сам
                        unitTest.PeriodSeconds = (int)TimeSpan.FromDays(1).TotalSeconds;
                    }
                    else
                    {
                        unitTest.PeriodSeconds = (int)TimeSpan.FromMinutes(10).TotalSeconds;
                    }
                }

                _storage.UnitTests.Add(unitTest);

                if (unitTestTypeId == SystemUnitTestType.HttpUnitTestType.Id)
                {
                    var httpRequestUnitTest = new HttpRequestUnitTestForAdd()
                    {
                        UnitTestId = unitTest.Id
                    };
                    _storage.HttpRequestUnitTests.Add(httpRequestUnitTest);
                }

                var bulbForUpdate = new BulbForUpdate(statusDataId);
                bulbForUpdate.UnitTestId.Set(unitTest.Id);
                _storage.Bulbs.Update(bulbForUpdate);

                transaction.Commit();
            }

            return unitTestId;
        }

        public IUnitTestCacheReadObject GetOrCreateUnitTest(Guid accountId, GetOrCreateUnitTestRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (string.IsNullOrEmpty(data.SystemName))
            {
                throw new ParameterRequiredException("SystemName");
            }
            if (data.ComponentId == null)
            {
                throw new ParameterRequiredException("ComponentId");
            }

            if (!data.UnitTestTypeId.HasValue)
            {
                var unittestTypeService = new UnitTestTypeService(_storage);
                var unittestType = unittestTypeService.GetOrCreateUnitTestType(accountId, new GetOrCreateUnitTestTypeRequestData()
                {
                    SystemName = "CustomUnitTestType",
                    DisplayName = "Пользовательская проверка"
                });
                data.UnitTestTypeId = unittestType.Id;
            }

            var cache = new AccountCache(accountId);
            var componentId = data.ComponentId.Value;
            var systemName = data.SystemName;

            // проверим, что тип проверки существует
            var unitTestTypeId = data.UnitTestTypeId.Value;
            if (!SystemUnitTestType.IsSystem(unitTestTypeId))
            {
                var unitTestType = AllCaches.UnitTestTypes.Find(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = unitTestTypeId
                });

                if (unitTestType == null)
                    throw new UnknownUnitTestTypeIdException(unitTestTypeId);
            }

            // получим компонент
            var component = cache.Components.Read(componentId);
            if (component == null)
            {
                throw new UnknownComponentIdException(componentId, accountId);
            }

            // проверим есть ли у него ссылка на проверку
            var unitTestRef = component.UnitTests.FindByName(data.SystemName);
            if (unitTestRef != null)
            {
                // ссылка есть, вернем существующую проверку
                var unitTest = cache.UnitTests.Read(unitTestRef.Id);
                if (unitTest == null)
                {
                    throw new Exception("unitTest == null");
                }
                return unitTest;
            }

            // ссылки нет
            using (var writeComponent = cache.Components.Write(componentId))
            {
                // проверим ссылку еще раз
                unitTestRef = writeComponent.UnitTests.FindByName(data.SystemName);
                if (unitTestRef != null)
                {
                    var unitTest = cache.UnitTests.Read(unitTestRef.Id);
                    if (unitTest == null)
                    {
                        throw new Exception("unitTest == null");
                    }
                    return unitTest;
                }

                // Проверим лимиты
                var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
                var checkResult = checker.CheckMaxUnitTestsCount(_storage);

                if (!checkResult.Success)
                    throw new OverLimitException(checkResult.Message);

                // проверка лимитов пройдена
                if (string.IsNullOrWhiteSpace(data.DisplayName))
                {
                    data.DisplayName = data.SystemName;
                }

                // создаем проверку
                var unitTestId = Create(accountId, componentId, unitTestTypeId, systemName, data.DisplayName, data.NewId);
                unitTestRef = new CacheObjectReference(unitTestId, systemName);
                writeComponent.WriteUnitTests.Add(unitTestRef);
                writeComponent.BeginSave();

                checker.RefreshApiChecksCount();

                // обновим свойства
                using (var unitTestCache = cache.UnitTests.Write(unitTestId))
                {
                    unitTestCache.NoSignalColor = data.NoSignalColor;
                    unitTestCache.ErrorColor = data.ErrorColor;
                    unitTestCache.ActualTime = TimeSpanHelper.FromSeconds(data.ActualTimeSecs);
                    unitTestCache.AttempMax = data.AttempMax ?? 1;
                    if (data.PeriodSeconds.HasValue)
                    {
                        unitTestCache.PeriodSeconds = data.PeriodSeconds;
                    }
                    if (data.SimpleMode.HasValue)
                    {
                        unitTestCache.SimpleMode = data.SimpleMode.Value;
                    }
                    unitTestCache.BeginSave();
                    unitTestCache.WaitSaveChanges();
                    return unitTestCache;
                }
            }
        }

        public void SetUnitTestNextTime(Guid accountId, SetUnitTestNextTimeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.UnitTestId == null)
            {
                throw new ParameterRequiredException("UnitTestId");
            }
            var cache = new AccountCache(accountId);
            using (var unitTest = cache.UnitTests.Write(data.UnitTestId.Value))
            {
                if (data.NextTime == null)
                {
                    unitTest.NextExecutionDate = DateTime.Now;
                }
                else
                {
                    unitTest.NextExecutionDate = data.NextTime.Value;
                }
                unitTest.BeginSave();
            }
        }

        public void SetUnitTestNextStepProcessTime(Guid accountId, SetUnitTestNextStepProcessTimeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.UnitTestId == null)
            {
                throw new ParameterRequiredException("UnitTestId");
            }
            if (data.NextStepProcessTime == null)
            {
                throw new ParameterRequiredException("NextStepProcessTime");
            }
            var cache = new AccountCache(accountId);
            using (var unitTest = cache.UnitTests.Write(data.UnitTestId.Value))
            {
                unitTest.NextExecutionDate = data.NextStepProcessTime;
                unitTest.AttempCount = unitTest.AttempCount + 1;
                unitTest.BeginSave();
            }
        }

        public void UpdateUnitTest(Guid accountId, UpdateUnitTestRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.UnitTestId == null)
            {
                throw new ParameterRequiredException("UnitTestId");
            }
            if (data.UnitTestId == Guid.Empty)
            {
                throw new ParameterErrorException("UnitTestId не может быть Guid.Empty");
            }
            if (data.DisplayName != null && string.IsNullOrWhiteSpace(data.DisplayName))
            {
                throw new ParameterErrorException("DisplayName не может быть пустым");
            }

            if (data.ComponentId.HasValue)
            {
                //получим компонент, чтобы убедиться, что он принадлежит аккаунту
                var req = new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = data.ComponentId.Value
                };
                var component = AllCaches.Components.Find(req);
                if (component == null)
                {
                    throw new UnknownComponentIdException(data.ComponentId.Value, accountId);
                }
            }

            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = data.UnitTestId.Value
            };

            // сохраним изменения
            bool noSignalColorChanged;
            IUnitTestCacheReadObject unitTestCache;
            using (var unitTest = AllCaches.UnitTests.Write(request))
            {
                unitTestCache = unitTest;

                // период выполнения проверки
                if (unitTest.IsSystemType)
                {
                    // не для всех системных проверок можно указывать период (для проверки домена нельзя)
                    if (SystemUnitTestType.CanEditPeriod(unitTest.TypeId))
                    {
                        if (data.PeriodSeconds.HasValue && data.PeriodSeconds.Value < 60)
                        {
                            throw new ParameterErrorException("Период проверки НЕ может быть меньше 1 минуты");
                        }
                        unitTest.PeriodSeconds = (int?)data.PeriodSeconds ?? unitTest.PeriodSeconds;
                    }

                    // чтобы выполнить проверку прямо сейчас с новыми параметрами и увидеть результат
                    unitTest.NextExecutionDate = DateTime.Now;
                }

                noSignalColorChanged = data.NoSignalColor != unitTest.NoSignalColor;

                unitTest.DisplayName = data.DisplayName ?? unitTest.DisplayName;
                unitTest.ComponentId = data.ComponentId ?? unitTest.ComponentId;
                unitTest.ErrorColor = data.ErrorColor ?? unitTest.ErrorColor;
                unitTest.NoSignalColor = data.NoSignalColor ?? unitTest.NoSignalColor;
                unitTest.ActualTime = data.ActualTime.HasValue ? TimeSpanHelper.FromSeconds(data.ActualTime) : null;
                unitTest.SimpleMode = data.SimpleMode ?? unitTest.SimpleMode;
                unitTest.AttempMax = data.AttempMax ?? unitTest.AttempMax;

                unitTest.BeginSave();
            }

            // ждем сохранения в кэше
            unitTestCache.WaitSaveChanges();

            // при изменении "Цвет если нет сигнала" нужно обновить цвет проверки
            if (noSignalColorChanged)
            {
                UpdateNoSignalColor(unitTestCache);
            }
        }

        public void UpdateNoSignalColor(IUnitTestCacheReadObject unitTest)
        {
            var statusService = new BulbService(_storage);
            var statusData = statusService.GetRaw(unitTest.AccountId, unitTest.StatusDataId);
            if (!statusData.HasSignal)
            {
                var now = DateTime.Now;
                var noSignalEvent = GetNoSignalEvent(unitTest, now, now);
                SaveResultEvent(now, unitTest, noSignalEvent, null);
            }
        }

        public void Delete(Guid accountId, Guid unitTestId)
        {
            // удаляем юнит-тест
            var unitTestRequest = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = unitTestId
            };
            Guid componentId;
            Guid componentUnitTestsStatudDataId;
            Guid unitTestStatusDataId;
            using (var unitTest = AllCaches.UnitTests.Write(unitTestRequest))
            {
                componentId = unitTest.ComponentId;
                unitTestStatusDataId = unitTest.StatusDataId;
                unitTest.IsDeleted = true;
                unitTest.BeginSave();
            }

            // удаляем колбасу юнит-теста
            var statusDataRequest = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = unitTestStatusDataId
            };
            using (var statusData = AllCaches.StatusDatas.Write(statusDataRequest))
            {
                statusData.IsDeleted = true;
                statusData.BeginSave();
            }

            // удаляем ссылку на юнит-тест в компоненте
            var componentRequest = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = componentId
            };
            using (var component = AllCaches.Components.Write(componentRequest))
            {
                componentUnitTestsStatudDataId = component.UnitTestsStatusId;
                component.WriteUnitTests.Delete(unitTestId);
                component.BeginSave();
            }

            // обновляем статус колбаски тестов
            var allUnitTestsStatusDataIds = new List<Guid>();
            var component1 = AllCaches.Components.Find(new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = componentId
            });
            foreach (var unitTestRef in component1.UnitTests.GetAll())
            {
                var unitTest = AllCaches.UnitTests.Find(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = unitTestRef.Id
                });
                if (unitTest != null && unitTest.IsDeleted == false)
                {
                    allUnitTestsStatusDataIds.Add(unitTest.StatusDataId);
                }
            }
            var statuservice = new BulbService(_storage);
            statuservice.CalculateByChilds(accountId, componentUnitTestsStatudDataId, allUnitTestsStatusDataIds.ToArray());

            var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            checker.RefreshApiChecksCount();
        }

        public Guid AddPingUnitTest(Guid accountId, AddPingUnitTestRequestData data)
        {
            var unitTestCache = GetOrCreateUnitTest(accountId, new GetOrCreateUnitTestRequestData()
            {
                UnitTestTypeId = SystemUnitTestType.PingTestType.Id,
                ComponentId = data.ComponentId,
                SystemName = data.SystemName,
                DisplayName = data.DisplayName,
                AttempMax = data.AttempMax
            });

            using (var unitTest = AllCaches.UnitTests.Write(unitTestCache))
            {
                unitTest.PeriodSeconds = data.PeriodSeconds;
                unitTest.ErrorColor = data.ErrorColor;
                unitTest.NextExecutionDate = DateTime.Now;
                unitTest.BeginSave();
                unitTest.WaitSaveChanges(TimeSpan.FromSeconds(30));
            }

            using (var transaction = _storage.BeginTransaction())
            {
                var unitTestPingRule = _storage.UnitTestPingRules.GetOneOrNullByUnitTestId(unitTestCache.Id);
                if (unitTestPingRule == null)
                {
                    var unitTestPingRuleForAdd = new UnitTestPingRuleForAdd()
                    {
                        UnitTestId = unitTestCache.Id
                    };
                    _storage.UnitTestPingRules.Add(unitTestPingRuleForAdd);
                }

                var unitTestPingRuleForUpdate = new UnitTestPingRuleForUpdate(unitTestCache.Id);
                unitTestPingRuleForUpdate.Host.Set(data.Host);
                unitTestPingRuleForUpdate.TimeoutMs.Set(data.TimeoutMs);
                _storage.UnitTestPingRules.Update(unitTestPingRuleForUpdate);

                transaction.Commit();
            }

            return unitTestCache.Id;
        }

        public Guid AddHttpUnitTest(Guid accountId, AddHttpUnitTestRequestData data)
        {
            var unitTestCache = GetOrCreateUnitTest(accountId, new GetOrCreateUnitTestRequestData()
            {
                UnitTestTypeId = SystemUnitTestType.HttpUnitTestType.Id,
                ComponentId = data.ComponentId,
                SystemName = data.SystemName,
                DisplayName = data.DisplayName,
                AttempMax = data.AttempMax
            });

            Guid id;
            using (var unitTest = AllCaches.UnitTests.Write(unitTestCache))
            {
                unitTest.PeriodSeconds = data.PeriodSeconds;
                unitTest.ErrorColor = data.ErrorColor;
                unitTest.NextExecutionDate = DateTime.Now;
                unitTest.BeginSave();
                unitTest.WaitSaveChanges(TimeSpan.FromSeconds(30));
                id = unitTest.Id;
            }

            using (var transaction = _storage.BeginTransaction())
            {
                foreach (var ruleData in data.Rules)
                {
                    var rule = new HttpRequestUnitTestRuleForAdd()
                    {
                        Id = Guid.NewGuid(),
                        HttpRequestUnitTestId = id,
                        SortNumber = ruleData.SortNumber,
                        DisplayName = ruleData.DisplayName,
                        Url = ruleData.Url,
                        Method = ruleData.Method,
                        ResponseCode = ruleData.ResponseCode,
                        MaxResponseSize = ruleData.MaxResponseSize,
                        SuccessHtml = ruleData.SuccessHtml,
                        ErrorHtml = ruleData.ErrorHtml,
                        TimeoutSeconds = ruleData.TimeoutSeconds
                    };
                    _storage.HttpRequestUnitTestRules.Add(rule);
                }

                transaction.Commit();
            }

            return id;
        }

        private IBulbCacheReadObject SaveResultEvent(
            DateTime processDate,
            IUnitTestCacheReadObject unitTest,
            EventForAdd newEvent,
            DateTime? nextExecutionTime,
            int? attempCount = null)
        {
            var request = new AccountCacheRequest()
            {
                AccountId = unitTest.AccountId,
                ObjectId = unitTest.StatusDataId
            };
            using (var statusData = AllCaches.StatusDatas.Write(request))
            {
                // сохраним результаты
                var eventService = new EventService(_storage);
                IEventCacheReadObject lastEvent = null;
                if (statusData.LastEventId.HasValue)
                {
                    lastEvent = eventService.GetEventCacheOrNullById(
                        unitTest.AccountId,
                        statusData.LastEventId.Value);
                }

                // для системных проверок пробелы должны быть серыми, 
                // чтобы из-за простоев агента зидиума не красить пользовательские проверки
                var noSignalImportance = unitTest.IsSystemType
                    ? EventImportance.Unknown
                    : EventImportance.Alarm;

                if (lastEvent == null)
                {
                    // todo не понятно нужно ли в результаты проверок вставлять пробелы?
                    // TryAddSpaceResultEvent(unitTest, newEvent, noSignalImportance);
                }
                else
                {
                    // например, чтобы обрезать бесконечную актуальность у события "нет сигнала"
                    if (lastEvent.ActualDate > newEvent.StartDate)
                    {
                        if (AllCaches.Events.ExistsInStorage(lastEvent))
                        {
                            using (var wLastEvent = AllCaches.Events.Write(lastEvent))
                            {
                                wLastEvent.ActualDate = newEvent.StartDate;
                                wLastEvent.BeginSave();
                            }
                        }
                    }
                }

                // синхронная вставка
                // асинхронное создание объектов сильно усложняет код, поэтому будет создавать всё синхронно
                eventService.Add(unitTest.AccountId, newEvent);

                using (var wUnitTest = AllCaches.UnitTests.Write(unitTest))
                {
                    wUnitTest.LastExecutionDate = newEvent.EndDate;
                    wUnitTest.AttempCount = attempCount ?? wUnitTest.AttempCount;

                    // установим время следующего выполнения
                    if (nextExecutionTime.HasValue)
                    {
                        // время задано явно
                        wUnitTest.NextExecutionDate = nextExecutionTime.Value;
                    }
                    else if (wUnitTest.PeriodSeconds > 0)
                    {
                        // время явно не задано, нужно рассчитать
                        var nextTime = wUnitTest.NextExecutionDate ?? processDate;
                        var period = TimeSpan.FromSeconds(wUnitTest.PeriodSeconds.Value);
                        while (nextTime <= processDate)
                        {
                            nextTime = nextTime + period;
                        }
                        wUnitTest.NextExecutionDate = nextTime;
                    }
                    wUnitTest.BeginSave();
                }

                // обновим статус
                var statusService = new BulbService(_storage);
                var newEventForRead = _storage.Events.GetOneById(newEvent.Id);
                var signal = BulbSignal.Create(processDate, newEventForRead, noSignalImportance, unitTest.AccountId);
                var newStatus = statusService.SetSignal(unitTest.StatusDataId, signal);

                return newStatus;
            }
        }

        public IBulbCacheReadObject SendUnitTestResult(Guid accountId, SendUnitTestResultRequestData data)
        {
            // Проверка на наличие необходимых параметров
            if (data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (data.UnitTestId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestId");
            }
            if (data.ActualIntervalSeconds.HasValue && data.ActualIntervalSeconds < 0)
            {
                throw new ParameterErrorException("ActualIntervalSeconds must be >= 0");
            }

            using (var unitTest = AllCaches.UnitTests.Write(new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = data.UnitTestId.Value
            }))
            {
                var processDate = DateTime.Now;

                // получим актуальную колбасу, чтобы не было пробелов между результатами
                GetUnitTestResultInternal(unitTest, processDate);

                // если проверка выключена или выключен один из её родителей
                if (unitTest.CanProcess == false)
                {
                    throw new ResponseCodeException(Zidium.Api.ResponseCode.ObjectDisabled, "Проверка выключена");
                }

                var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
                var size = data.GetSize();

                try
                {
                    // Проверим лимит "Количество запросов в день"
                    checker.CheckUnitTestResultsPerDay(_storage);

                    var canIncreaseSizeInStatictics = true;
                    try
                    {
                        // Проверим лимит размера хранилища
                        checker.CheckStorageSize(_storage, size, out canIncreaseSizeInStatictics);

                        var resultEvent = CreateUnitTestResultEvent(unitTest, data, processDate);
                        if (resultEvent.ActualDate < processDate)
                        //todo это источник проблем из-за разного времени клиента и сервера
                        {
                            // код ниже никогда не выполнится, но пусть на всякий случай будет
                            throw new ResponseCodeException(
                                Zidium.Api.ResponseCode.ServerError,
                                "Результат юнит-теста неактуален (date + actualInterval < now)");
                        }

                        // сохраним результаты
                        var result = SaveResultEvent(processDate, unitTest, resultEvent, data.NextExecutionTime, data.AttempCount);

                        return result;
                    }
                    finally
                    {
                        if (canIncreaseSizeInStatictics)
                            checker.AddUnitTestsSizePerDay(_storage, size);
                    }
                }
                finally
                {
                    checker.AddUnitTestResultsPerDay(_storage, unitTest.Id);
                }
            }
        }

        public void SendUnitTestResults(Guid accountId, SendUnitTestResultRequestData[] data)
        {
            foreach (var item in data)
            {
                SendUnitTestResult(accountId, item);
            }
        }

        protected IBulbCacheReadObject GetUnitTestResultInternal(
            IUnitTestCacheReadObject unitTest,
            DateTime processDate)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            // обновим галочку "включено"
            if (unitTest.Enable == false
                && unitTest.DisableToDate.HasValue
                && unitTest.DisableToDate < DateTime.Now)
            {
                using (var unitTestWrite = AllCaches.UnitTests.Write(unitTest))
                {
                    unitTestWrite.Enable = true;
                    unitTestWrite.BeginSave();
                    unitTest = unitTestWrite;
                }
            }

            var statusService = new BulbService(_storage);
            var data = statusService.GetRaw(unitTest.AccountId, unitTest.StatusDataId);

            // если надо выключить
            if (unitTest.CanProcess == false && data.Status != MonitoringStatus.Disabled)
            {
                return UpdateEnableOrDisableStatusData(unitTest);
            }

            // если надо включить
            if (unitTest.CanProcess && data.Status == MonitoringStatus.Disabled)
            {
                return UpdateEnableOrDisableStatusData(unitTest);
            }

            // проверим актуальность
            if (data.Actual(processDate))
            {
                return data;
            }

            // значение неактуальное
            // сохраняем пробел (событие результата теста)
            var noSignalEvent = GetNoSignalEvent(unitTest, processDate, data.ActualDate);

            return SaveResultEvent(processDate, unitTest, noSignalEvent, null);
        }

        private EventForAdd GetNoSignalEvent(IUnitTestCacheReadObject unitTest, DateTime processDate, DateTime startDate)
        {
            var cache = new AccountCache(unitTest.AccountId);
            var unittestType = cache.UnitTestTypes.Read(unitTest.TypeId);

            var noSignalImportance = unitTest.IsSystemType
                ? EventImportance.Unknown
                : ImportanceHelper.Get(unitTest.NoSignalColor) ??
                  ImportanceHelper.Get(unittestType.NoSignalColor) ??
                  EventImportance.Alarm;

            return new EventForAdd()
            {
                Id = Guid.NewGuid(),
                Message = "Нет сигнала",
                OwnerId = unitTest.Id,
                ActualDate = DateTimeHelper.InfiniteActualDate,
                Category = EventCategory.UnitTestResult,
                Count = 1,
                CreateDate = processDate,
                LastUpdateDate = processDate,
                StartDate = startDate,
                EndDate = processDate,
                IsSpace = true,
                EventTypeId = SystemEventType.UnitTestResult.Id,
                Importance = noSignalImportance
            };
        }

        public IBulbCacheReadObject GetUnitTestResult(Guid accountId, Guid unitTestId)
        {
            var cache = new AccountCache(accountId);
            var unitTest = cache.UnitTests.Read(unitTestId);
            var processDate = DateTime.Now;
            return GetUnitTestResultInternal(unitTest, processDate);
        }

        protected static object BannerLock = new object();

        public bool SendHttpUnitTestBanner(Guid accountId, Guid unitTestId, bool hasBanner)
        {
            lock (BannerLock)
            {
                var limitChecker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);

                var httpRequestUnitTest = _storage.HttpRequestUnitTests.GetOneByUnitTestId(unitTestId);
                var httpRequestUnitTestForUpdate = httpRequestUnitTest.GetForUpdate();
                var hasBannerChanged = (httpRequestUnitTest.LastBannerCheck == null) || (httpRequestUnitTest.HasBanner != hasBanner);
                if (hasBannerChanged)
                {
                    httpRequestUnitTestForUpdate.HasBanner.Set(hasBanner);
                }

                // В любом случае баннер проверяется не чаще 1 раза в день
                httpRequestUnitTestForUpdate.LastBannerCheck.Set(DateTime.Now);
                _storage.HttpRequestUnitTests.Update(httpRequestUnitTestForUpdate);

                if (hasBannerChanged)
                {
                    limitChecker.RefreshHttpChecksNoBannerCount();
                }

                // Если баннера нет, то надо проверить лимит
                if (!hasBanner)
                {
                    var checkResult = limitChecker.CheckMaxHttpChecksNoBanner(_storage);
                    if (!checkResult.Success)
                    {
                        var disableData = new SetUnitTestDisableRequestData()
                        {
                            UnitTestId = unitTestId,
                            Comment = "Проверка выключена из-за отсутствия бесплатного баннера",
                            ToDate = null
                        };
                        Disable(accountId, disableData);

                        limitChecker.RefreshHttpChecksNoBannerCount();
                    }
                    return checkResult.Success;
                }

                return true;
            }
        }

        private IBulbCacheReadObject UpdateEnableOrDisableStatusData(IUnitTestCacheReadObject unitTest)
        {
            var statusService = new BulbService(_storage);
            IBulbCacheReadObject data = null;
            var processDate = DateTime.Now;
            if (unitTest.CanProcess)
            {
                var unknownSignal = BulbSignal.CreateUnknown(unitTest.AccountId, processDate);
                data = statusService.SetSignal(unitTest.StatusDataId, unknownSignal);
            }
            else
            {
                var disableSignal = BulbSignal.CreateDisable(unitTest.AccountId, processDate);
                data = statusService.SetSignal(unitTest.StatusDataId, disableSignal);
            }

            var componentService = new ComponentService(_storage);
            componentService.CalculateAllStatuses(unitTest.AccountId, unitTest.ComponentId);

            return data;
        }

        public IBulbCacheReadObject Disable(Guid accountId, SetUnitTestDisableRequestData requestData)
        {
            var unitTestId = requestData.UnitTestId;
            var cache = new AccountCache(accountId);
            IUnitTestCacheReadObject unitTestRead = null;

            // изменим юнит-тест
            using (var unitTest = cache.UnitTests.Write(unitTestId.Value))
            {
                unitTestRead = unitTest;
                unitTest.Enable = false;
                unitTest.DisableToDate = requestData.ToDate;
                unitTest.DisableComment = requestData.Comment;
                unitTest.BeginSave();
            }

            // обновим колбаски
            return UpdateEnableOrDisableStatusData(unitTestRead);
        }

        public IBulbCacheReadObject Enable(Guid accountId, Guid unitTestId)
        {
            var cache = new AccountCache(accountId);
            IUnitTestCacheReadObject unitTestRead = null;

            // изменим юнит-тест
            using (var unitTest = cache.UnitTests.Write(unitTestId))
            {
                unitTestRead = unitTest;
                unitTest.Enable = true;
                unitTest.BeginSave();
            }

            // обновим колбаски
            var statusService = new BulbService(_storage);
            var processDate = DateTime.Now;
            var unknownSignal = BulbSignal.CreateUnknown(accountId, processDate);
            var data = statusService.SetSignal(unitTestRead.StatusDataId, unknownSignal);

            var componentService = new ComponentService(_storage);
            componentService.CalculateAllStatuses(accountId, unitTestRead.ComponentId);

            return data;
        }

        public int RecalcUnitTestsResults(Guid accountId, int maxCount)
        {
            var unitTestsIds = _storage.UnitTests.GetNotActualIds(DateTime.Now, maxCount);
            foreach (var unitTestId in unitTestsIds)
            {
                GetUnitTestResult(accountId, unitTestId);
            }
            return unitTestsIds.Length;
        }

        public string GetFullDisplayName(UnitTestForRead unittest)
        {
            var component = _storage.Components.GetOneById(unittest.ComponentId);
            var componentService = new ComponentService(_storage);
            return componentService.GetFullDisplayName(component) + " / " + unittest.DisplayName;
        }

    }
}
