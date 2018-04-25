namespace Zidium.UserAccount.Models
{
    public class DashboardModel
    {
        // Компоненты - общая информация
        public DashboardComponentsInfoModel ComponentsTotal { get; set; }

        // Компоненты - разбивка по типам
        public DashboardComponentsInfoModel[] ComponentsByTypes { get; set; }

        /*
        // Типы актуальных событий - общая информация

        public int ActualEventTypesCount { get; set; }

        public int ActualEventTypesAlertCount { get; set; }

        public int ActualEventTypesWarningCount { get; set; }

        public int ActualEventTypesInfoCount { get; set; }

        public int ActualEventTypesOtherCount { get; set; }

        // Типы актуальных событий - сводка

        public DashboardEventTypesByPeriodModel TypesByPeriodAlert { get; set; }

        public DashboardEventTypesByPeriodModel TypesByPeriodWarning { get; set; }

        public DashboardEventTypesByPeriodModel TypesByPeriodInfo { get; set; }

        public DashboardEventTypesByPeriodModel TypesByPeriodOther { get; set; }

        // Актуальные события - общая информация

        public int ActualEventsCount { get; set; }

        public int ActualEventsAlertCount { get; set; }

        public int ActualEventsWarningCount { get; set; }

        public int ActualEventsInfoCount { get; set; }

        public int ActualEventsOtherCount { get; set; }

        // Актуальные события - сводка

        public DashboardEventsByPeriodModel ByPeriodAlert { get; set; }

        public DashboardEventsByPeriodModel ByPeriodWarning { get; set; }

        public DashboardEventsByPeriodModel ByPeriodInfo { get; set; }

        public DashboardEventsByPeriodModel ByPeriodOther { get; set; }

        // Актуальные события - по типам

        public DashboardEventsByTypeModel[] ActualEventsAlert { get; set; }

        public DashboardEventsByTypeModel[] ActualEventsWarning { get; set; }
        */
    }
}