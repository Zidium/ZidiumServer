using System;

namespace Zidium.Core.Caching
{
    public interface ICacheRequest
    {
        Guid ObjectId { get; set; }
    }
}
