using System;
using System.Threading;
using NLog;
using Zidium.Core;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.Notifications
{
    /// <summary>
    /// Базовый класс для отправщиков уведомлений
    /// </summary>
    public abstract class NotificationSenderBase
    {
        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public int MaxNotificationCount = 100;

        public int CreatedNotificationsCount;

        protected NotificationSenderBase(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Переопределите этот метод для указания каналов
        /// </summary>
        protected abstract SubscriptionChannel[] Channels { get; }

        /// <summary>
        /// Переопределите этот метод для выполнения реальной отправки
        /// </summary>
        protected abstract void Send(
            NotificationForRead notification,
            IStorage storage,
            NotificationForUpdate notificationForUpdate);

        public string Process(Guid? componentId = null)
        {
            string error = null;

            var storage = DependencyInjection.GetServicePersistent<IDefaultStorageFactory>().GetStorage();
            var notificationRepository = storage.Notifications;

            var notifications = notificationRepository.GetForSend(Channels, componentId, MaxNotificationCount);

            if (notifications.Length > 0)
            {
                // TODO Нужно ли проверять IsDebugEnabled?
                if (Logger.IsDebugEnabled)
                    Logger.Debug("Найдено уведомлений: " + notifications.Length);

                foreach (var notification in notifications)
                {
                    CancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        if (Logger.IsDebugEnabled)
                            Logger.Debug("Обработка уведомления " + notification.Id);

                        var notificationForUpdate = notification.GetForUpdate();

                        Send(notification,
                            storage,
                            notificationForUpdate);

                        notificationForUpdate.Status.Set(NotificationStatus.Processed);
                        notificationForUpdate.SendDate.Set(DateTime.Now);
                        notificationRepository.Update(notificationForUpdate);

                        Logger.Info("Уведомление " + notification.Id + " отправлено на " + notification.Address);
                        Interlocked.Increment(ref CreatedNotificationsCount);
                    }
                    catch (NotificationNonImportantException exception)
                    {
                        // ошибка при отправке, которая не считается важной
                        Logger.Info(exception);

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
                        error = exception.Message;
                        Logger.Error(exception);

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
                if (Logger.IsTraceEnabled)
                    Logger.Trace("Новых уведомлений нет");
            }

            if (CreatedNotificationsCount > 0)
                Logger.Info("Обработано уведомлений: " + CreatedNotificationsCount);

            return error;
        }

    }
}
