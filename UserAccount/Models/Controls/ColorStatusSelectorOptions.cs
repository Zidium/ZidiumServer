namespace Zidium.UserAccount.Models.Controls
{
    public class ColorStatusSelectorOptions
    {
        /// <summary>
        /// Перезагружает страницу, если изменился фильтр
        /// </summary>
        public bool AutoRefreshPage { get; set; }

        /// <summary>
        /// Можно ли выбирать несколько значений
        /// </summary>
        public bool MultiSelectMode { get; set; }

        public string Callback { get; set; }

        public ColorStatusSelectorOptions()
        {
            AutoRefreshPage = true;
            MultiSelectMode = true;
        }
    }
}