namespace Zidium.Storage.Ef
{
    public class StorageFactory : IStorageFactory
    {
        public IStorage GetStorage()
        {
            return new Storage();
        }
    }
}
