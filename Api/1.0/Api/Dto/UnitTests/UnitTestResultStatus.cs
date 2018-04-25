namespace Zidium.Api
{
    /// <summary>
    /// Результат юнит-теста
    /// </summary>
    public enum UnitTestResult
    {
        /// <summary>
        /// Результат юнит-теста неизвестен, т.е. нельзя сказать, что все хорошо или плохо.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Все ОК
        /// </summary>
        Success = 50,

        /// <summary>
        /// Требует внимания
        /// </summary>
        Warning = 100,

        /// <summary>
        /// Необходимо принять меры
        /// </summary>
        Alarm = 150
    }
}
