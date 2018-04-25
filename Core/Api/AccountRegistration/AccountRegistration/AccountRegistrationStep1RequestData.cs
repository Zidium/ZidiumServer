using System;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Запрос на регистрацию аккаунта - шаг 1
    /// </summary>
    public class AccountRegistrationStep1RequestData
    {
        public string AccountName { get; set; }

        public string EMail { get; set; }

        public Guid? UserAgentTag { get; set; }
    }
}
