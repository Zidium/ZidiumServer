using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
{
    /// <summary>
    /// Может изменять личные данные - пользователь и подписки.
    /// </summary>
    public class CanEditPrivateDataFilter : PermissionFilter
    {
        public CanEditPrivateDataFilter(ITempDataDictionaryFactory tempDataDictionaryFactory) : base(tempDataDictionaryFactory)
        {
        }

        public override bool CheckPermissions(HttpContext httpContext)
        {
            return UserHelper.CurrentUser(httpContext).CanEditPrivateData();
        }
    }
}