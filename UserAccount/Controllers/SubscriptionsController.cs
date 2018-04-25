using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;
using Zidium.UserAccount.Models.Others;
using Zidium.UserAccount.Models.Subscriptions;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class SubscriptionsController : ContextController
    {
        public ActionResult Index(Guid? userId = null)
        {
            // получаем пользователя
            if (userId == null)
            {
                userId = CurrentUser.Id;
            }

            // получаем подписки
            var subscriptions = CurrentAccountDbContext.GetSubscriptionRepository()
                .QueryAll()
                .Where(x => x.UserId == userId.Value)
                .Where(x=>x.Channel == SubscriptionChannel.Email || x.Channel == SubscriptionChannel.Sms)
                .ToArray();

            var model = new SubscriptionListModel()
            {
                UserId = userId.Value,
                Subscriptions = subscriptions
            };

            return View(model);
        }

        /// <summary>
        /// Добавление новой подписки
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="channel"></param>
        /// <param name="componentTypeId"></param>
        /// <param name="componentId"></param>
        /// <returns></returns>
        public ActionResult Add(
            Guid? userId,
            SubscriptionObject? @object,
            SubscriptionChannel? channel,
            Guid? componentTypeId,
            Guid? componentId)
        {
            if (@object == null)
            {
                throw new UserFriendlyException("Не указан тип добавляемой подписки");
            }
            userId = userId ?? CurrentUser.Id;
            var model = new SubscriptionEditModel()
            {
                ModalMode = Request.IsAjaxRequest(),
                ReturnUrl = Url.Action("Index", new { userId = userId }),
                Id = null,
                Object = @object.Value,
                NotifyBetterStatus = false,
                UserId = userId,
                CanShowChannel = channel==null,
                CanShowComponentType = componentTypeId==null,
                CanShowComponent = componentId==null,
                Channel = channel ?? SubscriptionChannel.Email,
                IsEnabled = true,
                Color = ColorStatusSelectorValue.FromColor(ObjectColor.Gray),
                MinimumDuration = null,
                ResendTime = null,
                ComponentTypeId = componentTypeId,
                ComponentId = componentId
            };
            return View("Edit", model);
        }

        /// <summary>
        /// Редактирует подписку
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            var subscriptionRepository = CurrentAccountDbContext.GetSubscriptionRepository();
            var subscription = subscriptionRepository.GetById(id);
            var model = new SubscriptionEditModel()
            {
                ModalMode = Request.IsAjaxRequest(),
                ReturnUrl = Url.Action("Index", new {userId = subscription.UserId}),
                Id = subscription.Id,
                Object = subscription.Object,
                NotifyBetterStatus = subscription.NotifyBetterStatus,
                UserId = subscription.UserId,
                ComponentTypeId = subscription.ComponentTypeId,
                ComponentId = subscription.ComponentId,
                Channel = subscription.Channel,
                CanShowChannel = false,
                CanShowComponentType = false,
                CanShowComponent = false,
                IsEnabled = subscription.IsEnabled,
                Color = ColorStatusSelectorValue.FromEventImportance(subscription.Importance),
                MinimumDuration = TimeSpanHelper.FromSeconds(subscription.DurationMinimumInSeconds),
                ResendTime = TimeSpanHelper.FromSeconds(subscription.ResendTimeInSeconds)
            };
            return View(model);
        }

        [HttpPost]
        [CanEditPrivateData]
        public ActionResult Edit(SubscriptionEditModel model)
        {
            var modelState = new ModelStateHelper<SubscriptionEditModel>(ModelState);

            // Проверка ComponentTypeId
            if (model.Object == SubscriptionObject.ComponentType && model.ComponentTypeId==null)
            {
                modelState.AddErrorFor(x => x.ComponentTypeId, "Выберите тип компонента");
            }

            // Проверка ComponentId
            if (model.Object == SubscriptionObject.Component && model.ComponentId == null)
            {
                modelState.AddErrorFor(x => x.ComponentId, "Выберите компонент");
            }

            // Проверка Channel
            if (model.Id==null && model.Channel==null)
            {
                // канал должен указываться явно только для новых подписок
                modelState.AddErrorFor(x => x.Channel, "Выберите канал");
            }

            // Проверка UserId
            if (model.Id == null && model.UserId == null)
            {
                // Пользователь должен указываться явно только для новых подписок
                modelState.AddErrorFor(x => x.UserId, "Выберите пользователя");
            }

            // проверка цвета
            var color = model.Color.GetSelectedOne();
            if (color == null)
            {
                modelState.AddErrorFor(x=>x.Color, "Укажите цвет");
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // проверка прав
                var isOtherUser = CurrentUser.Id != model.UserId;
                if (isOtherUser)
                {
                    if (CurrentUser.IsAdmin() == false)
                    {
                        throw new UserFriendlyException("Нет прав на создание подписок другим пользователям");
                    }
                }

                var importance = ImportanceHelper.Get(color).Value;
                var client = GetDispatcherClient();
                if (model.Id == null)
                {
                    // создание новой подписки
                    var createData = new CreateSubscriptionRequestData()
                    {
                        UserId = model.UserId.Value,
                        Channel = model.Channel.Value,
                        DurationMinimumInSeconds = TimeSpanHelper.GetSeconds(model.MinimumDuration),
                        ResendTimeInSeconds = TimeSpanHelper.GetSeconds(model.ResendTime),
                        Importance = importance,
                        IsEnabled = model.IsEnabled,
                        NotifyBetterStatus = model.NotifyBetterStatus,
                        Object = model.Object
                    };
                    if (model.Object == SubscriptionObject.Component)
                    {
                        createData.ComponentId = model.ComponentId;
                    }
                    if (model.Object == SubscriptionObject.ComponentType)
                    {
                        createData.ComponentTypeId = model.ComponentTypeId;
                    }
                    var response = client.CreateSubscription(CurrentUser.AccountId, createData);
                    response.Check();
                    model.Id = response.Data.Id;
                }
                else
                {
                    // редактирование существующей подписки
                    var updateData = new UpdateSubscriptionRequestData()
                    {
                        Id = model.Id.Value,
                        NotifyBetterStatus = model.NotifyBetterStatus,
                        IsEnabled = model.IsEnabled,
                        ResendTimeInSeconds = TimeSpanHelper.GetSeconds(model.ResendTime),
                        DurationMinimumInSeconds = TimeSpanHelper.GetSeconds(model.MinimumDuration),
                        Importance = importance
                    };
                    var response = client.UpdateSubscription(CurrentUser.AccountId, updateData);
                    response.Check();
                }
                if (model.ModalMode)
                {
                    return GetSuccessJsonResponse(new {subscriptionId = model.Id});
                }
                return Redirect(model.ReturnUrl);
            }
            catch (UserFriendlyException exception)
            {
                model.Exception = exception;
            }

            return View(model);
        }

        protected void CheckEditingPermissions(Subscription subscription)
        {
            if (!CurrentUser.CanManageAccount() && CurrentUser.Id != subscription.UserId)
                throw new NoAccessToPageException();
        }

        [HttpPost]
        [CanEditPrivateData]
        public ActionResult Enable(string ids, bool isEnable, Guid userId, SubscriptionChannel channel)
        {
            var subscriptionRepository = CurrentAccountDbContext.GetSubscriptionRepository();

            var list = ids.Split(',');
            foreach (var item in list)
            {
                var id = new Guid(item);

                var subscription = subscriptionRepository.GetById(id);
                CheckEditingPermissions(subscription);

                var client = GetDispatcherClient();
                var response = client.CreateSubscription(CurrentUser.AccountId, new CreateSubscriptionRequestData()
                {
                    UserId = subscription.UserId,
                    ComponentTypeId = subscription.ComponentTypeId,
                    Channel = subscription.Channel,
                    Object = subscription.Object
                });
                var data = response.Data;

                var response2 = client.UpdateSubscription(CurrentUser.AccountId, new UpdateSubscriptionRequestData()
                {
                    Id = data.Id,
                    NotifyBetterStatus = data.NotifyBetterStatus,
                    IsEnabled = isEnable,
                    Importance = data.Importance,
                    DurationMinimumInSeconds = data.DurationMinimumInSeconds,
                    ResendTimeInSeconds = data.ResendTimeInSeconds
                });
                response2.Check();
            }

            if (Request.IsAjaxRequest())
                return new EmptyResult();

            return RedirectToAction("Index", new { userId = userId, channel = channel });
        }

        [HttpPost]
        [CanEditPrivateData]
        public ActionResult Color(string ids, string color, Guid userId, SubscriptionChannel channel)
        {
            var colorValue = ColorStatusSelectorValue.FromString(color);
            var colors = colorValue.GetSelectedEventImportances();

            var subscriptionRepository = CurrentAccountDbContext.GetSubscriptionRepository();

            var list = ids.Split(',');
            foreach (var item in list)
            {
                var id = new Guid(item);

                var subscription = subscriptionRepository.GetById(id);
                CheckEditingPermissions(subscription);

                var client = GetDispatcherClient();
                var response = client.CreateSubscription(CurrentUser.AccountId, new CreateSubscriptionRequestData()
                {
                    UserId = subscription.UserId,
                    ComponentTypeId = subscription.ComponentTypeId,
                    Channel = subscription.Channel,
                    Object = subscription.Object
                });
                var data = response.Data;

                var response2 = client.UpdateSubscription(CurrentUser.AccountId, new UpdateSubscriptionRequestData()
                {
                    Id = data.Id,
                    IsEnabled = data.IsEnabled,
                    Importance = colors.Count > 0 ? colors[0] : data.Importance,
                    DurationMinimumInSeconds = data.DurationMinimumInSeconds,
                    ResendTimeInSeconds = data.ResendTimeInSeconds
                });
                response2.Check();
            }

            if (Request.IsAjaxRequest())
                return new EmptyResult();

            return RedirectToAction("Index", new { userId = userId, channel = channel });
        }

        public ActionResult Delete(Guid id)
        {
            var model = new DeleteConfirmationSmartModel()
            {
                Id = id,
                Message = "Вы действительно хотите удалить подписку?"
            };
            return ViewDialog(model);
        }

        [HttpPost]
        [CanEditPrivateData]
        public ActionResult Delete(DeleteConfirmationSmartModel model)
        {
            try
            {
                // TODO Удаление подписок нужно делать через Диспетчер

                var repository = CurrentAccountDbContext.GetSubscriptionRepository();
                var subscription = repository.GetById(model.Id);
                CheckEditingPermissions(subscription);

                var subscriptionService = new SubscriptionService(DbContext);
                subscriptionService.Remove(CurrentUser.AccountId, subscription);

                CurrentAccountDbContext.SaveChanges();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
                return GetErrorJsonResponse(exception);
            }
        }

        public ActionResult ShowComponentSubscriptions(
            Guid componentId, 
            Guid? userId, 
            ShowComponentSubscriptionsModel.ViewModeCode? viewMode, 
            SubscriptionChannel? channel)
        {
            if (viewMode == null)
            {
                viewMode = ShowComponentSubscriptionsModel.ViewModeCode.My;
            }
            if (viewMode.Value == ShowComponentSubscriptionsModel.ViewModeCode.My)
            {
                userId = null;
                channel = null;
            }
            var model = new ShowComponentSubscriptionsModel()
            {
                ComponentId = componentId,
                UserId = userId,
                Channel = channel,
                ViewMode = viewMode.Value
            };
            model.Init();
            return PartialView(model);
        }

        public ActionResult EditComponentSubscriptions(
            Guid componentId,
            Guid? userId)
        {
            var component = GetComponentById(componentId);
            userId = userId ?? CurrentUser.Id;
            var user = GetUserById(userId.Value);

            var subscriptions = CurrentAccountDbContext
                .GetSubscriptionRepository()
                .QueryAll()
                .Where(x => x.UserId == userId
                    && (x.Object == SubscriptionObject.Default
                    || (x.Object == SubscriptionObject.ComponentType && x.ComponentTypeId == component.ComponentTypeId)
                    || (x.Object == SubscriptionObject.Component && x.ComponentId == componentId)))
                .ToArray();
            

            var model = new EditComponentSubscriptionsModel()
            {
                Component = component,
                User = user,
                AllSubscriptions = subscriptions
            };

            return View(model);
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        public SubscriptionsController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public SubscriptionsController() { }
    }
}