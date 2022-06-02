using System;

namespace Zidium.Core.Api
{
    public class AddApiKeyRequestData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public Guid? UserId { get; set; }
    }
}
