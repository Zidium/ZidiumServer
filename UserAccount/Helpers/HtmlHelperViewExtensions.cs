using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Zidium.UserAccount.Helpers
{
    public static class HtmlHelperViewExtensions
    {
        public static IHtmlContent Action(this IHtmlHelper helper, string action, object parameters = null)
        {
            var controller = (string)helper.ViewContext.RouteData.Values["controller"];

            return Action(helper, action, controller, parameters);
        }

        public static IHtmlContent Action(this IHtmlHelper helper, string action, string controller, object parameters = null)
        {
            var area = (string)helper.ViewContext.RouteData.Values["area"];

            return Action(helper, action, controller, area, parameters);
        }

        public static IHtmlContent Action(this IHtmlHelper helper, string action, string controller, string area, object parameters = null)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (controller == null)
                throw new ArgumentNullException("controller");

            var task = RenderActionAsync(helper, action, controller, area, parameters);

            return task.Result;
        }

        private static async Task<IHtmlContent> RenderActionAsync(this IHtmlHelper helper, string action, string controller, string area, object parameters = null)
        {
            // fetching required services for invocation
            var serviceProvider = helper.ViewContext.HttpContext.RequestServices;
            var actionSelector = serviceProvider.GetRequiredService<IActionSelector>();

            // creating new action invocation context
            var routeData = new RouteData();
            foreach (var router in helper.ViewContext.RouteData.Routers)
            {
                routeData.PushState(router, null, null);
            }
            routeData.PushState(null, new RouteValueDictionary(new { controller = controller, action = action, area = area }), null);
            routeData.PushState(null, new RouteValueDictionary(parameters ?? new { }), null);

            //get the action descriptor
            RouteContext routeContext = new RouteContext(helper.ViewContext.HttpContext) { RouteData = routeData };
            var candidates = actionSelector.SelectCandidates(routeContext);
            var actionDescriptor = actionSelector.SelectBestCandidate(routeContext, candidates);

            try
            {
                var newHttpContext = serviceProvider.GetRequiredService<IHttpContextFactory>().Create(helper.ViewContext.HttpContext.Features);
                var body = new MemoryStream();
                newHttpContext.Response.Body = body;
                newHttpContext.Items.TryAdd("ChildAction", true);
                var actionContext = new ActionContext(newHttpContext, routeData, actionDescriptor);
                var invoker = serviceProvider.GetRequiredService<IActionInvokerFactory>().CreateInvoker(actionContext);
                await invoker.InvokeAsync();
                body.Position = 0;
                using (var reader = new StreamReader(body))
                {
                    return new HtmlString(reader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                var logger = DependencyInjection.GetLogger("HtmlHelperViewExtensions");
                logger.LogError(ex, ex.Message);
                return new HtmlString(ex.Message);
            }
        }

        public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.GetView(viewName, viewName, !partial);

                if (!viewResult.Success)
                {
                    return $"A view with the name {viewName} could not be found";
                }

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
