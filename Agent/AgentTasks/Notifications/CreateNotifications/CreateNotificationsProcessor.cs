using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Api;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using EventCategory = Zidium.Core.Api.EventCategory;
using EventImportance = Zidium.Core.Api.EventImportance;

namespace Zidium.Agent.AgentTasks.Notifications
{
    public class CreateNotificationsProcessor
    {
        public class StatusInfo
        {
            public Guid EventId;

            public Component Component;

            public DateTime CreateDate;

            public Guid ComponentId;

            public EventCategory Category;

            public TimeSpan Duration;

            public EventImportance Importance;

            public EventImportance PreviousImportance;

            public static StatusInfo Create(Event eventObj, Component component, DateTime now)
            {
                if (eventObj == null)
                {
                    throw new ArgumentNullException("eventObj");
                }
                if (component == null)
                {
                    throw new ArgumentNullException("component");
                }
                return new StatusInfo()
                {
                    Category = eventObj.Category,
                    EventId = eventObj.Id,
                    ComponentId = eventObj.OwnerId,
                    Component = component,
                    Duration = eventObj.GetDuration(now),
                    Importance = eventObj.Importance,
                    CreateDate = eventObj.CreateDate,
                    PreviousImportance = eventObj.PreviousImportance
                };
            }

            public static StatusInfo Create(Bulb bulb, Component component, DateTime now)
            {
                if (bulb == null)
                {
                    throw new ArgumentNullException("bulb");
                }
                if (component == null)
                {
                    throw new ArgumentNullException("component");
                }
                return new StatusInfo()
                {
                    Category = bulb.EventCategory,
                    EventId = bulb.StatusEventId,
                    ComponentId = bulb.ComponentId.Value,
                    Component = component,
                    Duration = bulb.GetDuration(now),
                    Importance = EventImportanceHelper.Get(bulb.Status),
                    CreateDate = bulb.StartDate,
                    PreviousImportance = EventImportanceHelper.Get(bulb.PreviousStatus)
                };
            }
        }

        protected ILogger Logger;

        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public int CreatedNotificationsCount;

        /// <summary>
        /// Максимальное количество событий, которое будет обработано у одного хранилища за одну итерацию
        /// </summary>
        public int EventMaxCount = 1000;

        public CreateNotificationsProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
        }

        public void ProcessAll()
        {
            DbProcessor.ForEachAccount(
                data => ProcessAccount(
                    data.Account.Id,
                    data.AccountDbContext,
                    data.Logger));

            if (CreatedNotificationsCount > 0)
                Logger.Info("Создано уведомлений: {0}", CreatedNotificationsCount);
        }

        public void ProcessAccount(Guid accountId, Guid? componentId = null, Guid? userId = null)
        {
            using (var dbContext = new DatabasesContext())
            {
                var accountDbContext = dbContext.GetAccountDbContext(accountId);
                ProcessAccount(accountId, accountDbContext, Logger, componentId, userId);
            }
        }

        protected void ProcessAccount(
            Guid accountId,
            AccountDbContext accountDbContext,
            ILogger logger,
            Guid? componentId = null,
            Guid? userId = null)
        {
            // Получим все включенные подписки аккаунта
            var subscriptionsRepository = accountDbContext.GetSubscriptionRepository();

            var subscriptions = subscriptionsRepository.QueryAll();
            var userToSubscriptions = subscriptions.GroupBy(x => x.UserId).ToDictionary(x => x.Key, y => y.ToArray());

            // получим всех пользователей
            var users = accountDbContext.GetUserRepository().QueryAll().ToArray();

            if (userId.HasValue)
                users = users.Where(t => t.Id == userId.Value).ToArray();

            //получим все компоненты, т.к. для каждого нужно проверить наличие уведомлений по его текущему статусу
            var componentRepository = accountDbContext.GetComponentRepository();
            var components = componentRepository.QueryAll().ToDictionary(a => a.Id, b => b);

            // обработка старых статусов
            ProcessAccountArchivedStatuses(
                accountId,
                accountDbContext,
                users,
                components,
                userToSubscriptions,
                logger,
                componentId);

            // обработка текущих статусов
            ProcessAccountCurrentStatuses(
                accountId,
                accountDbContext,
                users,
                userToSubscriptions,
                logger,
                componentId);

            // сохраняем изменения
            accountDbContext.SaveChanges();
        }

