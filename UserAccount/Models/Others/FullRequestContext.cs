using System;
using System.Web;
using Zidium.Core.Api;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Controllers;

namespace Zidium.UserAccount.Models
{
    public class FullRequestContext
    {
        public UserInfo CurrentUser { get; protected set; }

        public AccountInfo CurrentAccount { get; private set; }

        public BaseController Controller { get; }

        private IEnumName _enumName;

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
            var accountId = user.AccountId;
            CurrentAccount = GetDispatcherClient().GetAccountById(new GetAccountByIdRequestData() { Id = accountId }).Data;
            user.AccountName = CurrentAccount.SystemName;
        }

        public string Ip
        {
            get
            {
                var httpContext = Controller.HttpContext.ApplicationInstance.Context;
                return HttpContextHelper.GetClientIp(httpContext);
            }
        }

        public static FullRequestContext Current
        {
            get
            {
                return HttpContext.Current.Items["FullContext"] as FullRequestContext;
            }
            set
            {
                HttpContext.Current.Items["FullContext"] = value;
            }
        }

        public IEnumName EnumName
        {
            get
            {
                if (_enumName == null)
                {
                    _enumName = EnumNameHelper.Get(Language.Russian);
                }
                return _enumName;
            }
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