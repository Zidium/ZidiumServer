using System;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Api;
using Zidium.Core.ConfigDb;
using Zidium.Core.AccountsDb;
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
        /// Количество потоков для обработки всех аккаунтов
        /// </summary>
        public int StorageThreads { get; set; }

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
            StorageThreads = 4;
            DataBaseThreads = 4;
            ComponentsThreads = 4;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Все базы данных, которые находятся в рабочем состоянии
        /// </summary>
        /// <returns></returns>
        protected DatabaseInfo[] GetAllNotBroken()
        {
            var client = DispatcherHelper.GetDispatcherClient();
            var databases = client.GetDatabases().Data;

            Logger.Trace("Всего БД: " + databases.Length);

            foreach (var database in databases)
            {
                CancellationToken.ThrowIfCancellationRequested();
                if (database.IsBroken)
                {
                    Logger.Warn(
                        "БД недоступна; ID:{0}; SystemName:{1}",
                        database.Id,
                        database.SystemName);
                }
                else if (IsDataBaseBroken(database))
                {
                    Logger.Warn(
                        "БД сейчас недоступна; ID:{0}; SystemName:{1}",
                        database.Id,
                        database.SystemName);
                }
                else
                {
                    Logger.Trace(
                       "БД доступна; ID:{0}; SystemName:{1}",
                       database.Id,
                       database.SystemName);
                }
            }

            // вернем только рабочие бд
            databases = databases.Where(x => x.IsBroken == false).ToArray();
            return databases;
        }

        public static bool IsDataBaseBroken(DatabaseInfo database)
        {
            try
            {
                if (database.IsBroken)
                {
                    return true;
                }
                database.ValidateBroken();
                return false;
            }
            catch (Exception)
            {
                var client = DispatcherHelper.GetDispatcherClient();
                client.SetDatabaseIsBroken(new SetDatabaseIsBrokenRequestData()
                {
                    Id = database.Id,
                    IsBroken = true
                });

                return true;
            }
        }

        #region Циклы по аккаунтам

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
                Logger.Trace("Начинаем обработку всех аккаунтов");
                var client = DispatcherHelper.GetDispatcherClient();
                var accounts = client.GetAccounts(new GetAccountsRequestData() { Status = AccountStatus.Active }).Data;

                Logger.Trace("Найдено аккаунтов: " + accounts.Length);
                var tasks = new ThreadTaskQueue(AccountThreads);
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
                Tools.HandleOutOfMemoryException(exception);
                Logger.Error(exception);
            }
            finally
            {
                Logger.Trace("Обработка всех аккаунтов завершена");
            }
        }

        protected void ForEachAccountWrapper(AccountInfo account, Action<ForEachAccountData> method)
        {
            try
            {
                CancellationToken.ThrowIfCancellationRequested();
                Logger.Trace("Начинаем обработку аккаунта; ID:{0}; Name:{1}", account.Id, account.DisplayName);
                var logger = LogManager.GetLogger(Logger.Name + "." + account.SystemName);
                using (var data = new ForEachAccountData(logger, CancellationToken, account))
                {
                    method(data);
                }
            }
            catch (ThreadAbortException) { }
            catch (OperationCanceledException) { }
            catch (Exception exception)
            {
                SetException(exception);
                Tools.HandleOutOfMemoryException(exception);
                Logger.Error(exception);
            }
            finally
            {
                Logger.Trace("Обработка аккаунта завершена; ID:{0}; Name:{1}", account.Id, account.DisplayName);
            }
        }

        #endregion

        #region Цикл по компонентам

        protected void ForEachAccountComponentsWrapper(
            AccountInfo account,
            Component component,
            Action<ForEachComponentData> method)
        {
            try
            {
                Logger.Trace(
                "Начинаем обработку компонента; ID:{0} SystemName:{1}",
                component.Id,
                component.SystemName);

                var logger = LogManager.GetLogger(Logger.Name + "." + component.SystemName);
                using (var data = new ForEachComponentData(
                    logger,
                    CancellationToken,
                    account,
                    component))
                {
                    method(data);
                }

            }
            catch (ThreadAbortException) { }
            catch (OperationCanceledException) { }
            catch (Exception exception)
            {
                SetException(exception);
                Tools.HandleOutOfMemoryException(exception);
                Logger.Error(exception);
            }
            finally
            {
                Logger.Trace("Обработка компонента завершена; ID:{0} SystemName:{1}",
                    component.Id,
                    component.SystemName);
            }
        }

        public void ForEachAccountComponents(Guid accountId, Action<ForEachComponentData> method, bool withDeleted = false)
        {
            var client = DispatcherHelper.GetDispatcherClient();
            var account = client.GetAccountById(new GetAccountByIdRequestData() { Id = accountId }).Data;
            ForEachAccountComponents(account, method, withDeleted);
        }

        public void ForComponent(Guid accountId, Guid componentId, Action<ForEachComponentData> method)
        {
            ForEachAccount(x =>
            {
                if (x.Account.Id == accountId)
                {
                    ForEachAccountComponents(x.Account, y =>
                    {
                        if (y.Component.Id == componentId)
                        {
                            method(y);
                        }
                    });
                }
            });
        }

        public void ForEachAccountComponents(AccountInfo account, Action<ForEachComponentData> method, bool withDeleted = false)
        {
            try
            {
                Logger.Trace("Начинаем обработку компонентов аккаунта; accountId:{0} accountName:{1}", account.Id, account.DisplayName);
                CancellationToken.ThrowIfCancellationRequested();
                using (var accountDbContext = AccountDbContext.CreateFromDatabaseId(account.AccountDatabaseId))
                {
                    var componentRepository = accountDbContext.GetComponentRepository();
                    var components = !withDeleted ?
                        componentRepository.QueryAll().ToArray() :
                        componentRepository.QueryAllWithDeleted().ToArray();
                    Logger.Trace("Найдено компонентов: " + components.Length);
                    var tasks = new ThreadTaskQueue(ComponentsThreads);
                    tasks.ForEach(components, component =>
                    {
                        CancellationToken.ThrowIfCancellationRequested();
                        ForEachAccountComponentsWrapper(account, component, method);
                    });
                }
            }
            catch (ThreadAbortException) { }
            catch (OperationCanceledException) { }
            catch (Exception exception)
            {
                SetException(exception);
                Tools.HandleOutOfMemoryException(exception);
                Logger.Error(exception);
            }
            finally
            {
                Logger.Trace("Обработка аккаунта завершена; accountId:{0} accountName:{1}", account.Id, account.DisplayName);
            }
        }

        public void ForEachComponent(Action<ForEachComponentData> method, bool withDeleted = false)
        {
            Logger.Trace("Начинаем обработку всех компонентов во всех базах");
            ForEachAccount(data => ForEachAccountComponents(data.Account, method, withDeleted));
            Logger.Trace("Обработка всех компонентов во всех базах завершена");
        }

        #endregion
    }
}
