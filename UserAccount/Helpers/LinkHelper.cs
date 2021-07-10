using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Zidium.UserAccount
{
    public static class LinkHelper
    {
        //Builds URL by finding the best matching route that corresponds to the current URL,
        //with given parameters added or replaced.
        public static IHtmlContent Current(this UrlHelper helper, object substitutes = null, string action = null, string controller = null)
        {
            //get the route data for the current URL e.g. /Research/InvestmentModelling/RiskComparison
            //this is needed because unlike UrlHelper.Action, UrlHelper.RouteUrl sets includeImplicitMvcValues to false
            //which causes it to ignore current ViewContext.RouteData.Values
            var rd = new RouteValueDictionary(helper.ActionContext.RouteData.Values);

            //get the current query string e.g. ?BucketID=17371&amp;compareTo=123
            var qs = helper.ActionContext.HttpContext.Request.Query;

            //add query string parameters to the route value dictionary
            foreach (string param in qs.Keys)
            {
                if (param != null)
                {
                    if (!string.IsNullOrEmpty(qs[param]))
                        rd[param] = qs[param];
                }
            }

            // override action
            if (!string.IsNullOrEmpty(action))
                rd["action"] = action;

            // override controller
            if (!string.IsNullOrEmpty(controller))
                rd["controller"] = controller;

            //override parameters we're changing
            if (substitutes != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
                {
                    var value = property.GetValue(substitutes);
                    if (value == null)
                        rd.Remove(property.Name);
                    else
                        rd[property.Name] = value;
                }
            }

            // remove ajax trash
            rd.Remove("X-Requested-With");
            rd.Remove("_");

            //UrlHelper will find the first matching route
            //(the routes are searched in the order they were registered).
            //The unmatched parameters will be added as query string.
            var rc = new UrlRouteContext()
            {
                Values = rd
            };

            var url = helper.RouteUrl(rc);
            return new HtmlString(url);
        }

        public static string ToAbsolute(this IUrlHelper urlHelper, string url)
        {
            var uri = new Uri(new Uri(urlHelper.ActionContext.HttpContext.Request.GetDisplayUrl()), url);
            return uri.AbsoluteUri;
        }

        public static string GenerateUrl(IUrlHelper urlHelper, string action, string controller, object values)
        {
            var ac = new UrlActionContext()
            {
                Controller = controller,
                Action = action,
                Values = values
            };
            return urlHelper.Action(ac);
        }
    }
}