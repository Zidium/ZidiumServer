using System;
using System.Web;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Controllers;

namespace Zidium.UserAccount.Models
{
    public class FullRequestContext
    {
        public DatabasesContext DbContext { get; }

        public UserInfo CurrentUser { get; protected set; }

        public AccountInfo CurrentAccount { get; private set; }

        public AccountDbContext AccountDbContext { get; private set; }

        public ContextController Controller { get; }

        private IEnumName _enumName;

        public FullRequestContext(ContextController controller)
        {
            DbContext = new DatabasesContext();
            Controller = controller;
            if (Controller.CurrentUser != null)
            {
                SetUser(Controller.CurrentUser);
            }
        }
        
        public User GetUserById(Guid id)
        {
            // todo нужно убрать этот метод от сюда
            // для ЛК нужны свои репозитории
            var accountId = CurrentUser.AccountId;
            var repository = AccountDbContext.GetUserRepository();
            return repository.GetById(id);
        }

        public ComponentType GetComponentTypeById(Guid id)
        {
            // todo нужно убрать этот метод от сюда
            // для ЛК нужны свои репозитории
            var accountId = CurrentUser.AccountId;
            var repository = AccountDbContext.GetComponentTypeRepository();
            return repository.GetById(id);
        }

        public Component GetComponentById(Guid id)
        {
            // todo нужно убрать этот метод от сюда
            // для ЛК нужны свои репозитории
            var accountId = CurrentUser.AccountId;
            var repository = AccountDbContext.GetComponentRepository();
            return repository.GetById(id);
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
            AccountDbContext = DbContext.GetAccountDbContextByDataBaseId(CurrentAccount.AccountDatabaseId);
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