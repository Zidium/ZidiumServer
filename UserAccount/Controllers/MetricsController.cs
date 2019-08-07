using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
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
    public class MetricsController : ContextController
    {
        public ActionResult Show(Guid id)
        {
            var metricRepository = CurrentAccountDbContext.GetMetricRepository();
            var metric = metricRepository.GetById(id);

            var storageContext = CurrentAccountDbContext;
            var metricHistoryRepository = storageContext.GetMetricHistoryRepository();
            var values = metricHistoryRepository.QueryAllByMetricType(metric.ComponentId, metric.MetricTypeId)
                .OrderByDescending(t => t.BeginDate)
                .Take(ShowModel.LastValuesCountMax)
                .ToList();

            var actualTimeSecs = metric.ActualTimeSecs ?? metric.MetricType.ActualTimeSecs;
            var model = new ShowModel()
            {
                Metric = metric,
                ConditionRed = metric.ConditionAlarm ?? metric.MetricType.ConditionAlarm ?? "не задано",
                ConditionYellow = metric.ConditionWarning ?? metric.MetricType.ConditionWarning ?? "не задано",
                ConditionGreen = metric.ConditionSuccess ?? metric.MetricType.ConditionSuccess ?? "не задано",
                ElseColor = metric.ConditionElseColor ?? metric.MetricType.ConditionElseColor,
                NoSignalColor = metric.NoSignalColor ?? metric.MetricType.NoSignalColor,
                ActualInterval = actualTimeSecs.HasValue ? TimeSpan.FromSeconds(actualTimeSecs.Value) : (TimeSpan?)null,
                Values = values
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
                var metric = GetMetricById(id.Value);
                if (metric != null)
                {
                    model.Metric = metric;
                    model.MetricType = metric.MetricType;
                    model.Id = metric.Id;
                    model.ComponentId = metric.ComponentId;
                    model.MetricTypeId = metric.MetricTypeId;
                    model.ConditionRed = metric.ConditionAlarm;
                    model.ConditionYellow = metric.ConditionWarning;
                    model.ConditionGreen = metric.ConditionSuccess;
                    model.ElseColor = ColorStatusSelectorValue.FromColor(metric.ConditionElseColor);
                    model.ActualTime = TimeSpanHelper.FromSeconds(metric.ActualTimeSecs);
                }
            }
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var component = GetComponentById(model.ComponentId.Value);
                if (model.Id == null)
                {
                    var oldMetric = component.Metrics.FirstOrDefault(x => x.IsDeleted == false && x.MetricTypeId == model.MetricTypeId);
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
                    client.UpdateMetric(CurrentUser.AccountId, updateData).Check();
                }
                else
                {
                    var metricType = GetMetricTypeById(model.MetricTypeId.Value);
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
                    metricId = client.CreateMetric(CurrentUser.AccountId, createData).Data.MetricId;
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
                if (model.Id.HasValue)
                {
                    model.Metric = GetMetricById(model.Id.Value);
                }
                return View(model);
            }
        }

        public ActionResult Values(Guid? metricTypeId, Guid? componentId, ColorStatusSelectorValue color)
        {
            var repository = CurrentAccountDbContext.GetMetricRepository();
            var query = repository.QueryAll()
                .Include("Component")
                .Include("MetricType");

            if (metricTypeId.HasValue)
                query = query.Where(t => t.MetricTypeId == metricTypeId.Value);

            if (componentId.HasValue)
                query = query.Where(t => t.ComponentId == componentId.Value);
            
            if (color.Checked)
            {
                var colors = color.GetSelectedMonitoringStatuses();
                query = query.Where(t => colors.Contains(t.Bulb.Status));
            }
            query = query.OrderBy(t => t.Component.DisplayName).ThenBy(t => t.MetricType.DisplayName);

            var model = new ValuesModel()
            {
                Color = color,
                MetricTypeId = metricTypeId,
                ComponentId = componentId,
                Items = query
            };

            return View(model);
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var metric = GetMetricById(id);
            var model = new DeleteConfirmationAjaxModel()
            {
                Title = "Удаление метрики",
                Message = "Вы действительно хотите удалить метрику " + metric.MetricType.SystemName + "?",
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
                dispatcher.DeleteMetric(CurrentUser.AccountId, data).Check();
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
                client.SetMetricEnable(CurrentUser.AccountId, id).Check();
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
                client.SetMetricDisable(CurrentUser.AccountId, data).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
                return GetErrorJsonResponse(exception);
            }
        }

        public ActionResult SetActualValue(Guid id)
        {
            var metric = GetMetricById(id);
            var model = new SetActualValueModel()
            {
                Id = metric.Id,
                MetricTypeName = metric.MetricType.DisplayName
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult SetActualValue(SetActualValueModel model)
        {
            try
            {
                var metric = GetMetricById(model.Id);
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

                var data = new SendMetricRequestData()
                {
                    ComponentId = metric.ComponentId,
                    Name = metric.MetricType.SystemName,
                    Value = value
                };
                if (model.ActualTime.HasValue)
                {
                    data.ActualIntervalSecs = model.ActualTime.Value.TotalSeconds;
                }
                client.SendMetric(CurrentUser.AccountId, data).Check();
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
            var metric = GetMetricById(id);
            var model = new DeleteDialogAjaxModel()
            {
                Id = metric.Id,
                Message = "Вы действительно хотите удалить метрику " + metric.MetricType.DisplayName + "?"
            };
            return PartialView("Dialogs/DeleteDialogAjaxNew", model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult DeleteAjax(DeleteDialogAjaxModel model)
        {
            var client = GetDispatcherClient();
            client.DeleteMetric(CurrentUser.AccountId, new DeleteMetricRequestData()
            {
                MetricId = model.Id,
                UpdateComponentStatus = true
            }).Check();
            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        public ActionResult DisableAjax(Guid id)
        {
            var metric = GetMetricById(id);
            var model = new DisableDialogAjaxModel()
            {
                Id = metric.Id,
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
                date = MvcApplication.GetServerDateTime().AddHours(1);
            else if (model.Interval == DisableDialogAjaxModel.DisableInterval.Day)
                date = MvcApplication.GetServerDateTime().AddDays(1);
            else if (model.Interval == DisableDialogAjaxModel.DisableInterval.Week)
                date = MvcApplication.GetServerDateTime().AddDays(7);
            else
                date = model.Date;

            var client = GetDispatcherClient();
            var data = new SetMetricDisableRequestData()
            {
                Comment = model.Comment,
                ToDate = date,
                MetricId = model.Id
            };
            client.SetMetricDisable(CurrentUser.AccountId, data).Check();

            return GetSuccessJsonResponse();
        }

        [CanEditAllData]
        [HttpPost]
        public JsonResult EnableAjax(Guid id)
        {
            var client = GetDispatcherClient();
            client.SetMetricEnable(CurrentUser.AccountId, id).Check();
            return GetSuccessJsonResponse();
        }

    }
}
