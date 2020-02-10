using System;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;

namespace Zidium.Agent.AgentTasks.Notifications
{
    /// <summary>
    /// Базовый класс для отправщиков уведомлений
    /// </summary>
    public abstract class NotificationSenderBase
    {
        protected ILogger Logger; 

        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public int MaxNotificationCount = 100;

        public int CreatedNotificationsCount;

        protected NotificationSenderBase(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
        }

        /// <summary>
        /// Переопределите этот метод для указания каналов
        /// </summary>
        protected abstract SubscriptionChannel[] Channels { get; }

        /// <summary>
        /// Переопределите этот метод для выполнения реальной отправки
        /// </summary>
        protected abstract void Send(ILogger logger,
            Notification notification,
            AccountDbContext accountDbContext,
            string accountName, Guid accountId);

        /// <summary>
        /// Отправка уведомлений из указанной StorageDb
        /// </summary>
        protected void ProcessAccount(ForEachAccountData data, Guid? componentId = null)
        {
            var notificationRepository = data.AccountDbContext.GetNotificationRepository();

            var notificationsQuery = notificationRepository.GetForSend(Channels);

            if (componentId.HasValue)
                notificationsQuery = notificationsQuery.Where(t => t.Event.OwnerId == componentId.Value);

            var notifications = notificationsQuery.OrderBy(t => t.CreationDate).Take(MaxNotificationCount).ToList();

            if (notifications.Count > 0)
            {
                data.Logger.Debug("Найдено уведомлений: " + notifications.Count);
                foreach (var notification in notifications)
                {
                    data.CancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        data.Logger.Debug("Обработка уведомления " + notification.Id);

                        Send(data.Logger,
                            notification,
                            data.AccountDbContext,
                            data.Account.SystemName, data.Account.Id);

                        notification.Status = NotificationStatus.Processed;
                        notification.SendDate = DateTime.Now;
                        data.AccountDbContext.SaveChanges();

                        data.Logger.Info("Уведомление " + notification.Id + " отправлено на " + notification.Address);
                        Interlocked.Increment(ref CreatedNotificationsCount);
                    }
                    catch (NotificationNonImportantException exception)
                    {
                        // ошибка при отправке, которая не считается важной
                        data.Logger.Info(exception);

                        // обновим статус
                        notification.SendError = (exception.InnerException ?? exception).ToString();
                        notification.Status = NotificationStatus.Error;
                        notification.SendDate = DateTime.Now;
                        data.AccountDbContext.SaveChanges();

                        // TODO отправить ошибку в компонент аккаунта, чтобы её увидел админ аккаунта (а не админ Зидиума)
                    }
                    catch (Exception exception)
                    {
                        // залогируем ошибку
                        DbProcessor.SetException(exception);
                        data.Logger.Error(exception);

                        // обновим статус
                        notification.SendError = exception.ToString();
                        notification.Status = NotificationStatus.Error;
                        notification.SendDate = DateTime.Now;
                        data.AccountDbContext.SaveChanges();
                    }
                }
            }
            else
            {
                data.Logger.Trace("Новых уведомлений нет");
            }
        }

        public void Process()
        {
            DbProcessor.ForEachAccount(data =>
            {
                if (data.Account.Type != AccountType.Test)
                    ProcessAccount(data);
            });
            if (CreatedNotificationsCount > 0)
                Logger.Info("Обработано уведомлений: " + CreatedNotificationsCount);

        }

        public void Process(Guid accountId, Guid componentId)
        {
            DbProcessor.ForEachAccount(data =>
            {
                if (data.Account.Id == accountId)
                {
                    ProcessAccount(data, componentId);
                }
            });
            if (CreatedNotificationsCount > 0)
                Logger.Info("Обработано уведомлений: " + CreatedNotificationsCount);

        }
    }
}
