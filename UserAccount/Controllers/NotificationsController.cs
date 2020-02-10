using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.ConfigDb;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class NotificationsController : ContextController
    {
        public ActionResult Index(Guid? componentId = null, DateTime? fromDate = null, DateTime? toDate = null,
            EventCategory? category = null, SubscriptionChannel? channel = null, 
            NotificationStatus? status = null, Guid? userId = null)
        {
            var componentRepository = CurrentAccountDbContext.GetComponentRepository();

            if (!CurrentUser.CanManageAccount())
                userId = CurrentUser.Id;

            var allComponents = componentRepository.QueryAllWithDeleted();

            var notificationRepository = CurrentAccountDbContext.GetNotificationRepository();
            var query = notificationRepository.QueryAllForGui(componentId, fromDate, toDate, category, channel, status, userId);
            query = query.OrderByDescending(t => t.CreationDate).Take(1000);
            var notifications = query.ToArray()
                .Select(t => new NotificationsListItemModel()
                {
                    Id = t.Id,
                    CreationDate = t.CreationDate,
                    Event = t.Event,
                    User = t.User,
                    Component = allComponents.Single(x => x.Id == t.Event.OwnerId),
                    Address = t.Address,
                    Channel = t.Type,
                    Status = t.Status,
                    SendError = t.SendError,
                    SendDate = t.SendDate,
                    NextDate = null,
                    EventType = t.Event.EventType
                });

            var model = new NotificationsListModel()
            {
                AccountId = CurrentUser.AccountId,
                ComponentId = componentId,
                FromDate = fromDate,
                ToDate = toDate,
                Category = category,
                Channel = channel,
                Status = status,
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

            var model = new NotificationDetailsModel()
            {
                Id = notification.Id,
                Channel = notification.Type,
                CreationDate = notification.CreationDate,
                Event = notification.Event,
                SendDate = notification.SendDate,
                SendError = notification.SendError,
                Status = notification.Status,
                Subscription = notification.Subscription,
                User = notification.User,
                Address = notification.Address,
                Text = notification.SendEmailCommand?.Body ?? notification.SendMessageCommand?.Body
            };
            return View(model);
        }

        public ActionResult SendNotification()
        {
            var model = new SendNotificationModel()
            {
                Channel = SubscriptionChannel.Email,
                UserId = CurrentUser.Id,
                CommonWebsiteUrl = ConfigDbServicesHelper.GetUrlService().GetCommonWebsiteUrl()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult SendNotification(SendNotificationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!CurrentUser.CanManageAccount() && CurrentUser.Id != model.UserId)
            {
                ModelState.AddModelError(string.Empty, "Вы можете отправлять уведомления только себе");
                return View(model);
            }

            var userRepository = CurrentAccountDbContext.GetUserRepository();
            var user = userRepository.GetById(model.UserId);

            List<UserContact> contacts;

            if (model.Channel == SubscriptionChannel.Email)
                contacts = Core.UserHelper.GetUserContactsOfType(user, UserContactType.Email);
            else if (model.Channel == SubscriptionChannel.Sms)
                contacts = Core.UserHelper.GetUserContactsOfType(user, UserContactType.MobilePhone);
            else if (model.Channel == SubscriptionChannel.Http)
                contacts = Core.UserHelper.GetUserContactsOfType(user, UserContactType.Http);
            else if (model.Channel == SubscriptionChannel.Telegram)
                contacts = Core.UserHelper.GetUserContactsOfType(user, UserContactType.Telegram);
            else if (model.Channel == SubscriptionChannel.VKontakte)
                contacts = Core.UserHelper.GetUserContactsOfType(user, UserContactType.VKontakte);
            else
                contacts = new List<UserContact>();

            if (contacts.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "У пользователя нет контактов выбранного типа");
                return View(model);
            }

            var dispatcherClient = GetDispatcherClient();
            var root = dispatcherClient.GetRoot(CurrentUser.AccountId).Data;
            var status = dispatcherClient.GetComponentTotalState(CurrentUser.AccountId, root.Id).Data;

            var notificationId = Guid.Empty;
            foreach (var contact in contacts)
            {
                var notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    Address = contact.Value,
                    CreationDate = Now(),
                    EventId = status.StatusEventId,
                    Status = NotificationStatus.InQueue,
                    SubscriptionId = null,
                    Type = model.Channel,
                    UserId = model.UserId,
                    Reason = NotificationReason.NewImportanceStatus
                };

                if (notification.Type == SubscriptionChannel.Http)
                {
                    notification.NotificationHttp = new NotificationHttp()
                    {
                        Notification = notification
                    };
                }

                var notificationRepository = CurrentAccountDbContext.GetNotificationRepository();
                notificationRepository.Add(notification);
                CurrentAccountDbContext.SaveChanges();

                notificationId = notification.Id;
            }

            if (!IsSmartBlocksRequest())
            {
                var message = "Уведомления добавлены в очередь";

                this.SetTempMessage(TempMessageType.Success, message);
                return RedirectToAction("Index");
            }

            var result = new
            {
                NotificationId = notificationId
            };
            return GetSuccessJsonResponse(result);
        }

        // Для unit-тестов

        public NotificationsController() { }

        public NotificationsController(Guid accountId, Guid userId, bool isSmartBlocksRequest = false) : base(accountId, userId, isSmartBlocksRequest) { }
    }
}