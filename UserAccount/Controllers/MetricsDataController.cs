﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Zidium.Storage;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.MetricData;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class MetricsDataController : BaseController
    {
        public ActionResult Index(Guid? componentId, Guid? metricTypeId, DateTime? from, DateTime? to)
        {
            var eDate = to ?? DateTime.Now;
            var sDate = from ?? eDate.AddDays(-1);

            var model = new MetricDataListModel()
            {
                ComponentId = componentId,
                From = sDate,
                To = eDate
            };

            if (componentId != null)
            {
                MetricTypeForRead metricType;
                if (metricTypeId.HasValue)
                {
                    metricType = GetStorage().MetricTypes.GetOneById(metricTypeId.Value);
                    model.MetricTypeId = metricType.Id;
                    model.CounterName = metricType.DisplayName;
                }
                else
                {
                    var firstMetricTypeId = GetStorage().Metrics.GetByComponentId(componentId.Value).FirstOrDefault()?.MetricTypeId;
                    metricType = firstMetricTypeId.HasValue ? GetStorage().MetricTypes.GetOneById(firstMetricTypeId.Value) : null;
                    model.MetricTypeId = metricType?.Id;
                    model.CounterName = metricType?.DisplayName;
                }

                if (model.MetricTypeId.HasValue)
                {
                    var rows = GetStorage().MetricHistory.GetByPeriod(componentId.Value, sDate, eDate, new[] { model.MetricTypeId.Value }, 1000);

                    model.Data = rows.Take(100).ToArray();
                    model.Graph = GetCounterGraphDataModel(metricType, rows.OrderBy(t => t.BeginDate).ToArray());
                }
            }

            return View(model);
        }

        public ActionResult GraphByInterval(Guid id, TimelineInterval interval)
        {
            var toDate = MvcApplication.GetServerDateTime();
            var fromDate = TimelineHelper.IntervalToStartDate(toDate, interval);

            var metric = GetStorage().Metrics.GetOneById(id);
            var metricType = GetStorage().MetricTypes.GetOneById(metric.MetricTypeId);

            var rows = GetStorage().MetricHistory
                .GetByPeriod(metric.ComponentId, fromDate, toDate, new[] { metric.MetricTypeId }, 1000)
                .OrderBy(t => t.BeginDate)
                .ToArray();

            var model = GetCounterGraphDataModel(metricType, rows);

            return PartialView("GraphPartial", model);
        }

        private MetricGraphDataModel GetCounterGraphDataModel(MetricTypeForRead metricType, MetricHistoryForRead[] rows)
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
                var newRows = new List<MetricHistoryForRead>();
                var step = rows.Length / 100.0d;
                var index = 0.0d;
                while (index < rows.Length)
                {
                    // Продлим предыдущее значение на время всех выброшенных точек
                    if (index > 0)
                    {
                        var prevItem = rows[(int)index - 1];
                        var lastItem = newRows.Last();
                        newRows[newRows.Count - 1] = new MetricHistoryForRead(
                                lastItem.Id,
                                lastItem.ComponentId,
                                lastItem.MetricTypeId,
                                lastItem.BeginDate,
                                prevItem.ActualDate,
                                lastItem.Value,
                                lastItem.Color,
                                lastItem.StatusEventId,
                                lastItem.HasSignal
                                );
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