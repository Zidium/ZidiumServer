using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
{
    /// <summary>
    /// Может управлять аккаунтом - изменять пользователей, тариф, проводить оплату и т.п.
    /// Может смотреть партнёрскую программу.
    /// </summary>
    public class CanManageAccountFilter : PermissionFilter
    {
        public CanManageAccountFilter(ITempDataDictionaryFactory tempDataDictionaryFactory) : base(tempDataDictionaryFactory)
        {
        }

        public override bool CheckPermissions(HttpContext httpContext)
        {
            return UserHelper.CurrentUser(httpContext).CanManageAccount();
        }
    }
}