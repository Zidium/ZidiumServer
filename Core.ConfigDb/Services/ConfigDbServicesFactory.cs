namespace Zidium.Core.ConfigDb
{
    internal class ConfigDbServicesFactory : IConfigDbServicesFactory
    {
        public IAccountService GetAccountService()
        {
            return new AccountService();
        }

        public IAccountManagementService GetAccountManagementService()
        {
            return new AccountManagementService();
        }

        public IDatabaseService GetDatabaseService()
        {
            return new DatabaseService();
        }

        public IPaymentService GetPaymentService()
        {
            return new PaymentService();
        }

        public ILoginService GetLoginService()
        {
            return new LoginService(GetAccountService());
        }

        public ISettingService GetSettingService()
        {
            return new SettingService();
        }

        public IUrlService GetUrlService()
        {
            return new UrlService();
        }

        public IUserAccountActivityService GetUserAccountActivityService()
        {
            return new UserAccountActivityService();
        }

    }
}
