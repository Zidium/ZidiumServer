using System;

namespace Zidium.Storage
{
    public interface IStorageFactory
    {
        IStorage GetStorage(string connectionString);

        void OverrideSectionName(string sectionName);
    }
}
