using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;
using Zidium.UserAccount.Models.Counters;
using Zidium.UserAccount.Models.Metrics;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class MetricsController : BaseController
    {
        public ActionResult Show(Guid id)
        {
            var metric = GetStorage().Metrics.GetOneById(id);
            var metricType = GetStorage().MetricTypes.GetOneById(metric.MetricTypeId);
            var component = GetStorage().Components.GetOneById(metric.ComponentId);
            var bulb = GetStorage().Bulbs.GetOneById(metric.StatusDataId);
            var componentService = new ComponentService(GetStorage(), TimeService);

            var values = GetStorage().MetricHistory.GetLast(metric.ComponentId, metric.MetricTypeId, ShowModel.LastValuesCountMax);

            var actualTimeSecs = metric.ActualTimeSecs ?? metricType.ActualTimeSecs;
            var model = new ShowModel()
            {
                Metric = new ShowModel.MetricInfo()
                {
                    Id = metric.Id,
                    MetricTypeId = metric.MetricTypeId,
                    DisplayName = metricType.DisplayName,
                    Value = metric.Value,
                    Component = new ShowModel.ComponentInfo()
                    {
                        Id = component.Id,
                        FullName = componentService.GetFullDisplayName(component)
                    },
                    Bulb = new ShowModel.BulbInfo()
                    {
                        Status = bulb.Status,
                        ActualDate = bulb.ActualDate,
                        StartDate = bulb.StartDate,
                        Count = bulb.Count,
                        Duration = bulb.GetDuration(Now()),
                        EndDate = bulb.EndDate
                    }
                },
                ConditionRed = metric.ConditionAlarm ?? metricType.ConditionAlarm ?? "не задано",
                ConditionYellow = metric.ConditionWarning ?? metricType.ConditionWarning ?? "не задано",
                ConditionGreen = metric.ConditionSuccess ?? metricType.ConditionSuccess ?? "не задано",
                ElseColor = metric.ConditionElseColor ?? metricType.ConditionElseColor,
                NoSignalColor = metric.NoSignalColor ?? metricType.NoSignalColor,
                ActualInterval = actualTimeSecs.HasValue ? TimeSpan.FromSeconds(actualTimeSecs.Value) : (TimeSpan?)null,
                Values = values,
                MetricBreadCrumbs = MetricBreadCrumbsModel.Create(metric.Id, GetStorage())
            };

            return View(model);
        }

        [CanEditAllData]
        public ActionResult Edit(Guid? id, Guid? componentId, Guid? metricTypeId)
        {
            var model = new EditModel()
            {
                ComponentId = componentId,
                MetricTypeId = metricTypeId
            };
            if (id.HasValue)
            {
                var metric = GetStorage().Metrics.GetOneById(id.Value);
                model.Id = metric.Id;
                model.ComponentId = metric.ComponentId;
                model.MetricTypeId = metric.MetricTypeId;
                model.ConditionRed = metric.ConditionAlarm;
                model.ConditionYellow = metric.ConditionWarning;
                model.ConditionGreen = metric.ConditionSuccess;
                model.ElseColor = ColorStatusSelectorValue.FromColor(metric.ConditionElseColor);
                model.ActualTime = TimeSpanHelper.FromSeconds(metric.ActualTimeSecs);
            }
            RestoreEditModel(model);

            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditModel model)
        {
            RestoreEditModel(model);

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (model.Id == null)
                {
                    var oldMetric = GetStorage().Metrics.GetByComponentId(model.ComponentId.Value).FirstOrDefault(x => x.MetricTypeId == model.MetricTypeId);
                    if (oldMetric != null)
                    {
                        ModelState.AddModelError("MetricTypeId", "Метрика данного типа уже есть у компонента");
                        return View(model);
                    }
                }

                var client = GetDispatcherClient();
                Guid metricId;

                if (model.Id.HasValue)
                {
                    metricId = model.Id.Value;
                    var updateData = new UpdateMetricRequestData()
                    {
                        MetricId = model.Id.Value,
                        AlarmCondition = model.ConditionRed,
                        WarningCondition = model.ConditionYellow,
                        SuccessCondition = model.ConditionGreen,
                        ElseColor = model.ElseColor.GetSelectedOne(),
                        ActualTimeSecs = TimeSpanHelper.GetSeconds(model.ActualTime),
                        NoSignalColor = model.NoSignalColor.GetSelectedOne()
                    };
                    client.UpdateMetric(updateData).Check();
                }
                else
                {
                    var metricType = GetStorage().MetricTypes.GetOneById(model.MetricTypeId.Value);
                    var createData = new CreateMetricRequestData()
                    {
                        ComponentId = model.ComponentId.Value,
                        MetricName = metricType.SystemName,//todo сделать отдельную версию, где передается MetricTypeId
                        AlarmCondition = model.ConditionRed,
                        WarningCondition = model.ConditionYellow,
                        SuccessCondition = model.ConditionGreen,
                        ElseColor = model.ElseColor.GetSelectedOne(),
                        NoSignalColor = model.NoSignalColor.GetSelectedOne(),
                        ActualTimeSecs = TimeSpanHelper.GetSeconds(model.ActualTime)
                    };
                    metricId = client.CreateMetric(createData).Data.MetricId;
                }

                if (!Request.IsSmartBlocksRequest())
                {
                    return RedirectToAction("Show", new { id = metricId });
                }

                return GetSuccessJsonResponse(metricId);
            }
            catch (UserFriendlyException exception)
            {
                SetCommonError(exception);
                return View(model);
            }
        }

        private void RestoreEditModel(EditModel model)
        {
            if (model.Id.HasValue)
            {
                if (model.MetricTypeId.HasValue)
                    model.MetricType = GetStorage().MetricTypes.GetOneById(model.MetricTypeId.Value);
                model.MetricBreadCrumbs = MetricBreadCrumbsModel.Create(model.Id.Value, GetStorage());
            }
        }

        public ActionResult Values(Guid? metricTypeId, Guid? componentId, ColorStatusSelectorValue color)
        {
            var statuses = color.Checked ? color.GetSelectedMonitoringStatuses() : null;

            var metrics = GetStorage().Metrics.Filter(metricTypeId, componentId, statuses, 100);
            var metricTypes = GetStorage().MetricTypes.GetMany(metrics.Select(t => t.MetricTypeId).Distinct().ToArray()).ToDictionary(a => a.Id, b => b);
            var bulbs = GetStorage().Bulbs.GetMany(metrics.Select(t => t.StatusDataId).ToArray()).ToDictionary(a => a.Id, b => b);

            var model = new ValuesModel()
            {
                Color = color,
                MetricTypeId = metricTypeId,
                ComponentId = componentId,
                Items = metrics.Select(t => new ValuesModel.MetricInfo()
                {
                    Id = t.Id,
                    Value = t.Value,
                    DisplayName = metricTypes[t.MetricTypeId].DisplayName,
                    Status = bulbs[t.StatusDataId].Status,
                    ActualDate = t.ActualDate,
                    BeginDate = t.BeginDate,
                    ComponentId = t.ComponentId,
                    ComponentBreadCrumbs = ComponentBreadCrumbsModel.Create(t.ComponentId, GetStorage())
                }).ToArray()
            };

            return View(model);
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var metric = GetStorage().Metrics.GetOneById(id);
            var metricType = GetStorage().MetricTypes.GetOneById(metric.MetricTypeId);

            var model = new DeleteConfirmationAjaxModel()
            {
                Title = "Удаление метрики",
                Message = "Вы действительно хотите удалить метрику " + metricType.DisplayName + "?",
            };
            return View("Dialogs/DeleteConfirmationAjax", model);
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult Delete(Guid id, string fake)
        {
            try
            {
                var dispatcher = GetDispatcherClient();
                var data = new DeleteMetricRequestData()
                {
                    MetricId = id,
                    UpdateComponentStatus = true
                };
                dispatcher.DeleteMetric(data).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception, Logger);
                return GetErrorJsonResponse(exception);
            }
        }

        [CanEditAllData]
        public ActionResult Disable(Guid id)
        {
            var model = new DisableDialogGetModel()
            {
                Title = "Выключить метрику",
                Message = "На какое время выключить метрику?"
            };
            return View("Dialogs/DisableDialogAjax", model);
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult Enable(Guid id)
        {
            try
            {
                var client = GetDispatcherClient();
                client.SetMetricEnable(id).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception, Logger);
                return GetErrorJsonResponse(exception);
            }
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult Disable(DisableDialogPostModel model)
        {
            try
            {
                var client = GetDispatcherClient();
                var date = model.GetDate();
                var data = new SetMetricDisableRequestData()
                {
                    Comment = model.Comment,
                    ToDate = date,
                    MetricId = model.Id
                };
                client.SetMetricDisable(data).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception, Logger);
                return GetErrorJsonResponse(exception);
            }
        }

        public ActionResult SetActualValue(Guid id)
        {
            var metric = GetStorage().Metrics.GetOneById(id);
            var metricType = GetStorage().MetricTypes.GetOneById(metric.MetricTypeId);

            var model = new SetActualValueModel()
            {
                Id = metric.Id,
                MetricTypeName = metricType.DisplayName
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult SetActualValue(SetActualValueModel model)
        {
            try
            {
                var metric = GetStorage().Metrics.GetOneById(model.Id);
                var metricType = GetStorage().MetricTypes.GetOneById(metric.MetricTypeId);

                var client = GetDispatcherClient();

                // парсим число
                double? value = null;
                if (model.Value != null)
                {
                    try
                    {
                        value = ParseHelper.ParseDouble(model.Value);
                    }
                    catch (Exception)
                    {
                        throw new UserFriendlyException("Не удалось получить число из строки: " + model.Value);
                    }
                }

                var data = new SendMetricRequestDataDto()
                {
                    ComponentId = metric.ComponentId,
                    Name = metricType.SystemName,
                    Value = value
                };
                if (model.ActualTime.HasValue)
                {
                    data.ActualIntervalSecs = model.ActualTime.Value.TotalSeconds;
                }
                client.SendMetric(data).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception, Logger);
                return GetErrorJsonResponse(exception);
            }
        }

        [CanEditAllData]
        public ActionResult DeleteAjax(Guid id)
        {
            var metric = GetStorage().Metrics.GetOneById(id);
            var metricType = GetStorage().MetricTypes.GetOneById(metric.MetricTypeId);

            var model = new DeleteDialogAjaxModel()
            {
                Id = metric.Id,
                Message = "Вы действительно хотите удалить метрику " + metricType.DisplayName + "?"
            };
            return PartialView("Dialogs/DeleteDialogAjaxNew", model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult DeleteAjax(DeleteDialogAjaxModel model)
        {
            var client = GetDispatcherClient();
            client.DeleteMetric(new DeleteMetricRequestData()
            {
                MetricId = model.Id,
                UpdateComponentStatus = true
            }).Check();
            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        public ActionResult DisableAjax(Guid id)
        {
            var model = new DisableDialogAjaxModel()
            {
                Id = id,
                Message = "На какое время выключить метрику?",
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
                date = Now().AddHours(1);
            else if (model.Interval == DisableDialogAjaxModel.DisableInterval.Day)
                date = Now().AddDays(1);
            else if (model.Interval == DisableDialogAjaxModel.DisableInterval.Week)
                date = Now().AddDays(7);
            else
                date = model.Date;

            var client = GetDispatcherClient();
            var data = new SetMetricDisableRequestData()
            {
                Comment = model.Comment,
                ToDate = date,
                MetricId = model.Id
            };
            client.SetMetricDisable(data).Check();

            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult EnableAjax(Guid id)
        {
            var client = GetDispatcherClient();
            client.SetMetricEnable(id).Check();
            return GetSuccessJsonResponse();
        }

        public MetricsController(ILogger<MetricsController> logger) : base(logger)
        {
        }

    }
}
