using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class UnitTestsController : ContextController
    {
        public ActionResult Index(
            Guid? componentId = null,
            Guid? componentTypeId = null,
            Guid? unitTestTypeId = null,
            ColorStatusSelectorValue color = null)
        {
            var repository = CurrentAccountDbContext.GetUnitTestRepository();
            if (color == null)
            {
                color = new ColorStatusSelectorValue();
            }

            var statuses = color.GetSelectedMonitoringStatuses();

            var query = repository.QueryForGui(componentTypeId,
                componentId,
                unitTestTypeId,
                statuses)
                .Include("UnitTestType").Include("Component").Include("Bulb")
                .GroupBy(t => t.Type);

            var items = query.Select(t => new UnitTestsListItemLevel1Model()
            {
                UnitTestType = t.Key,
                UnitTests = t.Select(x => new UnitTestsListItemLevel2Model()
                {
                    Id = x.Id,
                    Component = x.Component,
                    Date = x.Bulb.EndDate,
                    DisplayName = x.DisplayName ?? t.Key.DisplayName,
                    Message = x.Bulb.Message,
                    Result = x.Bulb.Status,
                    HasBanner = x.HttpRequestUnitTest != null && x.HttpRequestUnitTest.HasBanner
                }).OrderBy(x => x.DisplayName).ToList()
            })
            .OrderBy(t => t.UnitTestType.DisplayName)
            .ToList();

            var model = new UnitTestsListModel()
            {
                AccountId = CurrentUser.AccountId,
                ComponentId = componentId,
                UnitTestTypeId = unitTestTypeId,
                UnitTestTypes = items,
                Color = color
            };
            return View(model);
        }

        [CanEditAllData]
        public ActionResult Add(Guid? componentId = null)
        {
            // зарегистрируем дефолтный тип пльзовательской проверки
            // чтобы в выпадающем списке всегда была хотя был хотя бы один тип пользовательской проверки
            var dispatcher = GetDispatcherClient();
            dispatcher.GetOrCreateUnitTestType(CurrentUser.AccountId, new GetOrCreateUnitTestTypeRequestData()
            {
                SystemName = "CustomUnitTestType",
                DisplayName = "Пользовательская проверка"
            });

            var model = new UnitTestAddModel();
            if (componentId.HasValue)
            {
                var componentRepository = CurrentAccountDbContext.GetComponentRepository();
                var component = componentRepository.GetById(componentId.Value);
                model.ComponentId = component.Id;
            }
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(UnitTestAddModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var componentRepository = CurrentAccountDbContext.GetComponentRepository();
            var component = componentRepository.GetById(model.ComponentId.Value);

            var unitTestTypeRepository = CurrentAccountDbContext.GetUnitTestTypeRepository();
            var unitTestType = unitTestTypeRepository.GetById(model.UnitTestTypeId);

            var client = GetDispatcherClient();
            var data = new GetOrCreateUnitTestRequestData()
            {
                ComponentId = component.Id,
                UnitTestTypeId = unitTestType.Id,
                DisplayName = model.DisplayName,
                SystemName = model.DisplayName
            };

            var response = client.GetOrCreateUnitTest(CurrentUser.AccountId, data);
            var unitTest = response.Data;

            this.SetTempMessage(TempMessageType.Success, string.Format("Добавлена проверка <a href='{1}' class='alert-link'>{0}</a>", unitTest.DisplayName, Url.Action("ResultDetails", new { id = unitTest.Id })));
            return RedirectToAction("Index");
        }

        [CanEditAllData]
        public ActionResult Edit(Guid id)
        {
            var unitTest = GetUnitTestById(id);

            var model = new UnitTestEditModel()
            {
                Id = unitTest.Id,
                ComponentId = unitTest.Component.Id,
                Date = unitTest.Bulb.EndDate,
                DisplayName = unitTest.DisplayName,
                IsDeleted = unitTest.IsDeleted,
                Message = unitTest.Bulb.Message,
                PeriodSeconds = unitTest.PeriodSeconds,
                ActualTime = TimeSpanHelper.FromSeconds(unitTest.ActualTimeSecs),
                NoSignalColor = ColorStatusSelectorValue.FromColor(unitTest.NoSignalColor),
                Status = unitTest.Bulb.Status,
                UnitTestType = unitTest.Type,
                UnitTest = unitTest
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UnitTestEditModel model)
        {
            model.UnitTest = GetUnitTestById(model.Id);

            if (!ModelState.IsValid)
                View(model);

            var updateData = new UpdateUnitTestRequestData()
            {
                NoSignalColor = model.NoSignalColor.GetSelectedOne(),
                ActualTime = TimeSpanHelper.GetSeconds(model.ActualTime),
                DisplayName = model.DisplayName,
                ComponentId = model.ComponentId,
                PeriodSeconds = model.PeriodSeconds,
                UnitTestId = model.Id,
                SystemName = model.UnitTest.SystemName
            };
            var dispatcher = GetDispatcherClient();
            var response = dispatcher.UpdateUnitTest(CurrentUser.AccountId, updateData);
            if (response.Success)
            {
                return RedirectToAction("ResultDetails", new { id = model.Id });
            }
            SetCommonError(response.ErrorMessage);
            return View(model);
        }

        public ActionResult ResultDetails(Guid id)
        {
            var model = new UnitTestResultModel();
            model.Init(id, CurrentAccountDbContext);
            return View(model);
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var unitTest = GetUnitTestById(id);
            var model = new DeleteConfirmationAjaxModel()
            {
                Title = "Удаление проверки",
                Message = "Вы действительно хотите удалить проверку " + unitTest.DisplayName + "?",
            };
            return View("Dialogs/DeleteConfirmationAjax", model);
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult Delete(Guid id, string fake)
        {
            try
            {
                var client = GetDispatcherClient();
                client.DeleteUnitTest(CurrentUser.AccountId, id).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
                return GetErrorJsonResponse(exception);
            }
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult Enable(Guid id)
        {
            try
            {
                var client = GetDispatcherClient();
                var response = client.SetUnitTestEnable(CurrentUser.AccountId, id);
                response.Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
                return GetErrorJsonResponse(exception);
            }
        }

        [CanEditAllData]
        public ActionResult Disable(Guid id)
        {
            var model = new DisableDialogGetModel()
            {
                Title = "Выключить проверку",
                Message = "На какое время выключить проверку?"
            };
            return View("Dialogs/DisableDialogAjax", model);
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult Disable(DisableDialogPostModel model)
        {
            try
            {
                var client = GetDispatcherClient();
                var date = model.GetDate();
                var data = new SetUnitTestDisableRequestData()
                {
                    Comment = model.Comment,
                    ToDate = date,
                    UnitTestId = model.Id
                };
                client.SetUnitTestDisable(CurrentUser.AccountId, data).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
                return GetErrorJsonResponse(exception);
            }
        }

        [CanEditAllData]
        public ActionResult SetResult(Guid id)
        {
            var unitTest = GetUnitTestById(id);
            var model = new UnitTestSetResultModel()
            {
                Id = unitTest.Id,
                Result = UnitTestResult.Success,
                ActualInterval = TimeSpan.FromMinutes(1),
                DisplayName = unitTest.DisplayName
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult SetResult(UnitTestSetResultModel model)
        {
            try
            {
                var unitTest = GetUnitTestById(model.Id);
                var client = GetDispatcherClient();
                var response = client.SendUnitTestResult(CurrentUser.AccountId, new SendUnitTestResultRequestData()
                {
                    UnitTestId = unitTest.Id,
                    Result = model.Result,
                    Message = model.Message,
                    ActualIntervalSeconds = model.ActualInterval.TotalSeconds
                });
                response.Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
                return GetErrorJsonResponse(exception);
            }
        }

        [CanEditAllData]
        public ActionResult DeleteAjax(Guid id)
        {
            var unittest = GetUnitTestById(id);

            var model = new DeleteDialogAjaxModel()
            {
                Id = unittest.Id,
                Message = "Вы действительно хотите удалить проверку " + unittest.DisplayName + "?"
            };
            return PartialView("Dialogs/DeleteDialogAjaxNew", model);
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult DeleteAjax(DeleteDialogAjaxModel model)
        {
            var client = GetDispatcherClient();
            client.DeleteUnitTest(CurrentUser.AccountId, model.Id).Check();
            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        public ActionResult DisableAjax(Guid id)
        {
            var unittest = GetUnitTestById(id);

            var model = new DisableDialogAjaxModel()
            {
                Id = unittest.Id,
                Message = "На какое время выключить проверку?",
                Interval = DisableDialogAjaxModel.DisableInterval.Forever
            };
            return PartialView("Dialogs/DisableDialogAjaxNew", model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult DisableAjax(DisableDialogAjaxModel model)
        {
            if (model.Interval == DisableDialogAjaxModel.DisableInterval.Custom && !model.Date.HasValue)
                ModelState.AddModelError("Date", "Пожалуйста, укажите дату");

            if (!ModelState.IsValid)
                return PartialView("Dialogs/DisableDialogAjaxNew", model);

            DateTime? date;

            if (model.Interval == DisableDialogAjaxModel.DisableInterval.Hour)
                date = MvcApplication.GetServerDateTime().AddHours(1);
            else if (model.Interval == DisableDialogAjaxModel.DisableInterval.Day)
                date = MvcApplication.GetServerDateTime().AddDays(1);
            else if (model.Interval == DisableDialogAjaxModel.DisableInterval.Week)
                date = MvcApplication.GetServerDateTime().AddDays(7);
            else
                date = model.Date;

            var client = GetDispatcherClient();
            var data = new SetUnitTestDisableRequestData()
            {
                Comment = model.Comment,
                ToDate = date,
                UnitTestId = model.Id
            };
            client.SetUnitTestDisable(CurrentUser.AccountId, data).Check();

            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult EnableAjax(Guid id)
        {
            var client = GetDispatcherClient();
            client.SetUnitTestEnable(CurrentUser.AccountId, id).Check();
            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult RunNowAjax(Guid id)
        {
            var client = GetDispatcherClient();
            client.SetUnitTestNextTime(CurrentUser.AccountId, new SetUnitTestNextTimeRequestData()
            {
                UnitTestId = id,
                NextTime = MvcApplication.GetServerDateTime()
            }).Check();
            return GetSuccessJsonResponse();
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        public UnitTestsController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public UnitTestsController() { }
    }
}