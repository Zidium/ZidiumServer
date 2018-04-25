namespace Zidium.Core.ConfigDb
{
    public enum AccountType
    {
        /// <summary>
        /// Это аккаунт самой системы, чтобы мониторить саму себя (агент, личный кабинет и т.д.)
        /// </summary>
        System = 1,

        /// <summary>
        /// За который платят деньги
        /// </summary>
        Paid = 2,

        /// <summary>
        /// Бесплатный
        /// </summary>
        Free = 3,

        /// <summary>
        /// Для внутреннего тестирования
        /// </summary>
        Test = 4,
    }
}
