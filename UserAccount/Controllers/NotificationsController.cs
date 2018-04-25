using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class NotificationsController : ContextController
    {
        public ActionResult Index(Guid? componentId = null, string fromDate = null, string toDate = null,
            string category = null, string channel = null, string status = null, Guid? userId = null)
        {
            var sDate = !string.IsNullOrEmpty(fromDate) ? DecodeDateTimeParameter(fromDate) : (DateTime?) null;
            var eDate = !string.IsNullOrEmpty(toDate) ? DecodeDateTimeParameter(toDate) : (DateTime?) null;

            var eventCategory = EnumHelper.StringToEnum<EventCategory>(category);
            var nChannel = EnumHelper.StringToEnum<NotificationType>(channel);
            var nStatus = EnumHelper.StringToEnum<NotificationStatus>(status);

            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            var userRepository = CurrentAccountDbContext.GetUserRepository();

            if (!CurrentUser.CanManageAccount())
                userId = CurrentUser.Id;

            if (userId.HasValue)
                userRepository.GetById(userId.Value);

            var users = userRepository.QueryAll().ToArray();
            var eventTypeRepository = CurrentAccountDbContext.GetEventTypeRepository();
            var eventTypes = eventTypeRepository.QueryAll().ToArray();
            var allComponents = componentRepository.QueryAllWithDeleted();

            var notificationRepository = CurrentAccountDbContext.GetNotificationRepository();
            var query = notificationRepository.QueryAllForGui(componentId, sDate, eDate, eventCategory, nChannel, nStatus, userId);
            query = query.OrderByDescending(t => t.CreationDate).Take(1000);
            var notifications = query.ToArray()
                .Join(users, a => a.UserId, b => b.Id, (a, b) => new {Notification = a, User = b})
                .Join(eventTypes, a => a.Notification.Event.EventTypeId, b => b.Id,
                    (a, b) => new {Notification = a, EventType = b})
                .Select(t => new NotificationsListItemModel()
                {
                    Id = t.Notification.Notification.Id,
                    CreationDate = t.Notification.Notification.CreationDate,
                    Event = t.Notification.Notification.Event,
                    User = t.Notification.User,
                    Component = allComponents.Single(x => x.Id == t.Notification.Notification.Event.OwnerId),
                    Address = t.Notification.Notification.Address,
                    Channel = t.Notification.Notification.Type,
                    Status = t.Notification.Notification.Status,
                    SendError = t.Notification.Notification.SendError,
                    SendDate = t.Notification.Notification.SendDate,
                    NextDate = null,
                    EventType = t.EventType
                });

            var model = new NotificationsListModel()
            {
                AccountId = CurrentUser.AccountId,
                ComponentId = componentId,
                FromDate = sDate,
                ToDate = eDate,
                Category = eventCategory,
                Channel = nChannel,
                Status = nStatus,
                UserId = userId,
                Notifications = notifications.OrderByDescending(t => t.CreationDate).ToList()
            };

            return View(model);
        }

        public ActionResult Show(Guid id, Guid componentId)
        {
            var storageContext = CurrentAccountDbContext;
            var notificationRepository = storageContext.GetNotificationRepository();
            var notification = notificationRepository.Find(id, componentId);

            if (!CurrentUser.CanManageAccount() && CurrentUser.Id != notification.UserId)
                throw new NoAccessToPageException();

            var userRepository = CurrentAccountDbContext.GetUserRepository();
            var user = userRepository.GetById(notification.UserId);
            var subscriptionRepository = CurrentAccountDbContext.GetSubscriptionRepository();
            var subscription = notification.SubscriptionId.HasValue ? subscriptionRepository.GetById(notification.SubscriptionId.Value) : null;
            var model = new NotificationDetailsModel()
            {
                Id = notification.Id,
                Channel = notification.Type,
                CreationDate = notification.CreationDate,
                Event = notification.Event,
                SendDate = notification.SendDate,
                SendError = notification.SendError,
                Status = notification.Status,
                Subscription = subscription,
                User = user,
                Address = notification.Address
            };
            return View(model);
        }

        // Для unit-тестов

        public NotificationsController() { }

        public NotificationsController(Guid accountId, Guid userId) : base(accountId, userId) { }
    }
}