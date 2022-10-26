namespace Zidium.Storage
{
    /// <summary>
    /// Тип ошибки
    /// </summary>
    public enum HttpRequestErrorCode
    {
        /// <summary>
        /// Нет ошибок (все хорошо)
        /// </summary>
        Success = 1,

        /// <summary>
        /// Неверный код ответа (не 200)
        /// </summary>
        InvalidResponseCode = 2,

        /// <summary>
        /// Не найден ожидаемый фрагмент Html
        /// </summary>
        SuccessHtmlNotFound = 3,

        /// <summary>
        /// Обнаружен фрагмент html, которого быть не должно
        /// </summary>
        ErrorHtmlFound = 4,

        /// <summary>
        /// Превышен таймаут
        /// </summary>
        Timeout = 5,

        /// <summary>
        /// Слишком большой ответ
        /// </summary>
        TooLargeResponse = 6,

        /// <summary>
        /// Не удалось определить домен
        /// </summary>
        UnknownDomain = 7,

        /// <summary>
        /// Ошибка TCP соединения
        /// </summary>
        TcpError = 8,

        /// <summary>
        /// Неверный формат URL
        /// </summary>
        UrlFormatError = 9,

        /// <summary>
        /// Ошибка неизвестна
        /// </summary>
        UnknownError = 50
    }
}
