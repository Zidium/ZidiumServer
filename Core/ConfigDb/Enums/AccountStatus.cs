namespace Zidium.Core.ConfigDb
{
    public enum AccountStatus
    {
        /// <summary>
        /// Нормальное состояние
        /// </summary>
        Active = 1,

        /// <summary>
        /// Заблокирован
        /// </summary>
        Blocked = 2,

        /// <summary>
        /// В процессе создания
        /// </summary>
        InCreation = 3
    }
}
