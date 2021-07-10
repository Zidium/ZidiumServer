using Zidium.Common;

namespace Zidium.DatabaseUpdater
{
    internal interface IDatabasesUpdateConfiguration
    {
        IDatabaseConfiguration WorkDatabase { get; }

        IDatabaseConfiguration TestDatabase { get; }
    }
}
