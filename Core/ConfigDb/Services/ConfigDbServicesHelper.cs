using Zidium.Core.Common;
using Zidium.Core.DispatcherLayer;

namespace Zidium.Core.ConfigDb
{
    public static class ConfigDbServicesHelper
    {
        public static IAccountService GetAccountService()
        {
            return DependencyInjection.GetService<IAccountService>();
        }

        public static IAccountManagementService GetAccountManagementService(DispatcherContext context)
        {
            return DependencyInjection.GetService<IAccountManagementService>(context);
        }

        public static IDatabaseService GetDatabaseService()
        {
            return DependencyInjection.GetService<IDatabaseService>();
        }

        public static IPaymentService GetPaymentService(DatabasesContext context)
        {
            return DependencyInjection.GetService<IPaymentService>(context);
        }

        public static ILoginService GetLoginService()
        {
            return DependencyInjection.GetService<ILoginService>();
        }

        public static ISettingService GetSettingService()
        {
            return DependencyInjection.GetService<ISettingService>();
        }

        public static IUrlService GetUrlService()
        {
            return DependencyInjection.GetService<IUrlService>();
        }

        public static IUserAccountActivityService GetUserAccountActivityService()
        {
            return DependencyInjection.GetService<IUserAccountActivityService>();
        }
    }
}
