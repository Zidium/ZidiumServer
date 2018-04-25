using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.MetricData;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class MetricsDataController : ContextController
    {

        public ActionResult Index(Guid? componentId, Guid? counterId, DateTime? from, DateTime? to)
        {
            var eDate = to ?? DateTime.Now;
            var sDate = from ?? eDate.AddDays(-1);

            Component component = null;
            if (componentId.HasValue)
            {
                var componentRepository = CurrentAccountDbContext.GetComponentRepository();
                component = componentRepository.GetById(componentId.Value);
            }

            var model = new CounterDataListModel()
            {
                ComponentId = componentId,
                From = sDate,
                To = eDate
            };

            if (component != null)
            {
                var counterRepository = CurrentAccountDbContext.GetMetricTypeRepository();

                MetricType metricType;
                if (counterId.HasValue)
                {
                    metricType = counterRepository.GetById(counterId.Value);
                    model.CounterId = metricType.Id;
                    model.CounterName = metricType.SystemName;
                }
                else
                {
                    var metricTypes = component.Metrics.Where(t => t.IsDeleted == false && t.MetricType.IsDeleted == false).Select(t => t.MetricType);
                    metricType = metricTypes.OrderBy(t => t.SystemName).FirstOrDefault();
                    model.CounterId = metricType != null ? metricType.Id : (Guid?)null;
                    model.CounterName = metricType != null ? metricType.SystemName : null;
                }

                if (model.CounterId.HasValue)
                {
                    var storageContext = CurrentAccountDbContext;
                    var metricHistoryRepository = storageContext.GetMetricHistoryRepository();

                    var rows = metricHistoryRepository
                        .GetByPeriod(component.Id, sDate, eDate, new[] { model.CounterId.Value })
                        .OrderByDescending(t => t.BeginDate)
                        .ToArray();

                    model.Data = rows.ToArray();
                    model.Graph = GetCounterGraphDataModel(metricType, rows.OrderBy(t => t.BeginDate).ToArray());
                }
            }

            return View(model);
        }

        public ActionResult GraphByInterval(Guid id, TimelineInterval interval)
        {
            var toDate = MvcApplication.GetServerDateTime();
            var fromDate = TimelineHelper.IntervalToStartDate(toDate, interval);

            var metricRepository = CurrentAccountDbContext.GetMetricRepository();
            var metric = metricRepository.GetById(id);

            var metricHistoryRepository = CurrentAccountDbContext.GetMetricHistoryRepository();
            var rows = metricHistoryRepository
                .GetByPeriod(metric.ComponentId, fromDate, toDate, new[] { metric.MetricTypeId })
                .OrderBy(t => t.BeginDate)
                .ToArray();

            var model = GetCounterGraphDataModel(metric.MetricType, rows);

            return PartialView("GraphPartial", model);
        }

        private MetricGraphDataModel GetCounterGraphDataModel(MetricType metricType, MetricHistory[] rows)
        {
            var model = new MetricGraphDataModel();

            // Посчитаем статистику
            model.Min = rows.Min(t => t.Value);
            model.Max = rows.Max(t => t.Value);
            model.Avg = rows.Average(t => t.Value);

            // Если точек больше 100, то надо прореживать выборку
            // График с большим количеством точек рисуется медленно
            if (rows.Length > 100)
            {
                var newRows = new List<MetricHistory>();
                var step = rows.Length / 100.0d;
                var index = 0.0d;
                while (index < rows.Length)
                {
                    // Продлим предыдущее значение на время всех выброшенных точек
                    if (index > 0)
                    {
                        var prevItem = rows[(int)index - 1];
                        newRows.Last().ActualDate = prevItem.ActualDate;
                    }

                    var item = rows[(int)index];
                    newRows.Add(item);

                    index += step;
                }
                rows = newRows.ToArray();
            }

            // Преобразуем данные, вставив разрывы там, где теряется актуальность
            var result = new List<MetricGraphDataItem>();
            DateTime? prevDate = null;
            double? prevValue = 0;
            foreach (var row in rows)
            {
                // Если дата начала значения метрики больше даты актуальности предыдущего, то вставим разрыв
                if (prevDate.HasValue && (row.BeginDate - prevDate.Value).TotalSeconds > 1)
                {
                    result.Add(new MetricGraphDataItem()
                    {
                        Date = prevDate.Value,
                        Value = prevValue,
                        Color = ObjectColor.Gray
                    });
                    result.Add(new MetricGraphDataItem()
                    {
                        Date = prevDate.Value,
                        Value = null,
                        Color = ObjectColor.Gray
                    });
                }

                // Добавим текущее значение
                result.Add(new MetricGraphDataItem()
                {
                    Date = row.BeginDate,
                    Value = row.Value,
                    Color = row.Color
                });

                prevDate = row.ActualDate;
                prevValue = row.Value;
            }

            model.Name = metricType.DisplayName;
            model.GraphData = result.ToArray();

            return model;
        }
    }
}