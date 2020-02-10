using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount
{
    public static class TimelineHelper
    {
        public static List<TimelineItemModel> GetTimelineItemsByStates(this ContextController controller, TimelineState[] states, DateTime from, DateTime to)
        {
            var totalDuration = to - from;
            TimeSpan step;

            // Определим длительность одного блока в зависимости от диапазона дат
            if ((int) totalDuration.TotalMinutes <= 60)
                step = TimeSpan.FromMinutes(1);
            else if ((int) totalDuration.TotalHours <= 24)
                step = TimeSpan.FromMinutes(10);
            else if ((int) totalDuration.TotalDays <= 7)
                step = TimeSpan.FromHours(1);
            else if ((int) totalDuration.TotalDays <= 31)
                step = TimeSpan.FromHours(4);
            else
                step = TimeSpan.FromDays(1);

            var result = new List<TimelineItemModel>();
            var intervalFrom = from;
            TimelineItemModel lastItemModel = null;

            while (intervalFrom < to)
            {
                var intervalTo = intervalFrom + step;
                if (intervalTo >= to)
                    intervalTo = to;

                // Для каждого интервала найдём самое важное попадающее в него событие
                var intervalStates = states.Where(t => t.EndDate >= intervalFrom && t.StartDate < intervalTo).ToArray();
                var state = intervalStates
                    .OrderByDescending(t => t.Status)
                    .Take(1)
                    .FirstOrDefault();

                TimelineItemModel itemModel = null;

                if (state != null)
                {
                    // Посчитаем количество всех событий этой важности в интервале
                    var count = intervalStates.Where(t => t.Status == state.Status).Sum(t => t.Count);

                    // Если событие есть, создадим новый элемент диаграммы
                    itemModel = new TimelineItemModel()
                    {
                        EventId = state.Id,
                        StartDate = state.StartDate,
                        EndDate = state.EndDate,
                        Count = count,
                        Message = state.Message,
                        Status = state.Status,
                        Width = (intervalTo - intervalFrom).TotalSeconds / totalDuration.TotalSeconds * 100.0
                    };
                }
                else
                {
                    // Если события нет, создадим заполнитель
                    itemModel = new TimelineItemModel()
                    {
                        EventId = null,
                        StartDate = intervalFrom,
                        EndDate = intervalTo,
                        Count = null,
                        Message = null,
                        Status = null,
                        Width = (intervalTo - intervalFrom).TotalSeconds / totalDuration.TotalSeconds * 100.0
                    };
                }

                // Проверим, совпадает ли новый элемент с предыдущим
                if (lastItemModel != null && itemModel.EventId == lastItemModel.EventId)
                {
                    // Если совпадает, то продлим предыдущий
                    lastItemModel.EndDate = itemModel.EndDate;
                    lastItemModel.Width += itemModel.Width;
                    // Количество не прибавляем, оно уже учтено в предыдущем интервале
                    // lastItemModel.Count += itemModel.Count;
                }
                else
                {
                    // Если не совпадает, добавим новый элемент в модель
                    result.Add(itemModel);
                    lastItemModel = itemModel;
                }

                intervalFrom = intervalTo;
            }

            return result;
        }

        public static DateTime IntervalToStartDate(DateTime toDate, TimelineInterval interval)
        {
            if (interval == TimelineInterval.Hour)
                return toDate.AddHours(-1);

            if (interval == TimelineInterval.Day)
                return toDate.AddDays(-1);

            if (interval == TimelineInterval.Week)
                return toDate.AddDays(-7);

            if (interval == TimelineInterval.Month)
                return toDate.AddMonths(-1);

            return toDate.AddDays(-1);

        }

        public static string UnknownStatusColor = "#b7b7b7";

        public static string NoStatusColor = "#ececec";

    }
}