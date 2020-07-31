using System;

namespace Zidium.Storage
{
    public class HttpRequestUnitTestRuleForAdd
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Ссылка на настройку http-проверки
        /// </summary>
        public Guid HttpRequestUnitTestId;

        /// <summary>
        /// Порядковый номер
        /// </summary>
        public int SortNumber;

        /// <summary>
        /// Дружелюбное имя правила
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Урл запроса
        /// </summary>
        public string Url;

        /// <summary>
        /// Метод запроса
        /// </summary>
        public HttpRequestMethod Method;

        /// <summary>
        /// Содержимое Body для POST-запроса
        /// </summary>
        public string Body;

        /// <summary>
        /// Код ответа, который должен быть
        /// </summary>
        public int? ResponseCode;

        /// <summary>
        /// Максимальный размер ответа
        /// </summary>
        public int MaxResponseSize;

        /// <summary>
        /// Фрагмент Html, который должен быть в ответе, если ошибок нет
        /// </summary>
        public string SuccessHtml;

        /// <summary>
        /// Фрагмент Html который должен быть при ошибке
        /// </summary>
        public string ErrorHtml;

        /// <summary>
        /// Максимальное время выполнения запроса
        /// </summary>
        public int? TimeoutSeconds;

        /// <summary>
        /// Результат последнего выполнения
        /// </summary>
        public HttpRequestErrorCode? LastRunErrorCode;

        /// <summary>
        /// Последняя ошибка, если была
        /// </summary>
        public string LastRunErrorMessage;

        /// <summary>
        /// Время последнего запуска
        /// </summary>
        public DateTime? LastRunTime;

        /// <summary>
        /// Длительность последнего запуска
        /// </summary>
        public int? LastRunDurationMs;

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted;

    }
}
