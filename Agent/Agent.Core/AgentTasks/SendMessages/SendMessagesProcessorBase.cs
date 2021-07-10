using System;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Core;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.SendMessages
{
    public abstract class SendMessagesProcessorBase
    {
        public bool FakeMode { get; set; }

        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public int SuccessSendCount;

        public int SendMaxCount = 100;

        protected SendMessagesProcessorBase(
            ILogger logger,
            CancellationToken cancellationToken
        )
        {
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        protected abstract SubscriptionChannel Channel { get; }

        protected abstract void Send(string to, string body);

        public string Process(Guid? commandId = null)
        {
            var storage = DependencyInjection.GetServicePersistent<IDefaultStorageFactory>().GetStorage();
            var messageCommandRepository = storage.SendMessageCommands;
            var notificationRepository = storage.Notifications;

            var commands = messageCommandRepository.GetForSend(Channel, SendMaxCount);

            if (commandId.HasValue)
                commands = commands.Where(t => t.Id == commandId.Value).ToArray();

            var count = commands.Length;
            if (count > 0)
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug("Всего сообщений для отправки: " + count);
            }
            else
            {
                if (Logger.IsTraceEnabled)
                    Logger.Trace("Нет сообщений для отправки");
            }

            string error = null;

            foreach (var command in commands)
            {
                CancellationToken.ThrowIfCancellationRequested();

                if (Logger.IsDebugEnabled)
                    Logger.Debug("Отправляем сообщение получателю {0}", command.To);

                try
                {
                    // отправим сообщение
                    Send(command.To, command.Body);

                    // обновим статус  и статистику
                    messageCommandRepository.MarkAsSendSuccessed(command.Id, DateTime.Now);
                    Interlocked.Increment(ref SuccessSendCount);

                    // если есть соответствующее уведомление, поменяем его статус
                    if (command.ReferenceId != null)
                    {
                        var notification = notificationRepository.GetOneOrNullById(command.ReferenceId.Value);
                        if (notification != null)
                        {
                            var notificationForUpdate = notification.GetForUpdate();
                            notificationForUpdate.Status.Set(NotificationStatus.Sent);
                            notificationRepository.Update(notificationForUpdate);
                        }
                    }

                    Logger.Info("Отправлено сообщение получателю {0}", command.To);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (SendMessagesDisabledException exception)
                {
                    // Пользователь не разрешает отправку сообщений
                    // Статус поменяем, но ошибкой задачи не считаем

                    messageCommandRepository.MarkAsSendFail(command.Id, DateTime.Now, exception.Message);

                    // если есть соответствующее уведомление, поменяем его статус
                    if (command.ReferenceId != null)
                    {
                        var notification = notificationRepository.GetOneOrNullById(command.ReferenceId.Value);
                        if (notification != null)
                        {
                            var notificationForUpdate = notification.GetForUpdate();
                            notificationForUpdate.Status.Set(NotificationStatus.Error);
                            notificationForUpdate.SendError.Set(exception.Message);
                            notificationRepository.Update(notificationForUpdate);
                        }
                    }

                    Logger.Info("Получатель {0} не разрешал отправку сообщений", command.To);
                }
                catch (SendMessagesOverlimitException exception)
                {
                    // Особый случай - превышение лимита мессенджера
                    // За ошибку не считаем, в следующий раз отправится

                    Logger.Error(exception, exception.Message);

                    // Отправлять дальше нет смысла
                    break;
                }
                catch (Exception exception)
                {
                    error = exception.Message;

                    exception.Data.Add("CommandId", command.Id);
                    Logger.Error(exception);

                    messageCommandRepository.MarkAsSendFail(command.Id, DateTime.Now, exception.Message);

                    // если есть соответствующее уведомление, поменяем его статус
                    if (command.ReferenceId != null)
                    {
                        var notification = notificationRepository.GetOneOrNullById(command.ReferenceId.Value);
                        if (notification != null)
                        {
                            var notificationForUpdate = notification.GetForUpdate();
                            notificationForUpdate.Status.Set(NotificationStatus.Error);
                            notificationForUpdate.SendError.Set(exception.Message);
                            notificationRepository.Update(notificationForUpdate);
                        }
                    }

                }
            }

            return error;
        }

    }
}