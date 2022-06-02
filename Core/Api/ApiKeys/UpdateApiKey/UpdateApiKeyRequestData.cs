using System;

namespace Zidium.Core.Api
{
    public class UpdateApiKeyRequestData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? UserId { get; set; }
    }
}
