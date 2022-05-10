using System;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Storage;
using Zidium.Api.Dto;
using Zidium.Common;

namespace Zidium.Core.AccountsDb
{
    public class UnitTestTypeService : IUnitTestTypeService
    {
        public UnitTestTypeService(IStorage storage, ITimeService timeService)
        {
            _storage = storage;
            _timeService = timeService;
        }

        private readonly IStorage _storage;
        private readonly ITimeService _timeService;

        public IUnitTestTypeCacheReadObject GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDataDto data)
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
                var unitTestTypeCache = AllCaches.UnitTestTypes.FindByName(data.SystemName, _storage);

                if (unitTestTypeCache != null)
                    return unitTestTypeCache;

                if (string.IsNullOrEmpty(data.DisplayName))
                    data.DisplayName = data.SystemName;

                var unitTestType = new UnitTestTypeForAdd()
                {
                    Id = Ulid.NewUlid(),
                    SystemName = data.SystemName,
                    DisplayName = data.DisplayName,
                    IsSystem = false,
                    ActualTimeSecs = data.ActualTimeSecs,
                    NoSignalColor = data.NoSignalColor
                };
                _storage.UnitTestTypes.Add(unitTestType);

                unitTestTypeCache = AllCaches.UnitTestTypes.Read(new AccountCacheRequest()
                {
                    ObjectId = unitTestType.Id
                });

                return unitTestTypeCache;
            }
        }

        public IUnitTestTypeCacheReadObject GetUnitTestTypeById(Guid id)
        {
            var unitTestTypeCache = AllCaches.UnitTestTypes.Read(new AccountCacheRequest()
            {
                ObjectId = id
            });

            return unitTestTypeCache;
        }

        public void UpdateUnitTestType(UpdateUnitTestTypeRequestData data)
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
                var cache = new AccountCache();
                var unitTestType = cache.UnitTestTypes.Read(data.UnitTestTypeId.Value);

                // Если поменялось системное имя, то проверим, что нет другого типа проверки с таким системным именем
                if (!string.Equals(unitTestType.SystemName, data.SystemName, StringComparison.OrdinalIgnoreCase))
                {
                    var anotherUnitTestType = AllCaches.UnitTestTypes.FindByName(data.SystemName, _storage);
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

                    var unitTestService = new UnitTestService(_storage, _timeService);
                    foreach (var unitTestId in unitTests)
                    {
                        var unitTestCacheReadObject = cache.UnitTests.Read(unitTestId);
                        unitTestService.UpdateNoSignalColor(unitTestCacheReadObject);
                    }
                }
            }
        }

        public void DeleteUnitTestType(Guid unitTestTypeId)
        {
            var request = new AccountCacheRequest()
            {
                ObjectId = unitTestTypeId
            };

            using (var unitTest = AllCaches.UnitTestTypes.Write(request))
            {
                unitTest.IsDeleted = true;
                unitTest.BeginSave();
                unitTest.WaitSaveChanges();
            }
        }
    }
}
