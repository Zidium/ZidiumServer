namespace Zidium.Core.ConfigDb
{
    public interface IConfigDbServicesFactory
    {
        IAccountService GetAccountService();

        IAccountManagementService GetAccountManagementService();

        IDatabaseService GetDatabaseService();

        IPaymentService GetPaymentService();

        ILoginService GetLoginService();

        ISettingService GetSettingService();

        IUrlService GetUrlService();

        IUserAccountActivityService GetUserAccountActivityService();
    }
}
