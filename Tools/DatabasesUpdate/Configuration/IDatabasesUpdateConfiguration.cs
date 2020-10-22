using Zidium.Common;

namespace Zidium.DatabasesUpdate
{
    internal interface IDatabasesUpdateConfiguration
    {
        IDatabaseConfiguration WorkDatabase { get; }
        
        IDatabaseConfiguration TestDatabase { get; }
        
        IDatabaseConfiguration LocalDatabase { get; }
    }
}
