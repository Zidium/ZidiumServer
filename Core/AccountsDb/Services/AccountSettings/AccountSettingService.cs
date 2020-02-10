using System;

namespace Zidium.Core.AccountsDb.Services.AccountSettings
{
    class AccountSettingService : IAccountSettingService
    {
        protected AccountDbContext Context { get; set; }

        private const string VIRUS_TOTAL_API_KEY = "VirusTotal_ApiKey";

        public AccountSettingService(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private string GetValue(string name)
        {
            return Context.GetAccountSettingRepository().GetValue(name);
        }

        private void SetValue(string name, string value)
        {
            Context.GetAccountSettingRepository().SetValue(name, value);
        }

        public string VirusTotalApiKey
        {
            get => GetValue(VIRUS_TOTAL_API_KEY);
            set => SetValue(VIRUS_TOTAL_API_KEY, value);
        }
    }
}
