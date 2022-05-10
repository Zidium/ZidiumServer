using System;
using System.Collections.Generic;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.Limits;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class UnitTestService : IUnitTestService
    {
        public UnitTestService(IStorage storage, ITimeService timeService)
        {
            _storage = storage;
            _timeService = timeService;
        }

        private readonly IStorage _storage;
        private readonly ITimeService _timeService;

        private EventForAdd CreateUnitTestResultEvent(
            IUnitTestCacheReadObject unitTest,
            SendUnitTestResultRequestDataDto data,
            DateTime processDate)
        {
            data.Message = data.Message ?? string.Empty;

            if (data.Message.Length > 255)
                data.Message = data.Message.Substring(0, 255);

            var importance = EventImportanceHelper.Get(data.Result ?? UnitTestResult.Unknown);

            var cache = new AccountCache();
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
                Id = Ulid.NewUlid(),
                Count = 1,
                JoinKeyHash = joinKeyHash,
                Importance = importance,
                OwnerId = unitTest.Id,
                ActualDate = actualDate,
                CreateDate = _timeService.Now(),
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
            Guid componentId,
            Guid unitTestTypeId,
            string systemName,
            string displayName,
            Guid? newId)
        {
            var now = _timeService.Now();
            var component = _storage.Components.GetOneById(componentId);

            var unitTestId = newId ?? Ulid.NewUlid();

            var bulbService = new BulbService(_storage, _timeService);
            var statusDataId = bulbService.CreateBulb(
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

        public IUnitTestCacheReadObject GetOrCreateUnitTest(GetOrCreateUnitTestRequestDataDto data)
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
                var unittestTypeService = new UnitTestTypeService(_storage, _timeService);
                var unittestType = unittestTypeService.GetOrCreateUnitTestType(new GetOrCreateUnitTestTypeRequestDataDto()
                {
                    SystemName = "CustomUnitTestType",
                    DisplayName = "Пользовательская проверка"
                });
                data.UnitTestTypeId = unittestType.Id;
            }

            var cache = new AccountCache();
            var componentId = data.ComponentId.Value;
            var systemName = data.SystemName;

            // проверим, что тип проверки существует
            var unitTestTypeId = data.UnitTestTypeId.Value;
            if (!SystemUnitTestType.IsSystem(unitTestTypeId))
            {
                var unitTestType = AllCaches.UnitTestTypes.Find(new AccountCacheRequest()
                {
                    ObjectId = unitTestTypeId
                });

                if (unitTestType == null)
                    throw new UnknownUnitTestTypeIdException(unitTestTypeId);
            }

            // получим компонент
            var component = cache.Components.Read(componentId);
            if (component == null)
            {
                throw new UnknownComponentIdException(componentId);
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

                if (string.IsNullOrWhiteSpace(data.DisplayName))
                {
                    data.DisplayName = data.SystemName;
                }

                // создаем проверку
                var unitTestId = Create(componentId, unitTestTypeId, systemName, data.DisplayName, data.NewId);
                unitTestRef = new CacheObjectReference(unitTestId, systemName);
                writeComponent.WriteUnitTests.Add(unitTestRef);
                writeComponent.BeginSave();

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

        public void SetUnitTestNextTime(SetUnitTestNextTimeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.UnitTestId == null)
            {
                throw new ParameterRequiredException("UnitTestId");
            }
            var cache = new AccountCache();
            using (var unitTest = cache.UnitTests.Write(data.UnitTestId.Value))
            {
                if (data.NextTime == null)
                {
                    unitTest.NextExecutionDate = _timeService.Now();
                }
                else
                {
                    unitTest.NextExecutionDate = data.NextTime.Value;
                }
                unitTest.BeginSave();
            }
        }

        public void SetUnitTestNextStepProcessTime(SetUnitTestNextStepProcessTimeRequestData data)
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
            var cache = new AccountCache();
            using (var unitTest = cache.UnitTests.Write(data.UnitTestId.Value))
            {
                unitTest.NextExecutionDate = data.NextStepProcessTime;
                unitTest.AttempCount++;
                unitTest.BeginSave();
            }
        }

        public void UpdateUnitTest(UpdateUnitTestRequestData data)
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
                    ObjectId = data.ComponentId.Value
                };
                var component = AllCaches.Components.Find(req);
                if (component == null)
                {
                    throw new UnknownComponentIdException(data.ComponentId.Value);
                }
            }

            var request = new AccountCacheRequest()
            {
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
                    unitTest.NextExecutionDate = _timeService.Now();
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
            var statusService = new BulbService(_storage, _timeService);
            var statusData = statusService.GetRaw(unitTest.StatusDataId);
            if (!statusData.HasSignal)
            {
                var now = _timeService.Now();
                var noSignalEvent = GetNoSignalEvent(unitTest, now, now);
                SaveResultEvent(now, unitTest, noSignalEvent, null);
            }
        }

        public void Delete(Guid unitTestId)
        {
            // удаляем юнит-тест
            var unitTestRequest = new AccountCacheRequest()
            {
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
                ObjectId = componentId
            });
            foreach (var unitTestRef in component1.UnitTests.GetAll())
            {
                var unitTest = AllCaches.UnitTests.Find(new AccountCacheRequest()
                {
                    ObjectId = unitTestRef.Id
                });
                if (unitTest != null && unitTest.IsDeleted == false)
                {
                    allUnitTestsStatusDataIds.Add(unitTest.StatusDataId);
                }
            }
            var statuservice = new BulbService(_storage, _timeService);
            statuservice.CalculateByChilds(componentUnitTestsStatudDataId, allUnitTestsStatusDataIds.ToArray());
        }

        public Guid AddPingUnitTest(AddPingUnitTestRequestData data)
        {
            var unitTestCache = GetOrCreateUnitTest(new GetOrCreateUnitTestRequestDataDto()
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
                unitTest.NextExecutionDate = _timeService.Now();
                unitTest.BeginSave();
                unitTest.WaitSaveChanges();
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

        public Guid AddHttpUnitTest(AddHttpUnitTestRequestData data)
        {
            var unitTestCache = GetOrCreateUnitTest(new GetOrCreateUnitTestRequestDataDto()
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
                unitTest.NextExecutionDate = _timeService.Now();
                unitTest.BeginSave();
                unitTest.WaitSaveChanges();
                id = unitTest.Id;
            }

            using (var transaction = _storage.BeginTransaction())
            {
                foreach (var ruleData in data.Rules)
                {
                    var rule = new HttpRequestUnitTestRuleForAdd()
                    {
                        Id = Ulid.NewUlid(),
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
                ObjectId = unitTest.StatusDataId
            };
            using (var statusData = AllCaches.StatusDatas.Write(request))
            {
                // сохраним результаты
                var eventService = new EventService(_storage, _timeService);
                IEventCacheReadObject lastEvent = null;
                if (statusData.LastEventId.HasValue)
                {
                    lastEvent = eventService.GetEventCacheOrNullById(
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
                eventService.Add(newEvent);

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
                var statusService = new BulbService(_storage, _timeService);
                var newEventForRead = _storage.Events.GetOneById(newEvent.Id);
                var signal = BulbSignal.Create(processDate, newEventForRead, noSignalImportance);
                var newStatus = statusService.SetSignal(unitTest.StatusDataId, signal);

                return newStatus;
            }
        }

        public IBulbCacheReadObject SendUnitTestResult(SendUnitTestResultRequestDataDto data)
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
                ObjectId = data.UnitTestId.Value
            }))
            {
                var processDate = _timeService.Now();

                // получим актуальную колбасу, чтобы не было пробелов между результатами
                GetUnitTestResultInternal(unitTest, processDate);

                // если проверка выключена или выключен один из её родителей
                if (unitTest.CanProcess == false)
                {
                    throw new ResponseCodeException(ResponseCode.ObjectDisabled, "Проверка выключена");
                }

                var checker = AccountLimitsCheckerManager.GetChecker();
                var size = data.GetSize();

                try
                {
                    try
                    {
                        var resultEvent = CreateUnitTestResultEvent(unitTest, data, processDate);
                        if (resultEvent.ActualDate < processDate)
                        //todo это источник проблем из-за разного времени клиента и сервера
                        {
                            // код ниже никогда не выполнится, но пусть на всякий случай будет
                            throw new ResponseCodeException(
                                ResponseCode.ServerError,
                                "Результат юнит-теста неактуален (date + actualInterval < now)");
                        }

                        // сохраним результаты
                        var result = SaveResultEvent(processDate, unitTest, resultEvent, data.NextExecutionTime, data.AttempCount);

                        return result;
                    }
                    finally
                    {
                        checker.AddUnitTestsSizePerDay(_storage, size);
                    }
                }
                finally
                {
                    checker.AddUnitTestResultsPerDay(_storage, unitTest.Id);
                }
            }
        }

        public void SendUnitTestResults(SendUnitTestResultRequestDataDto[] data)
        {
            foreach (var item in data)
            {
                SendUnitTestResult(item);
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
                && unitTest.DisableToDate < _timeService.Now())
            {
                using (var unitTestWrite = AllCaches.UnitTests.Write(unitTest))
                {
                    unitTestWrite.Enable = true;
                    unitTestWrite.BeginSave();
                    unitTest = unitTestWrite;
                }
            }

            var statusService = new BulbService(_storage, _timeService);
            var data = statusService.GetRaw(unitTest.StatusDataId);

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
            var cache = new AccountCache();
            var unittestType = cache.UnitTestTypes.Read(unitTest.TypeId);

            var noSignalImportance = unitTest.IsSystemType
                ? EventImportance.Unknown
                : ImportanceHelper.Get(unitTest.NoSignalColor) ??
                  ImportanceHelper.Get(unittestType.NoSignalColor) ??
                  EventImportance.Alarm;

            return new EventForAdd()
            {
                Id = Ulid.NewUlid(),
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

        public IBulbCacheReadObject GetUnitTestResult(Guid unitTestId)
        {
            var cache = new AccountCache();
            var unitTest = cache.UnitTests.Read(unitTestId);
            var processDate = _timeService.Now();
            return GetUnitTestResultInternal(unitTest, processDate);
        }

        private IBulbCacheReadObject UpdateEnableOrDisableStatusData(IUnitTestCacheReadObject unitTest)
        {
            var statusService = new BulbService(_storage, _timeService);
            IBulbCacheReadObject data = null;
            var processDate = _timeService.Now();
            if (unitTest.CanProcess)
            {
                var unknownSignal = BulbSignal.CreateUnknown(processDate);
                data = statusService.SetSignal(unitTest.StatusDataId, unknownSignal);
            }
            else
            {
                var disableSignal = BulbSignal.CreateDisable(processDate);
                data = statusService.SetSignal(unitTest.StatusDataId, disableSignal);
            }

            var componentService = new ComponentService(_storage, _timeService);
            componentService.CalculateAllStatuses(unitTest.ComponentId);

            return data;
        }

        public IBulbCacheReadObject Disable(SetUnitTestDisableRequestDataDto requestData)
        {
            var unitTestId = requestData.UnitTestId;
            var cache = new AccountCache();
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

        public IBulbCacheReadObject Enable(Guid unitTestId)
        {
            var cache = new AccountCache();
            IUnitTestCacheReadObject unitTestRead = null;

            // изменим юнит-тест
            using (var unitTest = cache.UnitTests.Write(unitTestId))
            {
                unitTestRead = unitTest;
                unitTest.Enable = true;
                unitTest.BeginSave();
            }

            // обновим колбаски
            var statusService = new BulbService(_storage, _timeService);
            var processDate = _timeService.Now();
            var unknownSignal = BulbSignal.CreateUnknown(processDate);
            var data = statusService.SetSignal(unitTestRead.StatusDataId, unknownSignal);

            var componentService = new ComponentService(_storage, _timeService);
            componentService.CalculateAllStatuses(unitTestRead.ComponentId);

            return data;
        }

        public int RecalcUnitTestsResults(int maxCount)
        {
            var unitTestsIds = _storage.UnitTests.GetNotActualIds(_timeService.Now(), maxCount);
            foreach (var unitTestId in unitTestsIds)
            {
                GetUnitTestResult(unitTestId);
            }
            return unitTestsIds.Length;
        }

        public string GetFullDisplayName(UnitTestForRead unittest)
        {
            var component = _storage.Components.GetOneById(unittest.ComponentId);
            var componentService = new ComponentService(_storage, _timeService);
            return componentService.GetFullDisplayName(component) + " / " + unittest.DisplayName;
        }

    }
}
