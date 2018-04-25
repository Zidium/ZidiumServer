using System;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.DispatcherLayer;
using Zidium.Core.Limits;

namespace Zidium.Core.AccountsDb
{
    class ComponentTypeService : IComponentTypeService
    {
        protected DispatcherContext Context { get; set; }

        public ComponentTypeService(DispatcherContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public ComponentType GetOrCreateComponentType(
            Guid accountId,
            GetOrCreateComponentTypeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.DisplayName == null)
            {
                data.DisplayName = data.SystemName;
            }
            if (data.SystemName == null)
            {
                data.SystemName = data.DisplayName;
            }
            if (string.IsNullOrEmpty(data.SystemName))
            {
                throw new UserFriendlyException("Не задано системное имя типа компонента");
            }

            var context = Context.GetAccountDbContext(accountId);
            var repository = context.GetComponentTypeRepository();

            var lockObject = LockObject.ForAccount(accountId);
            lock (lockObject)
            {
                var result = repository.GetOneOrNullBySystemName(data.SystemName);
                if (result == null)
                {
                    // Проверим лимит
                    var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
                    var limitCheckResult = checker.CheckMaxComponentTypes(context);
                    if (!limitCheckResult.Success)
                        throw new OverLimitException(limitCheckResult.Message);

                    result = repository.Add(data.DisplayName, data.SystemName);

                    checker.RefreshComponentTypesCount();
                }
                return result;
            }
        }

        public bool Contains(Guid accountId, Guid componentTypeId)
        {
            var accountdbContext = Context.GetAccountDbContext(accountId);
            var type = accountdbContext.ComponentTypes.SingleOrDefault(x => x.Id == componentTypeId);
            return type != null;
        }

        public ComponentType GetOneOrNullBySystemName(Guid accountId, string systemName)
        {
            var accountdbContext = Context.GetAccountDbContext(accountId);
            var repository = accountdbContext.GetComponentTypeRepository();
            return repository.GetOneOrNullBySystemName(systemName);
        }

        public ComponentType GetOneOrNullById(Guid accountId, Guid typeId)
        {
            var accountdbContext = Context.GetAccountDbContext(accountId);
            var repository = accountdbContext.GetComponentTypeRepository();
            return repository.GetByIdOrNull(typeId);
        }

        public ComponentType GetBySystemName(Guid accountId, string systemName)
        {
            var componentType = GetOneOrNullBySystemName(accountId, systemName);
            if (componentType == null)
                throw new ParameterErrorException("SystemName");
            return componentType;
        }

        public ComponentType GetById(Guid accountId, Guid typeId)
        {
            var componentType = GetOneOrNullById(accountId, typeId);
            if (componentType == null)
                throw new UnknownComponentTypeIdException(typeId);
            return componentType;
        }

        public ComponentType UpdateComponentType(Guid accountId, UpdateComponentTypeData data)
        {
            var type = GetById(accountId, data.Id.Value);
            if (type.IsSystem)
            {
                throw new CantUpdateSystemObjectException();
            }
            if (string.IsNullOrEmpty(data.DisplayName) == false)
            {
                type.DisplayName = data.DisplayName;
            }
            if (string.IsNullOrEmpty(data.SystemName) == false)
            {
                type.SystemName = data.SystemName;
            }
            var accountdbContext = Context.GetAccountDbContext(accountId);
            accountdbContext.SaveChanges();
            return type;
        }

        public void Delete(Guid accountId, Guid typeId)
        {
            var repository = Context.GetAccountDbContext(accountId).GetComponentTypeRepository();
            var componentType = repository.GetById(typeId);
            repository.Remove(componentType);

            var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            checker.RefreshComponentTypesCount();

            var componentService = Context.ComponentService;
            foreach (var component in componentType.Components.ToArray())
                componentService.DeleteComponent(accountId, component.Id);
        }
    }
}
