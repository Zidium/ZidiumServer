namespace Zidium.Storage.Ef
{
    public class StorageFactory : IStorageFactory
    {
        public IStorage GetStorage(string connectionString)
        {
            return new Storage(_sectionName, connectionString);
        }

        public void OverrideSectionName(string sectionName)
        {
            _sectionName = sectionName;
        }

        private string _sectionName;

        public static void DisableMigrations()
        {
            AccountDbContext.DisableMigrations();
        }

    }
}
