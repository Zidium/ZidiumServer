namespace Zidium.Api
{
    /// <summary>
    /// Категория события
    /// </summary>
    public enum EventCategory
    {
        #region Пользовательские события компонента

        /// <summary>
        /// Обычные (общие) события компонента
        /// </summary>
        ComponentEvent = 1,

        /// <summary>
        /// Ошибки приложений
        /// </summary>
        ApplicationError = 3,

        #endregion

        #region Юнит-тесты

        /// <summary>
        /// Результат юнит-теста
        /// </summary>
        UnitTestResult = 10,

        /// <summary>
        /// Статус юнит-теста
        /// </summary>
        UnitTestStatus = 11,

        #endregion

        #region Метрики

        /// <summary>
        /// Событие метрики
        /// </summary>
        // MetricResult = 20,

        /// <summary>
        /// Статус метрики
        /// </summary>
        MetricStatus = 21,

        #endregion

        #region Колбаски компонента

        /// <summary>
        /// Колбаска всех юнит-тестов компонента
        /// </summary>
        ComponentUnitTestsStatus = 30,

        /// <summary>
        /// Колбаска всех пользовательских событий
        /// </summary>
        ComponentEventsStatus = 31,

        /// <summary>
        /// Колбаска всех метрик компонента
        /// </summary>
        ComponentMetricsStatus = 32,

        /// <summary>
        /// Колбаска всех дочерних компонентов
        /// </summary>
        ComponentChildsStatus = 33,

        /// <summary>
        /// Внутренний статус компонента
        /// </summary>
        ComponentInternalStatus = 40,

        /// <summary>
        /// Внешний статус компонента
        /// </summary>
        ComponentExternalStatus = 41,

        #endregion
    }
}
