using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.Notifications
{
    public class CreateNotificationsProcessor
    {
        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public int CreatedNotificationsCount;

        /// <summary>
        /// Максимальное количество событий, которое будет обработано у одного хранилища за одну итерацию
        /// </summary>
        public int EventMaxCount = 1000;

        public CreateNotificationsProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        public void Process(Guid? componentId = null, Guid? userId = null)
        {
            // Получим все подписки
            var storage = DependencyInjection.GetServicePersistent<IDefaultStorageFactory>().GetStorage();
            var subscriptions = storage.Subscriptions.GetAll();
            var userToSubscriptions = subscriptions.GroupBy(x => x.UserId).ToDictionary(x => x.Key, y => y.ToArray());

            // получим всех пользователей с их часовыми поясами
            var userSettingService = new UserSettingService(storage);
            var users = storage.Users.GetForNotifications()
                .ToArray()
                .Select(t => new UserInfo()
                {
                    Id = t.Id,
                    Login = t.Login,
                    CreateDate = t.CreateDate,
                    TimeZoneOffsetMinutes = userSettingService.TimeZoneOffsetMinutes(t.Id)
                })
                .ToArray();

            if (userId.HasValue)
                users = users.Where(t => t.Id == userId.Value).ToArray();

            // обработка старых статусов
            ProcessAccountArchivedStatuses(
                storage,
                users,
                userToSubscriptions,
                componentId);

            // обработка текущих статусов
            ProcessAccountCurrentStatuses(
                storage,
                users,
                userToSubscriptions,
                componentId);

            if (CreatedNotificationsCount > 0)
                Logger.LogInformation("Создано уведомлений: {0}", CreatedNotificationsCount);
        }

        protected void ProcessAccountArchivedStatuses(
            IStorage storage,
            UserInfo[] users,
            Dictionary<Guid, SubscriptionForRead[]> userToSubscriptions,
            Guid? componentId = null)
        {
            //получим все компоненты, т.к. для каждого нужно проверить наличие уведомлений по его текущему статусу
            var components = storage.Components.GetForNotifications(componentId).ToDictionary(a => a.Id, b => b);

            // получим архивные статусы
            var archivedStatuses = storage.ArchivedStatuses.GetForNotifications(componentId, EventMaxCount);

            var now = Now();
            var statuses = new List<StatusInfo>(archivedStatuses.Length);
            foreach (var archivedStatus in archivedStatuses)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var eventObj = archivedStatus.Event;
                components.TryGetValue(eventObj.OwnerId, out var component);

                if (component == null)
                {
                    continue;
                }

                // удалим, чтобы больше не обрабатывать
                storage.ArchivedStatuses.Delete(archivedStatus.Id);

                var status = StatusInfo.Create(eventObj, component, now);
                statuses.Add(status);
            }

            ProcessAccountInternal(
                users,
                userToSubscriptions,
                statuses,
                storage);
        }

        protected void ProcessAccountCurrentStatuses(
            IStorage storage,
            UserInfo[] users,
            Dictionary<Guid, SubscriptionForRead[]> userToSubscriptions,
            Guid? componentId = null)
        {
            // получим колбаски компонентов
            var bulbs = storage.Bulbs.GetForNotifications();

            if (componentId.HasValue)
                bulbs = bulbs.Where(t => t.ComponentId == componentId.Value).ToArray();

            var now = Now();
            var statuses = new List<StatusInfo>(bulbs.Length);
            foreach (var bulb in bulbs)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var status = StatusInfo.Create(bulb, now);
                statuses.Add(status);
            }

            ProcessAccountInternal(
                users,
                userToSubscriptions,
                statuses,
                storage);
        }

        protected void ProcessAccountInternal(
            UserInfo[] users,
            Dictionary<Guid, SubscriptionForRead[]> userToSubscriptions,
            List<StatusInfo> statuses,
            IStorage storage)
        {
            // Обработаем каждый статус
            foreach (var status in statuses)
            {
                Logger.LogDebug("Обрабатываем событие: " + status.EventId);

                CancellationToken.ThrowIfCancellationRequested();

                // Пропускаем корневой (причина?)
                if (status.ComponentTypeId == SystemComponentType.Root.Id)
                {
                    Logger.LogTrace("component.IsRoot");
                    continue;
                }

                // цикл по пользователям
                foreach (var user in users)
                {
                    var channels = SubscriptionHelper.AvailableSubscriptionChannels;

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
                                subscription = channelSubscriptions.FirstOrDefault(x => x.ComponentTypeId == status.ComponentTypeId);
                                if (subscription == null)
                                {
                                    // ищем дефолтную подписку
                                    subscription = channelSubscriptions.FirstOrDefault(x => x.Object == SubscriptionObject.Default);

                                    // если дефолтной для email канала нет, то создаем её
                                    if (subscription == null && channel == SubscriptionChannel.Email)
                                    {
                                        var dispatcherClient = AgentHelper.GetDispatcherClient();

                                        var response = dispatcherClient.CreateUserDefaultSubscription(
                                            new CreateUserDefaultSubscriptionRequestData()
                                            {
                                                UserId = user.Id
                                            });

                                        subscription = storage.Subscriptions.GetOneById(response.Data.Id);
                                    }
                                }
                            }

                            // проверим подписку
                            if (subscription != null)
                            {
                                Logger.LogTrace("Проверяем подписку " + subscription.Id + ", событие " + status.EventId + " и компонент " + status.ComponentId);

                                // если отправка запрещена
                                if (subscription.IsEnabled == false)
                                {
                                    Logger.LogTrace("False by IsEnabled");
                                    continue;
                                }

                                // подписка шлет уведомления только по событиям, которые создались после создания подписки
                                if (status.CreateDate < subscription.LastUpdated)
                                {
                                    Logger.LogTrace("False by LastUpdated");
                                    continue;
                                }

                                if (status.Category != EventCategory.ComponentExternalStatus)
                                {
                                    Logger.LogTrace("False by Category");
                                    continue;
                                }

                                // минимальную длительность проверяем только у важных событий (цвет важности указывается пользователем в подписке)
                                if (subscription.DurationMinimumInSeconds.HasValue && status.Importance >= subscription.Importance)
                                {
                                    var eventDuration = (int)status.Duration.TotalSeconds;
                                    if (eventDuration < subscription.DurationMinimumInSeconds.Value)
                                    {
                                        Logger.LogTrace("False by DurationMinimumInSeconds");
                                        continue;
                                    }
                                }

                                // проверяем интервал отправки
                                if (subscription.SendOnlyInInterval)
                                {
                                    // переводим текущее время в часовой пояс клиента
                                    var nowUtc = Now().ToUniversalTime();
                                    var nowOffset = new DateTimeOffset(nowUtc);
                                    var nowForUser = nowOffset.ToOffset(TimeSpan.FromMinutes(user.TimeZoneOffsetMinutes));
                                    var nowForUserTime = nowForUser.TimeOfDay;
                                    var fromTime = TimeSpan.FromMinutes(subscription.SendIntervalFromHour.Value * 60 + subscription.SendIntervalFromMinute.Value);
                                    var toTime = TimeSpan.FromMinutes(subscription.SendIntervalToHour.Value * 60 + subscription.SendIntervalToMinute.Value);
                                    if (nowForUserTime < fromTime || nowForUserTime > toTime)
                                    {
                                        Logger.LogTrace("False by SendInterval");
                                        continue;
                                    }
                                }

                                Logger.LogDebug("Создаем уведомления для подписки: " + subscription.Id);

                                CancellationToken.ThrowIfCancellationRequested();

                                UserContactForRead[] contacts;

                                if (subscription.Channel == SubscriptionChannel.Email)
                                    contacts = UserHelper.GetUserContactsOfType(user.Id, user.Login, user.CreateDate, UserContactType.Email, storage);
                                else if (subscription.Channel == SubscriptionChannel.Sms)
                                    contacts = UserHelper.GetUserContactsOfType(user.Id, user.Login, user.CreateDate, UserContactType.MobilePhone, storage);
                                else if (subscription.Channel == SubscriptionChannel.Http)
                                    contacts = UserHelper.GetUserContactsOfType(user.Id, user.Login, user.CreateDate, UserContactType.Http, storage);
                                else if (subscription.Channel == SubscriptionChannel.Telegram)
                                    contacts = UserHelper.GetUserContactsOfType(user.Id, user.Login, user.CreateDate, UserContactType.Telegram, storage);
                                else if (subscription.Channel == SubscriptionChannel.VKontakte)
                                    contacts = UserHelper.GetUserContactsOfType(user.Id, user.Login, user.CreateDate, UserContactType.VKontakte, storage);
                                else
                                    contacts = new UserContactForRead[0];

                                Logger.LogDebug("Адресов для уведомлений " + contacts.Length);

                                foreach (var contact in contacts)
                                {
                                    // Уведомления создаются только по контактам, добавленным до наступления события
                                    if (contact.CreateDate > status.CreateDate)
                                        continue;

                                    var address = contact.Value;

                                    Logger.LogDebug("Создаём уведомление на адрес: " + address);

                                    var lastComponentNotification = status.LastComponentNotifications
                                        .FirstOrDefault(x => x.Address == address && x.Type == subscription.Channel);

                                    var isImportanceColor = status.Importance >= subscription.Importance;

                                    // важный статус
                                    if (isImportanceColor)
                                    {
                                        // первое уведомление о важном статусе
                                        if (lastComponentNotification == null)
                                        {
                                            Logger.LogInformation("Первое уведомление на адрес " + address + " для компонента " + status.ComponentId);
                                            lastComponentNotification = new ComponentGetForNotificationsInfo.LastComponentNotificationInfo()
                                            {
                                                Id = Guid.Empty,
                                                CreateDate = Now(),
                                                Type = subscription.Channel,
                                                EventImportance = status.Importance,
                                                EventId = status.EventId,
                                                Address = address
                                            };

                                            AddNotification(
                                                lastComponentNotification,
                                                status,
                                                subscription,
                                                storage,
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
                                                    Logger.LogInformation("Напоминание о статусе " + status.EventId + " на адрес " + address);

                                                    AddNotification(
                                                        lastComponentNotification,
                                                        status,
                                                        subscription,
                                                        storage,
                                                        NotificationReason.Reminder);

                                                    continue;
                                                }
                                            }

                                            Logger.LogDebug("Уже есть уведомление о событии " + status.EventId + " на адрес " + address);
                                        }
                                        else // новый важный статус
                                        {
                                            Logger.LogInformation("Новый важный статус " + status.EventId + " на адрес " + address);

                                            AddNotification(
                                                lastComponentNotification,
                                                status,
                                                subscription,
                                                storage,
                                                NotificationReason.NewImportanceStatus);
                                        }
                                    }
                                    else // НЕ важное событие
                                    {
                                        if (lastComponentNotification != null
                                            && subscription.NotifyBetterStatus
                                            && lastComponentNotification.EventImportance >= subscription.Importance)
                                        {
                                            Logger.LogInformation("Уведомление о том, что стало лучше " + status.EventId + " на адрес " + address);

                                            AddNotification(
                                                lastComponentNotification,
                                                status,
                                                subscription,
                                                storage,
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
                        var response = dispatcherClient.CreateUserDefaultSubscription(
                            new CreateUserDefaultSubscriptionRequestData()
                            {
                                UserId = user.Id
                            });
                        response.Check();
                    }
                }
            }
        }

        protected void AddNotification(
            ComponentGetForNotificationsInfo.LastComponentNotificationInfo lastComponentNotification,
            StatusInfo statusInfo,
            SubscriptionForRead subscription,
            IStorage storage,
            NotificationReason reason)
        {
            Interlocked.Increment(ref CreatedNotificationsCount);

            using (var transaction = storage.BeginTransaction())
            {
                var newNotification = new NotificationForAdd()
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
                storage.Notifications.Add(newNotification);

                if (lastComponentNotification.Id == Guid.Empty)
                {
                    var lastComponentNotificationForAdd = new LastComponentNotificationForAdd()
                    {
                        Id = Guid.NewGuid(),
                        Address = lastComponentNotification.Address,
                        ComponentId = statusInfo.ComponentId,
                        CreateDate = lastComponentNotification.CreateDate,
                        EventId = lastComponentNotification.EventId,
                        EventImportance = lastComponentNotification.EventImportance,
                        Type = lastComponentNotification.Type,
                        NotificationId = newNotification.Id
                    };
                    storage.LastComponentNotifications.Add(lastComponentNotificationForAdd);
                    lastComponentNotification.Id = lastComponentNotificationForAdd.Id;
                }

                if (newNotification.Type == SubscriptionChannel.Http)
                {
                    var newNotificationHttp = new NotificationHttpForAdd()
                    {
                        NotificationId = newNotification.Id,
                        // Json = GetNotificationJson(lastComponentNotification.Component, eventObj)
                    };
                    storage.NotificationsHttp.Add(newNotificationHttp);
                }

                var lastComponentNotificationForUpdate = new LastComponentNotificationForUpdate(lastComponentNotification.Id);
                lastComponentNotificationForUpdate.EventImportance.Set(statusInfo.Importance);
                lastComponentNotificationForUpdate.CreateDate.Set(newNotification.CreationDate);
                lastComponentNotificationForUpdate.EventId.Set(newNotification.EventId);
                lastComponentNotificationForUpdate.NotificationId.Set(newNotification.Id);
                storage.LastComponentNotifications.Update(lastComponentNotificationForUpdate);

                transaction.Commit();
            }
        }

        protected DateTime Now()
        {
            return _nowOverride ?? AgentHelper.GetDispatcherClient().GetServerTime().Data.Date;
        }

        private DateTime? _nowOverride;

        /// <summary>
        /// Для юнит-тестов
        /// </summary>
        public void SetNow(DateTime now)
        {
            _nowOverride = now;
        }

        protected class UserInfo
        {
            public Guid Id;

            public string Login;

            public DateTime CreateDate;

            public int TimeZoneOffsetMinutes;
        }

        protected class StatusInfo
        {
            public Guid EventId;

            public DateTime CreateDate;

            public Guid ComponentId;

            public Guid ComponentTypeId;

            public ComponentGetForNotificationsInfo.LastComponentNotificationInfo[] LastComponentNotifications;

            public EventCategory Category;

            public TimeSpan Duration;

            public EventImportance Importance;

            public EventImportance PreviousImportance;

            public static StatusInfo Create(ArchivedStatusGetForNotificationsInfo.EventInfo eventObj, ComponentGetForNotificationsInfo component, DateTime now)
            {
                return new StatusInfo()
                {
                    Category = eventObj.Category,
                    EventId = eventObj.Id,
                    ComponentId = component.Id,
                    ComponentTypeId = component.ComponentTypeId,
                    LastComponentNotifications = component.LastComponentNotifications.ToArray(),
                    Duration = EventHelper.GetDuration(eventObj.StartDate, eventObj.ActualDate, now),
                    Importance = eventObj.Importance,
                    CreateDate = eventObj.CreateDate,
                    PreviousImportance = eventObj.PreviousImportance
                };
            }

            public static StatusInfo Create(BulbGetForNotificationsInfo bulb, DateTime now)
            {
                return new StatusInfo()
                {
                    Category = bulb.Category,
                    EventId = bulb.StatusEventId,
                    ComponentId = bulb.ComponentId,
                    ComponentTypeId = bulb.ComponentTypeId,
                    LastComponentNotifications = bulb.LastComponentNotifications.ToArray(),
                    Duration = BulbExtensions.GetDuration(bulb.StartDate, bulb.ActualDate, now),
                    Importance = EventImportanceHelper.Get(bulb.Status),
                    CreateDate = bulb.StartDate,
                    PreviousImportance = EventImportanceHelper.Get(bulb.PreviousStatus)
                };
            }
        }

    }
}
