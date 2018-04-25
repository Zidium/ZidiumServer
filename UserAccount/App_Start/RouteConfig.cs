using System.Web.Mvc;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;

namespace Zidium.UserAccount
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Системные

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Компоненты

            routes.MapRoute(
                name: "ShowComponent",
                url: "Components/{id}",
                defaults: new { controller = "Components", action = "Show" },
                constraints: new { id = new GuidRouteConstraint() }
            );

            // Типы компонентов

            routes.MapRoute(
                name: "ShowComponentType",
                url: "ComponentTypes/{id}",
                defaults: new { controller = "ComponentTypes", action = "Show" },
                constraints: new { id = new GuidRouteConstraint() }
            );

            // События

            routes.MapRoute(
                name: "ShowEvent",
                url: "Events/{id}",
                defaults: new { controller = "Events", action = "Show" },
                constraints: new { id = new GuidRouteConstraint() }
            );

            // Пользователи

            routes.MapRoute(
                name: "ShowUser",
                url: "Users/{id}",
                defaults: new { controller = "Users", action = "Show" },
                constraints: new { id = new GuidRouteConstraint() }
            );

            // Заглавная страница

            routes.MapRoute(
                name: "StartPage",
                url: "",
                defaults: new { controller = "Home", action = "Start", id = UrlParameter.Optional }
            );

            // Общий

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            Routes = routes;
        }

        public static RouteCollection Routes;
    }
}
