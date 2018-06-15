using System;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.DispatcherLayer;
using Zidium.Core.Limits;

namespace Zidium.Core.AccountsDb
{
    public class UnitTestTypeService : IUnitTestTypeService
    {
        protected DispatcherContext Context { get; set; }

        public UnitTestTypeService(DispatcherContext dispatcherContext)
        {
            if (dispatcherContext == null)
            {
                throw new ArgumentNullException("dispatcherContext");
            }
            Context = dispatcherContext;
        }

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
                var unitTestTypeCache = AllCaches.UnitTestTypes.FindByName(accountId, data.SystemName);

                if (unitTestTypeCache != null)
                    return unitTestTypeCache;

                var accountDbContext = Context.GetAccountDbContext(accountId);

                // Проверим лимит
                var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
                var checkResult = checker.CheckMaxUnitTestTypes(accountDbContext);
                if (!checkResult.Success)
                    throw new OverLimitException(checkResult.Message);

                Guid unitTestTypeId;

                // создаем отдельный контекст, чтобы после обработки объектов кэшем основной контекст загрузил актуальную версию из БД
                using (var accountDbContextNew = AccountDbContext.CreateFromAccountIdLocalCache(accountId))
                {
                    if (string.IsNullOrEmpty(data.DisplayName))
                        data.DisplayName = data.SystemName;

                    var unitTestType = new UnitTestType()
                    {
                        SystemName = data.SystemName,
                        DisplayName = data.DisplayName,
                        IsSystem = false,
                        ActualTimeSecs = data.ActualTimeSecs,
                        NoSignalColor = data.NoSignalColor
                    };

                    var unitTestTypeRepository = accountDbContextNew.GetUnitTestTypeRepository();
                    unitTestTypeRepository.Add(unitTestType);
                    unitTestTypeId = unitTestType.Id;
                }

                checker.RefreshUnitTestTypesCount();

                unitTestTypeCache = AllCaches.UnitTestTypes.Read(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = unitTestTypeId
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

        public IUnitTestTypeCacheReadObject UpdateUnitTestType(Guid accountId, UpdateUnitTestTypeRequestData data)
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
                var request = new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = data.UnitTestTypeId.Value
                };

                var unitTestType = AllCaches.UnitTestTypes.Read(request);

                // Если поменялось системное имя, то проверим, что нет другого типа проверки с таким системным именем
                if (!string.Equals(unitTestType.SystemName, data.SystemName, StringComparison.OrdinalIgnoreCase))
                {
                    var anotherUnitTestType = AllCaches.UnitTestTypes.FindByName(accountId, data.SystemName);
                    if (anotherUnitTestType != null && anotherUnitTestType.Id != unitTestType.Id)
                        throw new ArgumentException("Тип проверки с таким системным именем уже существует");
                }

                using (var unitTestTypeWrite = AllCaches.UnitTestTypes.Write(request))
                {
                    unitTestTypeWrite.SystemName = data.SystemName;
                    unitTestTypeWrite.DisplayName = data.DisplayName;
                    unitTestTypeWrite.ActualTimeSecs = data.ActualTimeSecs;
                    unitTestTypeWrite.NoSignalColor = data.NoSignalColor;
                    unitTestTypeWrite.BeginSave();
                    unitTestTypeWrite.WaitSaveChanges();
                }

                return AllCaches.UnitTestTypes.Read(request);

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
