namespace Zidium.Core.AccountsDb
{
    public enum VirusTotalStep
    {
        /// <summary>
        /// Нужно выполнять сканирование
        /// </summary>
        Scan = 1,

        /// <summary>
        /// Нужно получить отчет (сканирование уже выполнилось)
        /// </summary>
        Report = 2
    }
}
