using System;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Запрос на регистрацию аккаунта - шаг 3
    /// </summary>
    public class AccountRegistrationStep3RequestData
    {
        public Guid RegId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FatherName { get; set; }

        public string Phone { get; set; }
    }
}
