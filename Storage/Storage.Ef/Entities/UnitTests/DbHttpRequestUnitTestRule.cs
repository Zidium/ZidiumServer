using System;
using System.Collections.Generic;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Правило http-проверки
    /// </summary>
    public class DbHttpRequestUnitTestRule
    {
        public DbHttpRequestUnitTestRule()
        {
            Datas = new HashSet<DbHttpRequestUnitTestRuleData>();    
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на настройку http-проверки
        /// </summary>
        public Guid HttpRequestUnitTestId { get; set; }

        /// <summary>
        /// Http-проверка
        /// </summary>
        public virtual DbHttpRequestUnitTest HttpRequestUnitTest { get; set; }

        /// <summary>
        /// Порядковый номер
        /// </summary>
        public int SortNumber { get; set; }

        /// <summary>
        /// Дружелюбное имя правила
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Урл запроса
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Метод запроса
        /// </summary>
        public HttpRequestMethod Method { get; set; }

        /// <summary>
        /// Содержимое Body для POST-запроса
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Код ответа, который должен быть
        /// </summary>
        public int? ResponseCode { get; set; }

        /// <summary>
        /// Максимальный размер ответа
        /// </summary>
        public int MaxResponseSize { get; set; }

        /// <summary>
        /// Фрагмент Html, который должен быть в ответе, если ошибок нет
        /// </summary>
        public string SuccessHtml { get; set; }

        /// <summary>
        /// Фрагмент Html который должен быть при ошибке
        /// </summary>
        public string ErrorHtml { get; set; }

        /// <summary>
        /// Максимальное время выполнения запроса
        /// </summary>
        public int? TimeoutSeconds { get; set; }

        /// <summary>
        /// Результат последнего выполнения
        /// </summary>
        public HttpRequestErrorCode? LastRunErrorCode { get; set; }

        /// <summary>
        /// Последняя ошибка, если была
        /// </summary>
        public string LastRunErrorMessage { get; set; }

        /// <summary>
        /// Время последнего запуска
        /// </summary>
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// Длительность последнего запуска
        /// </summary>
        public int? LastRunDurationMs { get; set; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Данные для запроса
        /// </summary>
        public virtual ICollection<DbHttpRequestUnitTestRuleData> Datas { get; set; }

    }
}
