using System;
using Zidium.Core.ConfigDb;

namespace Zidium.UserAccount.Models
{
    public class ShowAccountModel
    {
        public Guid Id { get; set; }

        public string AccountName { get; set; }

        public string SecretKey { get; set; }
        
        public string TariffName { get; set; }

        public AccountType AccountType { get; set; }
    }
}