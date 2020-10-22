using Zidium.Common;

namespace Zidium.Core.ConfigDb
{
    internal class ConfigDbConfiguration : JsonConfiguration<Options>, IConfigDbConfiguration
    {
        public string AccountSecretKey => Get().AccountSecretKey;

        public string AccountWebSite => Get().AccountWebSite;
    }
}
