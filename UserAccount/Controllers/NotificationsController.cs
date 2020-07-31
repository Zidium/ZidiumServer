using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class NotificationsController : BaseController
    {
        public ActionResult Index(Guid? componentId = null, DateTime? fromDate = null, DateTime? toDate = null,
            EventCategory? category = null, SubscriptionChannel? channel = null,
            NotificationStatus? status = null, Guid? userId = null)
        {
            if (!CurrentUser.CanManageAccount())
                userId = CurrentUser.Id;

            var notifications = GetStorage().Notifications.Filter(componentId, fromDate, toDate, category, channel, status, userId, 100);
            var events = GetStorage().Events.GetMany(notifications.Select(t => t.EventId).Distinct().ToArray()).ToDictionary(a => a.Id, b => b);
            var components = GetStorage().Components.GetMany(events.Values.Select(t => t.OwnerId).Distinct().ToArray()).ToDictionary(a => a.Id, b => b);
            var users = GetStorage().Users.GetMany(notifications.Select(t => t.UserId).Distinct().ToArray()).ToDictionary(a => a.Id, b => b);
            var eventTypes = GetStorage().EventTypes.GetMany(events.Values.Select(t => t.EventTypeId).Distinct().ToArray()).ToDictionary(a => a.Id, b => b);

            var items = notifications
                .Select(t => new NotificationsListItemModel()
                {
                    Id = t.Id,
                    CreationDate = t.CreationDate,
                    Event = events[t.EventId],
                    User = users[t.UserId],
                    Component = components[events[t.EventId].OwnerId],
                    Address = t.Address,
                    Channel = t.Type,
                    Status = t.Status,
                    SendError = t.SendError,
                    SendDate = t.SendDate,
                    NextDate = null,
                    EventType = eventTypes[events[t.EventId].EventTypeId]
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
                Notifications = items.ToArray()
            };

            return View(model);
        }

        public ActionResult Show(Guid id)
        {
            var notification = GetStorage().Notifications.GetOneById(id);

            if (!CurrentUser.CanManageAccount() && CurrentUser.Id != notification.UserId)
                throw new NoAccessToPageException();

            var eventObj = GetStorage().Events.GetOneById(notification.EventId);
            var subscription = notification.SubscriptionId.HasValue ? GetStorage().Subscriptions.GetOneById(notification.SubscriptionId.Value) : null;
            var user = GetStorage().Users.GetOneById(notification.UserId);
            var sendEmailCommand = notification.SendEmailCommandId.HasValue ? GetStorage().SendEmailCommands.GetOneById(notification.SendEmailCommandId.Value) : null;
            var sendMessageCommand = notification.SendMessageCommandId.HasValue ? GetStorage().SendMessageCommands.GetOneById(notification.SendMessageCommandId.Value) : null;

            var model = new NotificationDetailsModel()
            {
                Id = notification.Id,
                Channel = notification.Type,
                CreationDate = notification.CreationDate,
                Event = eventObj,
                SendDate = notification.SendDate,
                SendError = notification.SendError,
                Status = notification.Status,
                Subscription = subscription,
                User = user,
                Address = notification.Address,
                Text = sendEmailCommand?.Body ?? sendMessageCommand?.Body
            };
            return View(model);
        }

        public ActionResult SendNotification()
        {
            var model = new SendNotificationModel()
            {
                Channel = SubscriptionChannel.Email,
                UserId = CurrentUser.Id,
                CommonWebsiteUrl = GetConfigDbServicesFactory().GetUrlService().GetCommonWebsiteUrl()
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

            var user = GetStorage().Users.GetOneById(model.UserId);

            UserContactForRead[] contacts;

            if (model.Channel == SubscriptionChannel.Email)
                contacts = Core.UserHelper.GetUserContactsOfType(user, UserContactType.Email, GetStorage());
            else if (model.Channel == SubscriptionChannel.Sms)
                contacts = Core.UserHelper.GetUserContactsOfType(user, UserContactType.MobilePhone, GetStorage());
            else if (model.Channel == SubscriptionChannel.Http)
                contacts = Core.UserHelper.GetUserContactsOfType(user, UserContactType.Http, GetStorage());
            else if (model.Channel == SubscriptionChannel.Telegram)
                contacts = Core.UserHelper.GetUserContactsOfType(user, UserContactType.Telegram, GetStorage());
            else if (model.Channel == SubscriptionChannel.VKontakte)
                contacts = Core.UserHelper.GetUserContactsOfType(user, UserContactType.VKontakte, GetStorage());
            else
                contacts = new UserContactForRead[0];

            if (contacts.Length == 0)
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
                using (var transaction = GetStorage().BeginTransaction())
                {
                    var notification = new NotificationForAdd()
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
                    GetStorage().Notifications.Add(notification);

                    if (notification.Type == SubscriptionChannel.Http)
                    {
                        var notificationHttp = new NotificationHttpForAdd()
                        {
                            NotificationId = notification.Id
                        };
                        GetStorage().NotificationsHttp.Add(notificationHttp);
                    }

                    notificationId = notification.Id;

                    transaction.Commit();
                }
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