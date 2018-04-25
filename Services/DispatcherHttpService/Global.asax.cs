using System;
using System.Data.Entity;
using System.Threading;
using System.Web;
using NLog;
using Zidium.Api;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;
using Zidium.Core.Limits;

namespace Zidium.DispatcherHttpService
{
    public class Global : HttpApplication
    {
        protected void InitComponentControl()
        {
            DispatcherService.Version = DispatcherHandler.GetVersion();
            var componentControl = DispatcherService.Wrapper.Control;

            LogManager.GetCurrentClassLogger().Info("Запуск, IsFake={0}", componentControl.IsFake());
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            // Приложение не должно накатывать миграции или создавать базы
            AccountDbContext.DisableMigrations();

            Initialization.SetServices();
            InitComponentControl();

            try
            {
                DispatcherHandler.Init(DispatcherService.Wrapper.Control);
                InitLimitsSaving();
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Fatal(exception);
            }
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

            // Остановим обработку запросов
            WebHandlerBase.Stop();

            // Сохраним кеш
            DispatcherService.Wrapper.SaveCaches();
        }

        protected void InitLimitsSaving()
        {
            var control = DispatcherService.Wrapper.Control;
            if (control != null)
            {
                UnitTestControl = control.GetOrCreateUnitTestControl("Dispatcher.LimitsSaving");
            }
            DoSaveLimits(null);
        }

        protected Timer LimitsSavingTimer;

        protected IUnitTestControl UnitTestControl;

        protected void DoSaveLimits(object obj)
        {
            var startTime = DateTime.Now;
            var count = AccountLimitsCheckerManager.Save();
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            var control = DispatcherService.Wrapper.Control;
            if (AccountLimitsCheckerManager.LastSaveException != null)
            {
                if (control != null)
                {
                    control.SendApplicationError(AccountLimitsCheckerManager.LastSaveException);
                }
            }

            if (UnitTestControl != null)
            {
                if (AccountLimitsCheckerManager.LastSaveException == null)
                {
                    UnitTestControl.SendResult(UnitTestResult.Success, TimeSpan.FromMinutes(10), "Сохранено записей: " + count + " за " + duration);
                }
                else
                {
                    UnitTestControl.SendResult(UnitTestResult.Alarm, TimeSpan.FromMinutes(10), AccountLimitsCheckerManager.LastSaveException.Message);
                }
            }

            if (LimitsSavingTimer != null)
            {
                LimitsSavingTimer.Dispose();
            }
            LimitsSavingTimer = new Timer(DoSaveLimits, null, AccountLimitsChecker.LimitDataTimeStep * 60 * 1000, Timeout.Infinite);
        }
    }
}