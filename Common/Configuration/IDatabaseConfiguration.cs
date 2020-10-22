namespace Zidium.Common
{
    public interface IDatabaseConfiguration
    {
        string ProviderName { get; }

        string ConnectionString { get; }
    }
}
