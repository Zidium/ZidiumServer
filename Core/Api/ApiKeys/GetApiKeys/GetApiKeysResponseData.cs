using System;

namespace Zidium.Core.Api
{
    public class GetApiKeysResponseData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid? UserId { get; set; }

        public string User { get; set; }
    }
}
