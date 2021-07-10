using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
{
    /// <summary>
    /// Может изменять все данные. Может видеть секретный ключ аккаунта.
    /// </summary>
    public class CanEditAllDataFilter : PermissionFilter
    {
        public CanEditAllDataFilter(ITempDataDictionaryFactory tempDataDictionaryFactory) : base(tempDataDictionaryFactory)
        {
        }

        public override bool CheckPermissions(HttpContext httpContext)
        {
            return UserHelper.CurrentUser(httpContext).CanEditCommonData();
        }
    }
}