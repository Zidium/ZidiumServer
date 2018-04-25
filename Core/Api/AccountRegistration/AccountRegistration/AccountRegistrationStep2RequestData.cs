using System;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Запрос на регистрацию аккаунта - шаг 2
    /// </summary>
    public class AccountRegistrationStep2RequestData
    {
        public Guid RegId { get; set; }

        public string CompanyName { get; set; }

        public string Site { get; set; }

        public string CompanyPost { get; set; }
    }
}
