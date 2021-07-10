using System;
using Microsoft.AspNetCore.Http;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common;
using Zidium.UserAccount.Controllers;

namespace Zidium.UserAccount.Models
{
    public class FullRequestContext
    {
        public UserInfo CurrentUser { get; protected set; }

        public BaseController Controller { get; }

        public FullRequestContext(BaseController controller)
        {
            Controller = controller;
            if (Controller.CurrentUser != null)
            {
                SetUser(Controller.CurrentUser);
            }
        }

        public void SetUser(UserInfo user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            CurrentUser = user;
        }

        public string Ip
        {
            get
            {
                var httpContext = Controller.HttpContext;
                return HttpContextHelper.GetClientIp(httpContext);
            }
        }

        public static FullRequestContext GetCurrent(HttpContext httpContext)
        {
            return httpContext.Items["FullContext"] as FullRequestContext;
        }

        public static void SetCurrent(HttpContext httpContext, FullRequestContext value)
        {
            httpContext.Items["FullContext"] = value;
        }

        public DispatcherClient GetDispatcherClient()
        {
            if (_dispatcherClient == null)
                _dispatcherClient = new DispatcherClient("UserAccount." + GetType().Name);
            return _dispatcherClient;
        }

        private DispatcherClient _dispatcherClient;
    }
}