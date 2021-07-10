namespace Zidium.UserAccount.Models.Controls
{
    public class ColorStatusSelectorModel
    {
        public ColorStatusSelectorValue Value { get; set; }

        public string PropertyName { get; set; }

        public bool AutoRefreshPage { get; set; }

        /// <summary>
        /// Можно ли выбирать несколько значений
        /// </summary>
        public bool MultiSelectMode { get; set; }

        public string Callback { get; set; }
    }
}