using System.Collections.Generic;

namespace Zidium.Core.AccountsDb
{
    public interface IMetricRepository : IAccountBasedRepository<Metric>
    {
        List<Metric> GetNotActual(int maxCount);

        int GetCount();
    }
}
