﻿using System;
using System.Linq;

namespace Zidium.UserAccount.Models
{
    public class DashboardEventTypesByPeriodModel
    {
        public DashboardEventTypesByPeriodModel(DashboardEventModel[] events)
        {
            var byTodayEvents = events.Where(t => t.StartDate.Date == DateTime.Now.Date.ToUniversalTime()).ToArray();
            ByToday = new DashboardEventTypesByPeriodLineModel(byTodayEvents);
            var byWeekEvents = events.Where(t => t.StartDate.Date >= DateTime.Now.AddDays(-7).Date.ToUniversalTime()).ToArray();
            ByWeek = new DashboardEventTypesByPeriodLineModel(byWeekEvents);
            var byMonthEvents = events.Where(t => t.StartDate.Date >= DateTime.Now.AddMonths(-1).Date.ToUniversalTime()).ToArray();
            ByMonth = new DashboardEventTypesByPeriodLineModel(byMonthEvents);
        }

        public DashboardEventTypesByPeriodLineModel ByToday { get; set; }
        public DashboardEventTypesByPeriodLineModel ByWeek { get; set; }
        public DashboardEventTypesByPeriodLineModel ByMonth { get; set; }
    }
}