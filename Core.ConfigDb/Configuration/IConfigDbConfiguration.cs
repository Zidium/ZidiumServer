namespace Zidium.Core.ConfigDb
{
    internal interface IConfigDbConfiguration
    {
        string AccountSecretKey { get; }

        string AccountWebSite { get; }
    }
}
