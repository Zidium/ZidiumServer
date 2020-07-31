namespace Zidium.Storage
{
    public enum HttpRequestUnitTestRuleDataType
    {
        /// <summary>
        /// Заголовок HTTP-запроса
        /// </summary>
        RequestHeader = 1,

        /// <summary>
        /// Куки HTTP-запроса
        /// </summary>
        RequestCookie = 2,

        // Данные веб-формы
        WebFormData = 3
    }
}
