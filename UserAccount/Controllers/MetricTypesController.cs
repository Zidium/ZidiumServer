using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;
using Zidium.UserAccount.Models.MetricTypes;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class MetricTypesController : ContextController
    {
        public ActionResult Index(string search)
        {
            var countersRepository = CurrentAccountDbContext.GetMetricTypeRepository();

            var query = countersRepository
                .QueryAll()
                .Where(t => t.IsDeleted == false);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Id.ToString().Equals(search, StringComparison.OrdinalIgnoreCase) || t.SystemName.Contains(search));

            var counters = query.OrderBy(t => t.SystemName);

            var model = new ListModel()
            {
                Items = counters,
                Search = search
            };

            return View(model);
        }

        public ActionResult Show(Guid id)
        {
            var metricType = GetMetricTypeById(id);
            var model = new ShowModel()
            {
                Id = metricType.Id,
                SystemName = metricType.SystemName,
                DisplayName = metricType.DisplayName,
                ActualTime = TimeSpanHelper.FromSeconds(metricType.ActualTimeSecs),
                NoSignalColor = metricType.NoSignalColor,
                ConditionRed = metricType.ConditionAlarm ?? "не задано",
                ConditionYellow = metricType.ConditionWarning ?? "не задано",
                ConditionGreen = metricType.ConditionSuccess ?? "не задано",
                ElseColor = metricType.ConditionElseColor,
                ModalMode = Request.IsAjaxRequest()
            };
            return View(model);
        }

        [CanEditAllData]
        public ActionResult Edit(Guid? id)
        {
            var model = new EditModel()
            {
                ModalMode = Request.IsAjaxRequest()
            };
            if (id.HasValue)
            {
                var metricType = GetMetricTypeById(id.Value);
                model.Id = metricType.Id;
                model.SystemName = metricType.SystemName;
                model.DisplayName = metricType.DisplayName;
                model.NoSignalColor = ColorStatusSelectorValue.FromColor(metricType.NoSignalColor);
                model.ActualTime = TimeSpanHelper.FromSeconds(metricType.ActualTimeSecs);
                model.ConditionRed = metricType.ConditionAlarm;
                model.ConditionYellow = metricType.ConditionWarning;
                model.ConditionGreen = metricType.ConditionSuccess;
                model.ElseColor = ColorStatusSelectorValue.FromColor(metricType.ConditionElseColor);
            }
            else
            {
                // значения по умолчанию
                model.NoSignalColor = new ColorStatusSelectorValue()
                {
                    RedChecked = true
                };
                model.ElseColor = new ColorStatusSelectorValue()
                {
                    RedChecked = true
                };
            }
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditModel model)
        {
            if (model.NoSignalColor.Checked == false)
            {
                ModelState.AddModelError("NoSignalColor", "Выберите цвет");
            }

            if (!ModelState.IsValid)
                return View(model);


            Guid metricTypeId;
            var client = GetDispatcherClient();
            if (model.Id.HasValue)
            {
                metricTypeId = model.Id.Value;
                var noSignalColor = model.NoSignalColor.GetSelectedColors().Single();
                var updateData = new UpdateMetricTypeRequestData()
                {
                    MetricTypeId = metricTypeId,
                    SystemName = model.SystemName,
                    DisplayName = model.DisplayName,
                    AlarmCondition = model.ConditionRed,
                    WarningCondition = model.ConditionYellow,
                    SuccessCondition = model.ConditionGreen,
                    ElseColor = model.ElseColor.GetSelectedOne(),
                    ActualTimeSecs = TimeSpanHelper.GetSeconds(model.ActualTime),
                    NoSignalColor = noSignalColor
                };
                client.UpdateMetricType(CurrentUser.AccountId, updateData).Check();
            }
            else
            {
                var createData = new CreateMetricTypeRequestData()
                {
                    SystemName = model.SystemName,
                    DisplayName = model.DisplayName,
                    AlarmCondition = model.ConditionRed,
                    WarningCondition = model.ConditionYellow,
                    SuccessCondition = model.ConditionGreen,
                    ElseColor = model.ElseColor.GetSelectedOne(),
                    NoSignalColor = model.NoSignalColor.GetSelectedColors().SingleOrDefault(),
                    ActualTimeSecs = TimeSpanHelper.GetSeconds(model.ActualTime)
                };
                var response = client.CreateMetricType(CurrentUser.AccountId, createData);
                response.Check();
                metricTypeId = response.Data.MetricTypeId;
            }

            return RedirectToAction("Show", new {id = metricTypeId});
        }

        public JsonResult CheckName(EditModel model)
        {
            var repository = CurrentAccountDbContext.GetMetricTypeRepository();
            var counter = repository.GetOneOrNullByName(model.SystemName);
            if (counter != null && (!model.Id.HasValue || model.Id != counter.Id))
                return Json("Метрика с таким именем уже существует", JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var metricType = GetMetricTypeById(id);
            var model = new DeleteConfirmationAjaxModel()
            {
                Title = "Удаление типа метрики",
                Message = "Вы действительно хотите удалить тип метрики " + metricType.SystemName + "?",
            };
            return View("Dialogs/DeleteConfirmationAjax", model);
        }

        [CanEditAllData]
        [HttpPost]
        public ActionResult Delete(Guid id, string fake)
        {
            try
            {
                var dispatcher = GetDispatcherClient();
                var data = new DeleteMetricTypeRequestData()
                {
                    MetricTypeId = id
                };
                dispatcher.DeleteMetricType(CurrentUser.AccountId, id).Check();
                return GetSuccessJsonResponse();
            }
            catch (Exception exception)
            {
                MvcApplication.HandleException(exception);
                return GetErrorJsonResponse(exception);
            }
        }
	}
}