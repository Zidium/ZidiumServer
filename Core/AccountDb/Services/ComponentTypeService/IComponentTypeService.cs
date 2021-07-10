using System;
using Zidium.Core.Api;
using Zidium.Storage;
using Zidium.Api.Dto;

namespace Zidium.Core.AccountsDb
{
    public interface IComponentTypeService
    {
        ComponentTypeForRead GetOrCreateComponentType(GetOrCreateComponentTypeRequestDataDto data);

        void UpdateComponentType(UpdateComponentTypeRequestDataDto data);

        void Delete(Guid typeId);
    }
}
