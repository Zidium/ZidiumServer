using System;
using Zidium.Core.Api;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface IComponentTypeService
    {
        ComponentTypeForRead GetOrCreateComponentType(Guid accountId, GetOrCreateComponentTypeRequestData data);

        void UpdateComponentType(Guid accountId, UpdateComponentTypeRequestData data);

        void Delete(Guid accountId, Guid typeId);
    }
}
