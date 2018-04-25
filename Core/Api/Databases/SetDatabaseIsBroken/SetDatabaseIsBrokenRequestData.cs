using System;

namespace Zidium.Core.Api
{
    public class SetDatabaseIsBrokenRequestData
    {
        public Guid? Id { get; set; }

        public bool? IsBroken { get; set; }
    }
}
