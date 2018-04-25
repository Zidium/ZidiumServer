using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Zidium.UserAccount.Helpers
{
    public class RazorHelper
    {
        class FakeController : Controller
        {
            
        }

        public static string GetViewAsString(string filePath, object model)
        {
            var st = new StringWriter();
            var context = new HttpContextWrapper(HttpContext.Current);
            var routeData = new RouteData();
            var controllerContext = new ControllerContext(new RequestContext(context, routeData), new FakeController());
            var razor = new RazorView(controllerContext, filePath, null, false, null);
            razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), st), st);
            return st.ToString();
        }
    }
}