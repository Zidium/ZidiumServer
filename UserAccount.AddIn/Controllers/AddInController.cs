using System.Web.Mvc;

namespace Zidium.UserAccount.Controllers
{
    public class AddInController : ContextController
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
