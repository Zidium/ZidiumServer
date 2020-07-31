using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;
using Zidium.UserAccount.Models.Others;
using Zidium.UserAccount.Models.Subscriptions;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class SubscriptionsController : BaseController
    {
        public ActionResult Index(Guid? userId = null)
        {
            // получаем пользователя
            if (userId == null)
            {
                userId = CurrentUser.Id;
            }

            var channels = SubscriptionHelper.AvailableSubscriptionChannels;

            // получаем подписки
            var subscriptions = GetStorage().Subscriptions.Filter(userId.Value, channels);

            var model = new SubscriptionListModel()
            {
                UserId = userId.Value,
                Subscriptions = subscriptions,
                Channels = channels,
                DefaultSubscriptions = GetTable(SubscriptionObject.Default, userId.Value, channels, subscriptions),
                ComponentTypeSubscriptions = GetTable(SubscriptionObject.ComponentType, userId.Value, channels, subscriptions),
                ComponentSubscriptions = GetTable(SubscriptionObject.Component, userId.Value, channels, subscriptions)
            };

            return View(model);
        }

        private SubscriptionsTableModel GetTable(SubscriptionObject obj, Guid userId, SubscriptionChannel[] channels, SubscriptionForRead[] allSubscriptions)
        {
            var model = new SubscriptionsTableModel()
            {
                Object = obj,
                UserId = userId,
                Channels = channels
            };
            var subscriptions = allSubscriptions.Where(x => x.Object == obj).ToArray();

            // подписки по умолчанию
            if (obj == SubscriptionObject.Default)
            {
                var row = new SubscriptionsTableRowModel()
                {
                    Cells = new List<ShowSubscriptionCellModel>(),
                    Table = model,
                    SubscriptionObject = SubscriptionObject.Default
                };

                foreach (var channel in channels)
                {
                    row.Cells.Add(new ShowSubscriptionCellModel()
                    {
                        Channel = channel,
                        Object = SubscriptionObject.Default,
                        Subscription = subscriptions.SingleOrDefault(x => x.Channel == channel && x.Object == SubscriptionObject.Default),
                        Row = row
                    });
                }

                model.Rows = new[] { row };
            }

            // подписки на тип компонента
            if (obj == SubscriptionObject.ComponentType)
            {
                var componentTypeGroups = subscriptions.GroupBy(x => x.ComponentTypeId).Select(x => new
                {
                    ComponentTypeId = x.Key,
                    Subscriptions = x.ToArray()
                }).ToArray();

                var componentTypes = GetStorage().ComponentTypes.GetMany(componentTypeGroups.Select(t => t.ComponentTypeId.GetValueOrDefault()).ToArray()).ToDictionary(a => a.Id, b => b);

                var componentTypeGroupsWithTypes = componentTypeGroups.Select(t => new
                {
                    ComponentTypeId = t.ComponentTypeId,
                    ComponentType = componentTypes[t.ComponentTypeId.GetValueOrDefault()],
                    Subscriptions = t.Subscriptions
                }).OrderBy(t => t.ComponentType.DisplayName);

                var rows = new List<SubscriptionsTableRowModel>();
                foreach (var componentTypeGroup in componentTypeGroupsWithTypes)
                {
                    var row = new SubscriptionsTableRowModel()
                    {
                        Cells = new List<ShowSubscriptionCellModel>(),
                        Table = model,
                        SubscriptionObject = SubscriptionObject.ComponentType,
                        Text = componentTypeGroup.ComponentType.DisplayName
                    };

                    foreach (var channel in channels)
                    {
                        row.Cells.Add(new ShowSubscriptionCellModel()
                        {
                            Channel = channel,
                            Object = SubscriptionObject.ComponentType,
                            ObjectId = componentTypeGroup.ComponentType.Id,
                            Subscription = componentTypeGroup.Subscriptions.SingleOrDefault(x => x.Channel == channel),
                            Row = row
                        });
                    }

                    rows.Add(row);
                }
                model.Rows = rows.ToArray();
            }

            // подписки на компонент
            if (obj == SubscriptionObject.Component)
            {
                var componentGroups = subscriptions.GroupBy(x => x.ComponentId).Select(x => new
                {
                    ComponentId = x.Key,
                    FullName = ComponentHelper.GetComponentPathText(GetStorage().Components.GetOneById(x.Key.GetValueOrDefault()), GetStorage()),
                    Subscriptions = x.ToArray()
                })
                    .OrderBy(x => x.FullName)
                    .ToArray();

                var rows = new List<SubscriptionsTableRowModel>();
                foreach (var componentGroup in componentGroups)
                {
                    var row = new SubscriptionsTableRowModel()
                    {
                        Cells = new List<ShowSubscriptionCellModel>(),
                        Table = model,
                        SubscriptionObject = SubscriptionObject.Component,
                        Text = componentGroup.FullName,
                        ComponentId = componentGroup.ComponentId
                    };

                    foreach (var channel in channels)
                    {
                        row.Cells.Add(new ShowSubscriptionCellModel()
                        {
                            Channel = channel,
                            Object = SubscriptionObject.Component,
                            ObjectId = componentGroup.ComponentId,
                            Subscription = componentGroup.Subscriptions.SingleOrDefault(x => x.Channel == channel),
                            Row = row
                        });
                    }

                    rows.Add(row);
                }
                model.Rows = rows.ToArray();
            }

            return model;
        }

        /// <summary>
        /// Добавление новой подписки
        /// </summary>
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
                CanShowChannel = channel == null,
                CanShowComponentType = componentTypeId == null,
                CanShowComponent = componentId == null,
                Channel = channel ?? SubscriptionChannel.Email,
                IsEnabled = true,
                Color = ColorStatusSelectorValue.FromColor(ObjectColor.Gray),
                MinimumDuration = null,
                ResendTime = null,
                ComponentTypeId = componentTypeId,
                ComponentId = componentId,
                CommonWebsiteUrl = GetConfigDbServicesFactory().GetUrlService().GetCommonWebsiteUrl()
            };
            RestoreEditModel(model);
            return View("Edit", model);
        }

        /// <summary>
        /// Редактирует подписку
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            var subscription = GetStorage().Subscriptions.GetOneById(id);
            var model = new SubscriptionEditModel()
            {
                ModalMode = Request.IsAjaxRequest(),
                ReturnUrl = Url.Action("Index", new { userId = subscription.UserId }),
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
                ResendTime = TimeSpanHelper.FromSeconds(subscription.ResendTimeInSeconds),
                SendOnlyInInterval = subscription.SendOnlyInInterval,
                SendIntervalFrom = subscription.SendIntervalFromHour.HasValue && subscription.SendIntervalFromMinute.HasValue
                    ? new Time() { Hour = subscription.SendIntervalFromHour.Value, Minute = subscription.SendIntervalFromMinute.Value }
                    : (Time?)null,
                SendIntervalTo = subscription.SendIntervalToHour.HasValue && subscription.SendIntervalToMinute.HasValue
                    ? new Time() { Hour = subscription.SendIntervalToHour.Value, Minute = subscription.SendIntervalToMinute.Value }
                    : (Time?)null,
                CommonWebsiteUrl = GetConfigDbServicesFactory().GetUrlService().GetCommonWebsiteUrl()
            };
            RestoreEditModel(model);
            return View(model);
        }

        [HttpPost]
        [CanEditPrivateData]
        public ActionResult Edit(SubscriptionEditModel model)
        {
            RestoreEditModel(model);

            var modelState = new ModelStateHelper<SubscriptionEditModel>(ModelState);

            // Проверка ComponentTypeId
            if (model.Object == SubscriptionObject.ComponentType && model.ComponentTypeId == null)
            {
                modelState.AddErrorFor(x => x.ComponentTypeId, "Выберите тип компонента");
            }

            // Проверка ComponentId
            if (model.Object == SubscriptionObject.Component && model.ComponentId == null)
            {
                modelState.AddErrorFor(x => x.ComponentId, "Выберите компонент");
            }

            // Проверка Channel
            if (model.Id == null && model.Channel == null)
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
                modelState.AddErrorFor(x => x.Color, "Укажите цвет");
            }

            // проверка заполненности интервала отправки, если он включен
            if (model.SendOnlyInInterval && model.IsEnabled)
            {
                if (model.SendIntervalFrom == null)
                    modelState.AddErrorFor(x => x.SendIntervalFrom, "Укажите время");

                if (model.SendIntervalTo == null)
                    modelState.AddErrorFor(x => x.SendIntervalTo, "Укажите время");
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
                        throw new UserFriendlyException("Вы не можете создавать подписки другим пользователям");
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
                        Object = model.Object,
                        SendOnlyInInterval = model.SendOnlyInInterval,
                        SendIntervalFromHour = model.SendIntervalFrom?.Hour,
                        SendIntervalFromMinute = model.SendIntervalFrom?.Minute,
                        SendIntervalToHour = model.SendIntervalTo?.Hour,
                        SendIntervalToMinute = model.SendIntervalTo?.Minute
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
                        Importance = importance,
                        SendOnlyInInterval = model.SendOnlyInInterval,
                        SendIntervalFromHour = model.SendIntervalFrom?.Hour,
                        SendIntervalFromMinute = model.SendIntervalFrom?.Minute,
                        SendIntervalToHour = model.SendIntervalTo?.Hour,
                        SendIntervalToMinute = model.SendIntervalTo?.Minute
                    };
                    var response = client.UpdateSubscription(CurrentUser.AccountId, updateData);
                    response.Check();
                }
                if (model.ModalMode)
                {
                    return GetSuccessJsonResponse(new { subscriptionId = model.Id });
                }
                return Redirect(model.ReturnUrl);
            }
            catch (UserFriendlyException exception)
            {
                model.Exception = exception;
            }

            return View(model);
        }

        private void RestoreEditModel(SubscriptionEditModel model)
        {
            if (model.ComponentTypeId.HasValue)
            {
                model.ComponentTypeDisplayName = GetStorage().ComponentTypes.GetOneById(model.ComponentTypeId.Value).DisplayName;
            }

            if (model.ComponentId.HasValue)
            {
                var service = new ComponentService(GetStorage());
                var component = GetStorage().Components.GetOneById(model.ComponentId.Value);
                model.ComponentDisplayName = service.GetFullDisplayName(component);
            }
        }

        protected void CheckEditingPermissions(SubscriptionForRead subscription)
        {
            if (!CurrentUser.CanManageAccount() && CurrentUser.Id != subscription.UserId)
                throw new NoAccessToPageException();
        }

        [HttpPost]
        [CanEditPrivateData]
        public ActionResult Enable(string ids, bool isEnable, Guid userId, SubscriptionChannel channel)
        {
            var list = ids.Split(',');
            foreach (var item in list)
            {
                var id = new Guid(item);

                var subscription = GetStorage().Subscriptions.GetOneById(id);
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

            var list = ids.Split(',');
            foreach (var item in list)
            {
                var id = new Guid(item);

                var subscription = GetStorage().Subscriptions.GetOneById(id);
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
                    Importance = colors.Length > 0 ? colors[0] : data.Importance,
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
                var subscription = GetStorage().Subscriptions.GetOneById(model.Id);
                CheckEditingPermissions(subscription);

                var client = GetDispatcherClient();
                var response = client.DeleteSubscription(CurrentUser.AccountId, subscription.Id);
                response.Check();

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
            model.Init(GetStorage());

            return PartialView(model);
        }

        public ActionResult EditComponentSubscriptions(
            Guid componentId,
            Guid? userId)
        {
            var component = GetStorage().Components.GetOneById(componentId);
            userId = userId ?? CurrentUser.Id;
            var user = GetStorage().Users.GetOneById(userId.Value);
            var componentType = GetStorage().ComponentTypes.GetOneById(component.ComponentTypeId);
            var userContacts = GetStorage().UserContacts.GetByUserId(user.Id);

            var subscriptions = GetStorage().Subscriptions.GetByUserId(userId.Value)
                .Where(x =>
                    (x.Object == SubscriptionObject.Default
                    || (x.Object == SubscriptionObject.ComponentType && x.ComponentTypeId == component.ComponentTypeId)
                    || (x.Object == SubscriptionObject.Component && x.ComponentId == componentId)))
                .ToArray();

            var model = new EditComponentSubscriptionsModel()
            {
                Component = component,
                ComponentFullName = new ComponentService(GetStorage()).GetFullDisplayName(component),
                ComponentType = componentType,
                User = user,
                UserContacts = userContacts,
                AllSubscriptions = subscriptions,
                CommonWebsiteUrl = GetConfigDbServicesFactory().GetUrlService().GetCommonWebsiteUrl()
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