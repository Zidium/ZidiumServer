using Zidium.Storage;

namespace Zidium.Core
{
    public interface IDefaultStorageFactory
    {
        IStorage GetStorage();
    }
}
