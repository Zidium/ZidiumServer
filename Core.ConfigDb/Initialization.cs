namespace Zidium.Core.ConfigDb
{
    public static class Initialization
    {
        public static void SetServices()
        {
            DependencyInjection.SetService<IAccountService, AccountService>();
            DependencyInjection.SetService<IAccountManagementService, AccountManagementService>();
            DependencyInjection.SetService<IDatabaseService, DatabaseService>();
            DependencyInjection.SetService<ILoginService, LoginService>();
            DependencyInjection.SetService<IPaymentService, PaymentService>();
            DependencyInjection.SetService<ISettingService, SettingService>();
            DependencyInjection.SetService<IUrlService, UrlService>();
            DependencyInjection.SetService<IUserAccountActivityService, UserAccountActivityService>();
        }
    }
}
