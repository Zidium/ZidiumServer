using System;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;

namespace Zidium.Agent.AgentTasks.SendMessages
{
    public abstract class SendMessagesProcessorBase
    {
        public bool FakeMode { get; set; }

        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public int SuccessSendCount;

        public int SendMaxCount = 100;

        protected SendMessagesProcessorBase(
            ILogger logger,
            CancellationToken cancellationToken
        )
        {
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
        }

        protected abstract SubscriptionChannel Channel { get; }

        protected abstract void Send(string to, string body, ForEachAccountData data);

        public void Process(Guid? accountId = null, Guid? commandId = null)
        {
            DbProcessor.ForEachAccount(data =>
            {
                if (accountId == null && data.Account.Type != AccountType.Test)
                    ProcessAccount(data);
                else if (accountId != null && accountId.Value == data.Account.Id)
                    ProcessAccount(data, commandId);
            });
        }

        protected void ProcessAccount(ForEachAccountData data, Guid? commandId = null)
        {
            var messageCommandRepository = data.AccountDbContext.GetSendMessageCommandRepository();
            var notificationRepository = data.AccountDbContext.GetNotificationRepository();

            var commands = messageCommandRepository
                .GetForSend(Channel, SendMaxCount)
                .ToArray();

            if (commandId.HasValue)
                commands = commands.Where(t => t.Id == commandId.Value).ToArray();

            var count = commands.Length;
            if (count > 0)
                data.Logger.Debug("Всего сообщений для отправки: " + count);
            else
                data.Logger.Trace("Нет сообщений для отправки");

            foreach (var command in commands)
            {
                data.CancellationToken.ThrowIfCancellationRequested();
                data.Logger.Debug("Отправляем сообщение получателю {0}", command.To);
                try
                {
                    // отправим сообщение
                    Send(command.To, command.Body, data);

                    // обновим статус  и статистику
                    messageCommandRepository.MarkAsSendSuccessed(command.Id);
                    Interlocked.Increment(ref SuccessSendCount);

                    // если есть соответствующее уведомление, поменяем его статус
                    if (command.ReferenceId != null)
                    {
                        var notification = notificationRepository.GetOneOrNullById(command.ReferenceId.Value);
                        if (notification != null)
                        {
                            notification.Status = NotificationStatus.Sent;
                            data.AccountDbContext.SaveChanges();
                        }
                    }

                    data.Logger.Info("Отправлено сообщение получателю {0}", command.To);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (SendMessagesDisabledException exception)
                {
                    // Пользователь не разрешает отправку сообщений
                    // Статус поменяем, но ошибкой задачи не считаем

                    messageCommandRepository.MarkAsSendFail(command.Id, exception.Message);

                    // если есть соответствующее уведомление, поменяем его статус
                    if (command.ReferenceId != null)
                    {
                        var notification = notificationRepository.GetOneOrNullById(command.ReferenceId.Value);
                        if (notification != null)
                        {
                            notification.Status = NotificationStatus.Error;
                            notification.SendError = exception.Message;
                            data.AccountDbContext.SaveChanges();
                        }
                    }

                    data.Logger.Info("Получатель {0} не разрешал отправку сообщений", command.To);
                }
                catch (SendMessagesOverlimitException exception)
                {
                    // Особый случай - превышение лимита мессенджера
                    // За ошибку не считаем, в следующий раз отправится

                    data.Logger.Error(exception, exception.Message);

                    // Отправлять дальше нет смысла
                    break;
                }
                catch (Exception exception)
                {
                    DbProcessor.SetException(exception);

                    exception.Data.Add("CommandId", command.Id);
                    data.Logger.Error(exception);

                    messageCommandRepository.MarkAsSendFail(command.Id, exception.Message);

                    // если есть соответствующее уведомление, поменяем его статус
                    if (command.ReferenceId != null)
                    {
                        var notification = notificationRepository.GetOneOrNullById(command.ReferenceId.Value);
                        if (notification != null)
                        {
                            notification.Status = NotificationStatus.Error;
                            notification.SendError = exception.Message;
                            data.AccountDbContext.SaveChanges();
                        }
                    }

                }
            }

        }

    }
}