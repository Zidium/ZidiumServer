﻿using System;
using System.Linq;

namespace Zidium.UserAccount.Models
{
    public class DashboardEventsByPeriodModel
    {
        public DashboardEventsByPeriodModel(DashboardEventModel[] events)
        {
            var byTodayEvents = events.Where(t => t.StartDate.Date == DateTime.Now.Date.ToUniversalTime()).ToArray();
            ByToday = new DashboardEventsByPeriodLineModel(byTodayEvents);
            var byWeekEvents = events.Where(t => t.StartDate.Date >= DateTime.Now.AddDays(-7).Date.ToUniversalTime()).ToArray();
            ByWeek = new DashboardEventsByPeriodLineModel(byWeekEvents);
            var byMonthEvents = events.Where(t => t.StartDate.Date >= DateTime.Now.AddMonths(-1).Date.ToUniversalTime()).ToArray();
            ByMonth = new DashboardEventsByPeriodLineModel(byMonthEvents);
        }

        public DashboardEventsByPeriodLineModel ByToday { get; set; }
        public DashboardEventsByPeriodLineModel ByWeek { get; set; }
        public DashboardEventsByPeriodLineModel ByMonth { get; set; }
    }
}