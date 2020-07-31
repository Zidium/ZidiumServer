using System;
using System.Threading;
using NLog;
using Zidium.Core.ConfigDb;
using Zidium.Core.Api;

namespace Zidium.Core.Common
{
    /// <summary>
    /// Вспомогательный класс для обработки множеста баз данных
    /// </summary>
    public class MultipleDataBaseProcessor
    {
        /// <summary>
        /// Количество потоков для обработки всех аккаунтов
        /// </summary>
        public int AccountThreads { get; set; }

        /// <summary>
        /// Количество потоков для обработки БД 
        /// </summary>
        public int DataBaseThreads { get; set; }

        /// <summary>
        /// Количество потоков для обработки компонентов
        /// </summary>
        public int ComponentsThreads { get; set; }

        public Exception FirstException { get; protected set; }

        public int ErrorCount { get; protected set; }

        public void SetException(Exception exception)
        {
            lock (this)
            {
                ErrorCount++;
                if (FirstException == null)
                {
                    FirstException = exception;
                }
            }
        }

        public CancellationToken CancellationToken { get; protected set; }

        public ILogger Logger { get; protected set; }

        public MultipleDataBaseProcessor(ILogger logger = null, CancellationToken cancellationToken = new CancellationToken())
        {
            // значения по умолчанию
            Logger = logger ?? LogManager.GetCurrentClassLogger();
            AccountThreads = 4;
            DataBaseThreads = 4;
            ComponentsThreads = 4;
            CancellationToken = cancellationToken;
        }

        public void ForAccount(Guid accountId, Action<ForEachAccountData> method)
        {
            ForEachAccount(x =>
            {
                if (x.Account.Id == accountId)
                {
                    method(x);
                }
            });
        }

        /// <summary>
        /// Организует цикл по аккаунтам
        /// </summary>
        /// <param name="method"></param>
        public void ForEachAccount(Action<ForEachAccountData> method)
        {
            try
            {
                if (Logger.IsTraceEnabled)
                    Logger.Trace("Начинаем обработку всех аккаунтов");

                var client = DispatcherHelper.GetDispatcherClient();
                var accounts = client.GetAccounts(new GetAccountsRequestData() { Status = AccountStatus.Active }).Data;

                if (Logger.IsTraceEnabled)
                    Logger.Trace("Найдено аккаунтов: " + accounts.Length);

                var tasks = new ThreadTaskQueue(AccountThreads);
                tasks.UseSystemThreadPool = true;

                tasks.ForEach(accounts, account =>
                {
                    CancellationToken.ThrowIfCancellationRequested();
                    ForEachAccountWrapper(account, method);
                });
            }
            catch (ThreadAbortException) { }
            catch (OperationCanceledException) { }
            catch (Exception exception)
            {
                SetException(exception);
                Zidium.Api.Tools.HandleOutOfMemoryException(exception);
                Logger.Error(exception);
            }
            finally
            {
                if (Logger.IsTraceEnabled)
                    Logger.Trace("Обработка всех аккаунтов завершена");
            }
        }

        protected void ForEachAccountWrapper(AccountInfo account, Action<ForEachAccountData> method)
        {
            var accountLogger = LogManager.GetLogger(Logger.Name + "." + account.SystemName);
            try
            {
                var data = new ForEachAccountData(accountLogger, CancellationToken, account);

                if (Logger.IsTraceEnabled)
                    data.Logger.Trace("Начинаем обработку аккаунта; ID:{0}; Name:{1}", account.Id, account.DisplayName);

                try
                {
                    method(data);
                }
                finally
                {
                    if (Logger.IsTraceEnabled)
                        data.Logger.Trace("Обработка аккаунта завершена; ID:{0}; Name:{1}", account.Id, account.DisplayName);
                }
            }
            catch (ThreadAbortException) { }
            catch (OperationCanceledException) { }
            catch (Exception exception)
            {
                SetException(exception);
                Zidium.Api.Tools.HandleOutOfMemoryException(exception);
                accountLogger.Error(exception);
            }
        }

    }
}
