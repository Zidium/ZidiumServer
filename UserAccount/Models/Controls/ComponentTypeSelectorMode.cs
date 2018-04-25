namespace Zidium.UserAccount.Models
{
    public enum ComponentTypeSelectorMode
    {
        /// <summary>
        /// Показывать все типы
        /// </summary>
        ShowAllTypes = 0,

        /// <summary>
        /// Показывает только типы, у которых есть компоненты
        /// </summary>
        ShowOnlyUsedTypes = 1
    }
}