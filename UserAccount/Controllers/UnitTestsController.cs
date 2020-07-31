using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;
using Zidium.UserAccount.Models.UnitTests;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class UnitTestsController : BaseController
    {
        public ActionResult Index(
            Guid? componentId = null,
            Guid? componentTypeId = null,
            Guid? unitTestTypeId = null,
            ColorStatusSelectorValue color = null)
        {
            if (color == null)
            {
                color = new ColorStatusSelectorValue();
            }

            var statuses = color.GetSelectedMonitoringStatuses();

            var unittests = GetStorage().UnitTests.Filter(componentTypeId,
                componentId,
                unitTestTypeId,
                statuses, 100);

            var components = GetStorage().Components.GetMany(unittests.Select(t => t.ComponentId).Distinct().ToArray()).ToDictionary(a => a.Id, b => b);
            var bulbs = GetStorage().Bulbs.GetMany(unittests.Select(t => t.StatusDataId).ToArray()).ToDictionary(a => a.Id, b => b);

            var groupedUnittests = unittests.GroupBy(t => t.TypeId).ToArray();
            var unittestTypes = GetStorage().UnitTestTypes.GetMany(groupedUnittests.Select(t => t.Key).ToArray()).ToDictionary(a => a.Id, b => b);

            var items = groupedUnittests.Select(t =>
                {
                    var unittestType = unittestTypes[t.Key];
                    return new UnitTestsListItemLevel1Model()
                    {
                        UnitTestType = unittestType,
                        UnitTests = t.Select(x =>
                        {
                            var bulb = bulbs[x.StatusDataId];

                            var httpUnitTest = x.TypeId == SystemUnitTestType.HttpUnitTestType.Id
                                ? GetStorage().HttpRequestUnitTests.GetOneOrNullByUnitTestId(x.Id)
                                : null;

                            return new UnitTestsListItemLevel2Model()
                            {
                                Id = x.Id,
                                Component = components[x.ComponentId],
                                Date = bulb.EndDate,
                                DisplayName = x.DisplayName ?? unittestType.DisplayName,
                                Message = bulb.Message,
                                Result = bulb.Status,
                                HasBanner = httpUnitTest?.HasBanner ?? false
                            };
                        }).OrderBy(x => x.DisplayName).ToList()
                    };
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
            // TODO Move to database registrator
            var dispatcher = GetDispatcherClient();
            dispatcher.GetOrCreateUnitTestType(CurrentUser.AccountId, new GetOrCreateUnitTestTypeRequestData()
            {
                SystemName = "CustomUnitTestType",
                DisplayName = "Пользовательская проверка"
            });

            var model = new UnitTestAddModel();
            if (componentId.HasValue)
            {
                model.ComponentId = componentId.Value;
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

            var component = GetStorage().Components.GetOneById(model.ComponentId.Value);

            var unitTestType = GetStorage().UnitTestTypes.GetOneById(model.UnitTestTypeId);

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

            if (!Request.IsSmartBlocksRequest())
            {
                this.SetTempMessage(TempMessageType.Success, string.Format("Добавлена проверка <a href='{1}' class='alert-link'>{0}</a>", unitTest.DisplayName, Url.Action("ResultDetails", new { id = unitTest.Id })));
                return RedirectToAction("Index");
            }

            return GetSuccessJsonResponse(unitTest.Id);
        }

        [CanEditAllData]
        public ActionResult Edit(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);

            // данная страница должна редактировать только пользовательские проверки
            if (SystemUnitTestType.IsSystem(unitTest.TypeId))
            {
                throw new Exception("Проверка не является пользовательской");
            }

            var model = new UnitTestEditModel()
            {
                Id = unitTest.Id,
                ComponentId = unitTest.ComponentId,
                DisplayName = unitTest.DisplayName,
                IsDeleted = unitTest.IsDeleted,
                PeriodSeconds = unitTest.PeriodSeconds,
                ActualTime = TimeSpanHelper.FromSeconds(unitTest.ActualTimeSecs),
                NoSignalColor = ColorStatusSelectorValue.FromColor(unitTest.NoSignalColor)
            };
            RestoreEditModel(model);
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UnitTestEditModel model)
        {
            RestoreEditModel(model);

            if (!ModelState.IsValid)
                return View(model);

            var updateData = new UpdateUnitTestRequestData()
            {
                NoSignalColor = model.NoSignalColor.GetSelectedOne(),
                ActualTime = TimeSpanHelper.GetSeconds(model.ActualTime),
                DisplayName = model.DisplayName,
                ComponentId = model.ComponentId,
                PeriodSeconds = model.PeriodSeconds,
                UnitTestId = model.Id
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


        private void RestoreEditModel(UnitTestEditModel model)
        {
            var unittest = GetStorage().UnitTests.GetOneById(model.Id);
            model.UnitTestType = GetStorage().UnitTestTypes.GetOneById(unittest.TypeId);
            model.NoSignalColorDefault = model.UnitTestType.NoSignalColor ?? ObjectColor.Red;
            model.ActualTimeDefault = TimeSpanHelper.FromSeconds(model.UnitTestType.ActualTimeSecs) ?? UnitTestHelper.GetDefaultActualTime();

            var bulb = GetStorage().Bulbs.GetOneById(unittest.StatusDataId);
            model.Date = bulb.EndDate;
            model.Message = bulb.Message;
            model.Status = bulb.Status;
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

        private EventForRead GetUnitTestLastResultEvent(UnitTestForRead unitTest)
        {
            var statusId = GetStorage().Bulbs.GetOneById(unitTest.StatusDataId).StatusEventId;
            if (statusId != Guid.Empty)
            {
                var status = GetStorage().Events.GetOneById(statusId);
                if (status.FirstReasonEventId != null)
                {
                    var firstReason = GetStorage().Events.GetByOwnerIdAndLastStatusEventId(unitTest.Id, status.Id);
                    return firstReason;
                }
            }
            return null;
        }

        public ActionResult ResultDetails(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var unittestType = GetStorage().UnitTestTypes.GetOneById(unitTest.TypeId);
            var bulb = GetStorage().Bulbs.GetOneById(unitTest.StatusDataId);
            var component = GetStorage().Components.GetOneById(unitTest.ComponentId);

            string lastRunErrorMessage = null;

            if (unittestType.Id == SystemUnitTestType.HttpUnitTestType.Id)
            {
                var rule = GetStorage().HttpRequestUnitTestRules.GetByUnitTestId(id).FirstOrDefault();
                lastRunErrorMessage = rule?.LastRunErrorMessage;
            }

            var model = new UnitTestResult2Model()
            {
                UnitTestType = unittestType,
                UnitTestBreadCrumbs = UnitTestBreadCrumbsModel.Create(id, GetStorage()),
                Now = MvcApplication.GetServerDateTime(),
                UnitTestBulb = bulb,
                Component = component,
                LastRunErrorMessage = lastRunErrorMessage
            };

            model.Init(unitTest);

            return View("ResultDetails2", model);
        }

        public ActionResult OverviewCurrentStatus(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = OverviewCurrentStatusModel.Create(unitTest, GetStorage());
            return PartialView("OverviewCurrentStatus", model);
        }

        public ActionResult OverviewLastResult(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);

            // последний результат
            var lastResult = GetUnitTestLastResultEvent(unitTest);

            var model = OverviewLastResultModel.Create(unitTest, lastResult, GetStorage());
            return PartialView(model);
        }

        public ActionResult OverviewSettingsHttp(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = OverviewSettingsHttpModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult OverviewSettingsPing(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = OverviewSettingsPingModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult OverviewSettingsVirusTotal(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = OverviewSettingsVirusTotalModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult OverviewSettingsTcpPort(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = OverviewSettingsTcpPortModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult OverviewSettingsDomain(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = OverviewSettingsDomainModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult OverviewSettingsSsl(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = OverviewSettingsSslModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult OverviewSettingsSql(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = OverviewSettingsSqlModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult OverviewSettingsCustom(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = OverviewSettingsCustomModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult OverviewStatusDiagram(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = OverviewStatusDiagramModel.Create(unitTest);
            return PartialView(model);
        }

        public ActionResult ShowSettings(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = ShowSettingsModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult ShowSettingsHttp(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = ShowSettingsHttpModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult ShowSettingsPing(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = ShowSettingsPingModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult ShowSettingsVirusTotal(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = ShowSettingsVirusTotalModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult ShowSettingsTcpPort(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = ShowSettingsTcpPortModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult ShowSettingsDomain(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = ShowSettingsDomainModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult ShowSettingsSsl(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = ShowSettingsSslModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult ShowSettingsSql(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = ShowSettingsSqlModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult ShowSettingsCustom(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
            var model = ShowSettingsCustomModel.Create(unitTest, GetStorage());
            return PartialView(model);
        }

        public ActionResult ShowStatuses(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);

            var model = new ShowStatusesModel()
            {
                UnitTestId = unitTest.Id,
                MaxCount = 20
            };

            var resultEvents = GetStorage().Events.GetLastEvents(unitTest.Id, EventCategory.UnitTestStatus, model.MaxCount);

            var now = Now();
            model.Statuses = resultEvents.Select(t => new ShowStatusesModel.UnitTestStatusEvent
            {
                Date = t.StartDate,
                Duration = EventHelper.GetDuration(t.StartDate, t.ActualDate, now),
                EndDate = t.EndDate,
                Count = t.Count,
                Importance = t.Importance,
                Message = t.GetUnitTestMessage(),
                Id = t.Id,
                UnitTestId = unitTest.Id
            }).ToArray();

            return PartialView(model);
        }

        public ActionResult ShowExecutionResults(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);

            var model = new ShowExecutionResultsModel()
            {
                UnitTestId = unitTest.Id,
                MaxCount = 20
            };

            var resultEvents = GetStorage().Events.GetLastEvents(unitTest.Id, EventCategory.UnitTestResult, model.MaxCount);

            model.ExecutionResults = resultEvents.Select(t => new UnitTestResultEventDto
            {
                Date = t.StartDate,
                Importance = t.Importance,
                Message = t.GetUnitTestMessage(),
                Id = t.Id,
                UnitTestId = unitTest.Id
            }).ToArray();

            return PartialView(model);
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var unitTest = GetStorage().UnitTests.GetOneById(id);
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
            var unitTest = GetStorage().UnitTests.GetOneById(id);
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
                var unitTest = GetStorage().UnitTests.GetOneById(model.Id);
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
            var unitTest = GetStorage().UnitTests.GetOneById(id);

            var model = new DeleteDialogAjaxModel()
            {
                Id = unitTest.Id,
                Message = "Вы действительно хотите удалить проверку " + unitTest.DisplayName + "?"
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
            var unitTest = GetStorage().UnitTests.GetOneById(id);

            var model = new DisableDialogAjaxModel()
            {
                Id = unitTest.Id,
                Message = "На какое время выключить проверку" + unitTest.DisplayName + "?",
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

        // не ипользуется
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