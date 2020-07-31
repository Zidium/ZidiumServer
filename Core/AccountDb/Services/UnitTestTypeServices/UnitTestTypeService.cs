using System;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Limits;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class UnitTestTypeService : IUnitTestTypeService
    {
        public UnitTestTypeService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        public IUnitTestTypeCacheReadObject GetOrCreateUnitTestType(Guid accountId, GetOrCreateUnitTestTypeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (string.IsNullOrEmpty(data.SystemName))
            {
                throw new ParameterRequiredException("SystemName");
            }

            var lockObj = typeof(UnitTestTypeService);
            lock (lockObj)
            {
                var unitTestTypeCache = AllCaches.UnitTestTypes.FindByName(accountId, data.SystemName, _storage);

                if (unitTestTypeCache != null)
                    return unitTestTypeCache;

                // Проверим лимит
                var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
                var checkResult = checker.CheckMaxUnitTestTypes(_storage);
                if (!checkResult.Success)
                    throw new OverLimitException(checkResult.Message);

                if (string.IsNullOrEmpty(data.DisplayName))
                    data.DisplayName = data.SystemName;

                var unitTestType = new UnitTestTypeForAdd()
                {
                    Id = Guid.NewGuid(),
                    SystemName = data.SystemName,
                    DisplayName = data.DisplayName,
                    IsSystem = false,
                    ActualTimeSecs = data.ActualTimeSecs,
                    NoSignalColor = data.NoSignalColor
                };
                _storage.UnitTestTypes.Add(unitTestType);

                checker.RefreshUnitTestTypesCount();

                unitTestTypeCache = AllCaches.UnitTestTypes.Read(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = unitTestType.Id
                });

                return unitTestTypeCache;
            }
        }

        public IUnitTestTypeCacheReadObject GetUnitTestTypeById(Guid accountId, Guid id)
        {
            var unitTestTypeCache = AllCaches.UnitTestTypes.Read(new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = id
            });

            return unitTestTypeCache;
        }

        public void UpdateUnitTestType(Guid accountId, UpdateUnitTestTypeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.UnitTestTypeId == null)
            {
                throw new ParameterRequiredException("UnitTestTypeId");
            }

            var lockObject = LockObject.ForUnitTestType(data.UnitTestTypeId.Value);

            lock (lockObject)
            {
                var cache = new AccountCache(accountId);
                var unitTestType = cache.UnitTestTypes.Read(data.UnitTestTypeId.Value);

                // Если поменялось системное имя, то проверим, что нет другого типа проверки с таким системным именем
                if (!string.Equals(unitTestType.SystemName, data.SystemName, StringComparison.OrdinalIgnoreCase))
                {
                    var anotherUnitTestType = AllCaches.UnitTestTypes.FindByName(accountId, data.SystemName, _storage);
                    if (anotherUnitTestType != null && anotherUnitTestType.Id != unitTestType.Id)
                        throw new ArgumentException("Тип проверки с таким системным именем уже существует");
                }

                bool noSignalColorChanged;
                using (var unitTestTypeWrite = cache.UnitTestTypes.Write(unitTestType))
                {
                    noSignalColorChanged = data.NoSignalColor != unitTestTypeWrite.NoSignalColor;
                    unitTestTypeWrite.SystemName = data.SystemName;
                    unitTestTypeWrite.DisplayName = data.DisplayName;
                    unitTestTypeWrite.ActualTimeSecs = data.ActualTimeSecs;
                    unitTestTypeWrite.NoSignalColor = data.NoSignalColor;
                    unitTestTypeWrite.BeginSave();
                    unitTestTypeWrite.WaitSaveChanges();
                }

                // при изменении "Цвет если нет сигнала" нужно обновить цвет всех проверок этого типа
                if (noSignalColorChanged)
                {
                    var unitTests = _storage.UnitTests.GetByUnitTestTypeId(unitTestType.Id).Select(t => t.Id).ToArray();

                    var unitTestService = new UnitTestService(_storage);
                    foreach (var unitTestId in unitTests)
                    {
                        var unitTestCacheReadObject = cache.UnitTests.Read(unitTestId);
                        unitTestService.UpdateNoSignalColor(unitTestCacheReadObject);
                    }
                }
            }
        }

        public void DeleteUnitTestType(Guid accountId, Guid unitTestTypeId)
        {
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = unitTestTypeId
            };

            using (var unitTest = AllCaches.UnitTestTypes.Write(request))
            {
                unitTest.IsDeleted = true;
                unitTest.BeginSave();
                unitTest.WaitSaveChanges();
            }

            var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            checker.RefreshUnitTestTypesCount();
        }
    }
}
