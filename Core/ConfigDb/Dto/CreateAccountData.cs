using System;

namespace Zidium.Core.ConfigDb
{
    public class CreateAccountData
    {
        public string AccountName { get; set; }
        public string CompanyName { get; set; }
        public string AdminEmail { get; set; }
        public string AdminLastName { get; set; }
        public string AdminFirstName { get; set; }
        public string AdminMiddleName { get; set; }
        public string AdminPost { get; set; }
        public string AdminMobilePhone { get; set; }
        public Guid? RegistrationId { get; set; }
        public AccountType Type { get; set; }
        public Guid? FixedAccountId { get; set; }

        /// <summary>
        /// Пароль на логин администратора.
        /// Если не задано, то будет сгенерировано случайным образом
        /// </summary>
        public string AdminPassword { get; set; }
    }
}
