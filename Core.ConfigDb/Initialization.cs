namespace Zidium.Core.ConfigDb
{
    public static class Initialization
    {
        public static void SetServices()
        {
            DependencyInjection.SetServicePersistent<IConfigDbServicesFactory>(new ConfigDbServicesFactory());
        }
    }
}
