using System.Web.Mvc;

namespace Zidium.UserAccount.Controllers
{
    public class AddInController : BaseController
    {
        public PartialViewResult SystemMenuPartial()
        {
            return PartialView();
        }

        public PartialViewResult FooterPartial()
        {
            return PartialView();
        }
    }
}
