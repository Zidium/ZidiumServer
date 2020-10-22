namespace Zidium.Core.ConfigDb
{
    public static class Initialization
    {
        public static void SetServices()
        {
            var configuration = new ConfigDbConfiguration();
            DependencyInjection.SetServicePersistent<IConfigDbConfiguration>(configuration);

            DependencyInjection.SetServicePersistent<IConfigDbServicesFactory>(new ConfigDbServicesFactory());
        }
    }
}
