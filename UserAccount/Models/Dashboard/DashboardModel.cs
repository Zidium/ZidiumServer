namespace Zidium.UserAccount.Models
{
    public class DashboardModel
    {
        // Компоненты - общая информация
        public DashboardComponentsInfoModel ComponentsTotal { get; set; }

        // Компоненты - разбивка по типам
        public DashboardComponentsInfoModel[] ComponentsByTypes { get; set; }

    }
}