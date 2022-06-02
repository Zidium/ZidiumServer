using System;

namespace Zidium.UserAccount.Models.ApiKeys
{
    public class IndexModel
    {
        public ApiKeyInfo[] ApiKeys { get; set; }

        public class ApiKeyInfo
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public DateTime UpdatedAt { get; set; }

            public string User { get; set; }
        }
    }
}