        protected void ProcessAccountArchivedStatuses(
            Guid accountId,
            AccountDbContext accountDbContext,
            User[] users,
            Dictionary<Guid, Component> components,
            Dictionary<Guid, Subscription[]> userToSubscriptions,
            ILogger logger,
            Guid? componentId = null)
        {
            // получим архивные статусы
            var archivedStatusRepository = accountDbContext.GetArchivedStatusRepository();

            var archivedStatuses = archivedStatusRepository
                .GetTop(1000)
                .Include(x => x.Event)
                .ToArray();

            if (componentId.HasValue)
                archivedStatuses = archivedStatuses.Where(t => t.Event.OwnerId == componentId.Value).ToArray();

            var now = Now();
            var statuses = new List<StatusInfo>(archivedStatuses.Length);
            foreach (var archivedStatus in archivedStatuses)
            {
                DbProcessor.CancellationToken.ThrowIfCancellationRequested();

                var eventObj = archivedStatus.Event;
                components.TryGetValue(eventObj.OwnerId, out var component);

                if (component == null)
                {
                    continue;
                }

                // удалим, чтобы больше не обрабатывать
                archivedStatusRepository.Delete(archivedStatus);

                var status = StatusInfo.Create(eventObj, component, now);
                statuses.Add(status);
            }

            ProcessAccountInternal(
                accountId,
                users,
                userToSubscriptions,
                statuses,
                accountDbContext,
                logger);
        }

        protected void ProcessAccountCurrentStatuses(
            Guid accountId,
            AccountDbContext accountDbContext,
            User[] users,
            Dictionary<Guid, Subscription[]> userToSubscriptions,
            ILogger logger,
            Guid? componentId = null)
        {
            // получим колбаски компонентов
            var statusDatas = accountDbContext.Bulbs
                .Where(x => x.EventCategory == EventCategory.ComponentExternalStatus
                            && x.IsDeleted == false
                            && x.Component.IsDeleted == false)
                .ToArray();

            if (componentId.HasValue)
                statusDatas = statusDatas.Where(t => t.ComponentId == componentId.Value).ToArray();

            var now = Now();
            var statuses = new List<StatusInfo>(statusDatas.Length);
            foreach (var statusData in statusDatas)
            {
                DbProcessor.CancellationToken.ThrowIfCancellationRequested();

                var component = statusData.Component;
                if (component == null)
                {
                    continue;
                }
                var status = StatusInfo.Create(statusData, component, now);
                statuses.Add(status);
            }

            ProcessAccountInternal(
                accountId,
                users,
                userToSubscriptions,
                statuses,
                accountDbContext,
                logger);
        }

