using System;
using System.Threading;
using NLog;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;
using Zidium.Storage;

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
        protected abstract void Send(
            ILogger logger,
            NotificationForRead notification,
            IStorage storage,
            string accountName,
            Guid accountId,
            NotificationForUpdate notificationForUpdate);

        /// <summary>
        /// Отправка уведомлений из указанной StorageDb
        /// </summary>
        protected void ProcessAccount(ForEachAccountData data, Guid? componentId = null)
        {
            var notificationRepository = data.Storage.Notifications;

            var notifications = notificationRepository.GetForSend(Channels, componentId, MaxNotificationCount);

            if (notifications.Length > 0)
            {
                if (data.Logger.IsDebugEnabled)
                    data.Logger.Debug("Найдено уведомлений: " + notifications.Length);

                foreach (var notification in notifications)
                {
                    data.CancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        if (data.Logger.IsDebugEnabled)
                            data.Logger.Debug("Обработка уведомления " + notification.Id);

                        var notificationForUpdate = notification.GetForUpdate();

                        Send(data.Logger,
                            notification,
                            data.Storage,
                            data.Account.SystemName, data.Account.Id, notificationForUpdate);

                        notificationForUpdate.Status.Set(NotificationStatus.Processed);
                        notificationForUpdate.SendDate.Set(DateTime.Now);
                        notificationRepository.Update(notificationForUpdate);

                        data.Logger.Info("Уведомление " + notification.Id + " отправлено на " + notification.Address);
                        Interlocked.Increment(ref CreatedNotificationsCount);
                    }
                    catch (NotificationNonImportantException exception)
                    {
                        // ошибка при отправке, которая не считается важной
                        data.Logger.Info(exception);

                        // обновим статус
                        var notificationForUpdate = notification.GetForUpdate();
                        notificationForUpdate.SendError.Set((exception.InnerException ?? exception).ToString());
                        notificationForUpdate.Status.Set(NotificationStatus.Error);
                        notificationForUpdate.SendDate.Set(DateTime.Now);
                        notificationRepository.Update(notificationForUpdate);

                        // TODO отправить ошибку в компонент аккаунта, чтобы её увидел админ аккаунта (а не админ Зидиума)
                    }
                    catch (Exception exception)
                    {
                        // залогируем ошибку
                        DbProcessor.SetException(exception);
                        data.Logger.Error(exception);

                        // обновим статус
                        var notificationForUpdate = notification.GetForUpdate();
                        notificationForUpdate.SendError.Set(exception.ToString());
                        notificationForUpdate.Status.Set(NotificationStatus.Error);
                        notificationForUpdate.SendDate.Set(DateTime.Now);
                        notificationRepository.Update(notificationForUpdate);
                    }
                }
            }
            else
            {
                if (data.Logger.IsTraceEnabled)
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
