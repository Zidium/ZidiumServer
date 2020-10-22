using System;
using System.Web;
using NLog;
using Zidium.Api;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;
using Zidium.Storage;
using Zidium.Storage.Ef;

namespace Zidium.ApiHttpService
{
    public class Global : HttpApplication
    {
        protected void InitMonitoring()
        {
            // TODO Remove database usage and dependency
            var client = SystemAccountHelper.GetInternalSystemClient();
            client.EventPreparer = new HttpServiceEventPreparer();

            // Создадим компонент
            // Если запускаемся в отладке, то компонент будет не в корне, а в папке DEBUG
            var debugConfiguration = DependencyInjection.GetServicePersistent<IDebugConfiguration>();
            var folder = !debugConfiguration.DebugMode ? client.GetRootComponentControl() : client.GetRootComponentControl().GetOrCreateChildFolderControl("DEBUG");
            var componentType = client.GetOrCreateComponentTypeControl(!debugConfiguration.DebugMode ? "ApiWebService" : DebugHelper.DebugComponentType);
            var componentControl = folder
                .GetOrCreateChildComponentControl(componentType, "ApiWebService 1.0", ApiHandler.GetVersion());

            // Присвоим Id компонента по умолчанию, чтобы адаптер NLog мог его использовать
            Client.Instance = client;
            Client.Instance.Config.DefaultComponent.Id = componentControl.Info?.Id;

            ApiHandler.Init(componentControl);

            LogManager.GetCurrentClassLogger().Info("Запуск, IsFake={0}", componentControl.IsFake());
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            // Приложение не должно накатывать миграции или создавать базы
            StorageFactory.DisableMigrations();

            var configuration = new Configuration();
            DependencyInjection.SetServicePersistent<IDebugConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IDatabaseConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IDispatcherConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IApiHttpServiceConfiguration>(configuration);

            Initialization.SetServices();
            DependencyInjection.SetServicePersistent<IStorageFactory>(new StorageFactory());

            if (DispatcherHelper.UseLocalDispatcher())
                DependencyInjection.SetServicePersistent<IAccountStorageFactory>(new LocalAccountStorageFactory());
            else
                DependencyInjection.SetServicePersistent<IAccountStorageFactory>(new RemoteAccountStorageFactory());

            InitMonitoring();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (sender is HttpApplication application)
            {
                application.Context.Response.Headers.Remove("Server");
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception != null)
            {
                Tools.HandleOutOfMemoryException(exception);
                LogManager.GetCurrentClassLogger().Error(exception);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Залогируем остановку
            LogManager.GetCurrentClassLogger().Info("Остановка");
            Client.Instance.Flush();
        }
    }
}