        protected void ProcessAccountInternal(
            Guid accountId,
            User[] users,
            Dictionary<Guid, Subscription[]> userToSubscriptions,
            List<StatusInfo> statuses,
            AccountDbContext accountDbContext,
            ILogger logger)
        {
            var cancellationToken = DbProcessor.CancellationToken;

            // Обработаем каждый статус
            foreach (var status in statuses)
            {
                logger.Debug("Обрабатываем событие: " + status.EventId);

                cancellationToken.ThrowIfCancellationRequested();

                // Получим компонент
                var component = status.Component;
                if (component == null)
                {
                    logger.Trace("component == null");
                    continue;
                }

                // статус о выключенном компоненте мы должны получить, поэтому фильтр ниже закоментирован

                //if (component.Enable == false)
                //{
                //    if (isTraceEnabled)
                //    {
                //        log.Trace("component.Enable == false");
                //    }
                //    continue;
                //}

                // Пропускаем удалённые
                if (component.IsDeleted)
                {
                    logger.Trace("component.IsDeleted");
                    continue;
                }

                // Пропускаем удалённые
                if (component.IsRoot)
                {
                    logger.Trace("component.IsRoot");
                    continue;
                }

                // цикл по пользователям
                foreach (var user in users)
                {
                    var channels = new[]
                    {
                        SubscriptionChannel.Email,
                        SubscriptionChannel.Sms,
                        SubscriptionChannel.Http
                    };
                    if (userToSubscriptions.TryGetValue(user.Id, out var subscriptions))
                    {
                        foreach (var channel in channels)
                        {
                            var channelSubscriptions = subscriptions
                                .Where(x => x.Channel == channel)
                                .ToArray();

                            // ищем подписку на компонент
                            var subscription = channelSubscriptions.FirstOrDefault(x => x.ComponentId == status.ComponentId);
                            if (subscription == null)
                            {
                                // ищем подписку на тип события
                                subscription = channelSubscriptions.FirstOrDefault(x => x.ComponentTypeId == status.Component.ComponentTypeId);
                                if (subscription == null)
                                {
                                    // ищем дефолтную подписку
                                    subscription = channelSubscriptions.FirstOrDefault(x => x.Object == SubscriptionObject.Default);

                                    // если дефолтной для email канала нет, то создаем её
                                    if (subscription == null && channel == SubscriptionChannel.Email)
                                    {
                                        var dispatcherClient = AgentHelper.GetDispatcherClient();

                                        var response = dispatcherClient.CreateUserDefaultSubscription(
                                            accountId,
                                            new CreateUserDefaultSubscriptionRequestData()
                                            {
                                                Channel = channel,
                                                UserId = user.Id
                                            });

                                        var repository = accountDbContext.GetSubscriptionRepository();
                                        subscription = repository.GetById(response.Data.Id);
                                    }
                                }
                            }

                            // проверим подписку
                            if (subscription != null)
                            {
                                logger.Trace("Проверяем подписку " + subscription.Id + ", событие " + status.EventId + " и компонент " + component.Id);

                                // если отправка запрещена
                                if (subscription.IsEnabled == false)
                                {
                                    logger.Trace("False by IsEnabled");
                                    continue;
                                }

                                // подписка шлет уведомления только по событиям, которые создались после создания подписки
                                if (status.CreateDate < subscription.LastUpdated)
                                {
                                    logger.Trace("False by LastUpdated");
                                    continue;
                                }

                                if (status.Category != EventCategory.ComponentExternalStatus)
                                {
                                    logger.Trace("False by Category");
                                    continue;
                                }

                                // минимальную длительность проверяем только у важных событий (цвет важности указывается пользователем в подписке)
                                if (subscription.DurationMinimumInSeconds.HasValue && status.Importance >= subscription.Importance)
                                {
                                    var eventDuration = (int)status.Duration.TotalSeconds;
                                    if (eventDuration < subscription.DurationMinimumInSeconds.Value)
                                    {
                                        logger.Trace("False by DurationMinimumInSeconds");
                                        continue;
                                    }
                                }

                                logger.Debug("Создаем уведомления для подписки: " + subscription.Id);
                                cancellationToken.ThrowIfCancellationRequested();

                                List<UserContact> contacts;

                                if (subscription.Channel == SubscriptionChannel.Email)
                                    contacts = GetUserEMailContacts(subscription.User);
                                else if (subscription.Channel == SubscriptionChannel.Sms)
                                    contacts = GetUserMobileContacts(subscription.User);
                                else if (subscription.Channel == SubscriptionChannel.Http)
                                    contacts = GetUserHttpContacts(subscription.User);
                                else
                                    contacts = new List<UserContact>();

                                logger.Debug("Адресов для уведомлений " + contacts.Count);

                                foreach (var contact in contacts)
                                {
                                    // Уведомления создаются только по контактам, добавленным до наступления события
                                    if (contact.CreateDate > status.CreateDate)
                                        continue;

                                    var address = contact.Value;
                                    logger.Debug("Создаём уведомление на адрес: " + address);

                                    NotificationType notificationType;
                                    if (contact.Type == UserContactType.Email)
                                    {
                                        notificationType = NotificationType.Email;
                                    }
                                    else if (contact.Type == UserContactType.MobilePhone)
                                    {
                                        notificationType = NotificationType.Sms;
                                    }
                                    else if (contact.Type == UserContactType.Http)
                                    {
                                        notificationType = NotificationType.Http;
                                    }
                                    else
                                    {
                                        logger.Debug("Неизвестный тип контакта: " + contact.Type);
                                        continue;
                                    }

                                    var lastComponentNotification = component.LastNotifications.FirstOrDefault(
                                        x => x.Address == address && x.Type == notificationType);

                                    var isImportanceColor = status.Importance >= subscription.Importance;

                                    // важный статус
                                    if (isImportanceColor)
                                    {
                                        // первое уведомление о важном статусе
                                        if (lastComponentNotification == null)
                                        {
                                            logger.Info("Первое уведомление на адрес " + address + " для компонента " + component.Id);
                                            lastComponentNotification = new LastComponentNotification()
                                            {
                                                Id = Guid.NewGuid(),
                                                Address = address,
                                                Component = component,
                                                ComponentId = component.Id,
                                                CreateDate = Now(),
                                                EventId = status.EventId,
                                                EventImportance = status.Importance,
                                                Type = notificationType,
                                                NotificationId = Guid.Empty
                                            };
                                            component.LastNotifications.Add(lastComponentNotification);

                                            AddNotification(
                                                lastComponentNotification,
                                                status,
                                                subscription,
                                                accountDbContext,
                                                NotificationReason.NewImportanceStatus);

                                            continue;

                                        }

                                        // тот же важный статус
                                        if (lastComponentNotification.EventId == status.EventId)
                                        {
                                            if (subscription.ResendTimeInSeconds > 0)
                                            {
                                                // Проверим, нужно ли отправлять повторное уведомление
                                                var notifyDuration = (int)(Now() - lastComponentNotification.CreateDate).TotalSeconds;

                                                if (notifyDuration >= subscription.ResendTimeInSeconds)
                                                {
                                                    logger.Info("Напоминание о статусе " + status.EventId + " на адрес " + address);

                                                    AddNotification(
                                                        lastComponentNotification,
                                                        status,
                                                        subscription,
                                                        accountDbContext,
                                                        NotificationReason.Reminder);

                                                    continue;
                                                }
                                            }
                                            logger.Debug("Уже есть уведомление о событии " + status.EventId + " на адрес " + address);
                                        }
                                        else // новый важный статус
                                        {
                                            logger.Info("Новый важный статус " + status.EventId + " на адрес " + address);

                                            AddNotification(
                                                lastComponentNotification,
                                                status,
                                                subscription,
                                                accountDbContext,
                                                NotificationReason.NewImportanceStatus);

                                        }
                                    }
                                    else // НЕ важное событие
                                    {
                                        if (lastComponentNotification != null
                                            && subscription.NotifyBetterStatus
                                            && lastComponentNotification.EventImportance >= subscription.Importance)
                                        {
                                            logger.Info("Уведомление о том, что стало лучше " + status.EventId + " на адрес " + address);

                                            AddNotification(
                                                lastComponentNotification,
                                                status,
                                                subscription,
                                                accountDbContext,
                                                NotificationReason.BetterStatus);
                                        }
                                        else
                                        {
                                            //log.Debug("Не отправляем уведомление об улучшении "+ status.EventId + " на адрес " + address);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // у пользователя нет ни одной подписки
                        var dispatcherClient = AgentHelper.GetDispatcherClient();
                        var response = dispatcherClient.CreateUserDefaultSubscription(accountId,
                            new CreateUserDefaultSubscriptionRequestData()
                            {
                                Channel = SubscriptionChannel.Email,
                                UserId = user.Id
                            });
                        response.Check();
                    }
                }
            }
        }


        protected static List<UserContact> GetUserEMailContacts(User user)
        {
            var result = user
                .UserContacts.Where(x => x.Type == UserContactType.Email && !string.IsNullOrEmpty(x.Value))
                .ToList();

            if (result.Count == 0)
            {
                var login = user.Login;
                if (ValidationHelper.IsEmail(login))
                {
                    result.Add(new UserContact()
                    {
                        Type = UserContactType.Email,
                        Value = login
                    });
                }
            }

            return result;
        }

        protected static List<UserContact> GetUserMobileContacts(User user)
        {
            var result = user
                .UserContacts.Where(x => x.Type == UserContactType.MobilePhone && !string.IsNullOrEmpty(x.Value))
                .ToList();

            return result;
        }

        protected static List<UserContact> GetUserHttpContacts(User user)
        {
            var result = user
                .UserContacts.Where(x => x.Type == UserContactType.Http && !string.IsNullOrEmpty(x.Value))
                .ToList();

            return result;
        }

        protected void AddNotification(
            LastComponentNotification lastComponentNotification,
            StatusInfo statusInfo,
            Subscription subscription,
            AccountDbContext accountDbContext,
            NotificationReason reason)
        {

            var realEvent = accountDbContext.Events.Find(statusInfo.EventId);
            if (realEvent == null)
            {
                return;
            }

            Interlocked.Increment(ref CreatedNotificationsCount);

            var newNotification = new Notification()
            {
                Id = Guid.NewGuid(),
                Address = lastComponentNotification.Address,
                CreationDate = Now(),
                EventId = statusInfo.EventId,
                Status = NotificationStatus.InQueue,
                SubscriptionId = subscription.Id,
                Type = lastComponentNotification.Type,
                UserId = subscription.UserId,
                Reason = reason
            };

            if (newNotification.Type == NotificationType.Http)
            {
                newNotification.NotificationHttp = new NotificationHttp()
                {
                    Notification = newNotification
                    //Json = GetNotificationJson(lastComponentNotification.Component, eventObj)
                };
            }

            var notificationRepository = accountDbContext.GetNotificationRepository();
            notificationRepository.Add(newNotification);

            lastComponentNotification.Update(newNotification, statusInfo.Importance);

            accountDbContext.SaveChanges();
        }

        protected DateTime Now()
        {
            return AgentHelper.GetDispatcherClient().GetServerTime().Data.Date;
        }
    }
}
