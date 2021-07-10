namespace Zidium.Storage
{
    public interface IStorageFactory
    {
        IStorage GetStorage(string connectionString);
    }
}
