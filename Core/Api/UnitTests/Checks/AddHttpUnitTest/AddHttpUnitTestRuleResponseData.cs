using Zidium.Core.AccountsDb;

namespace Zidium.Core.Api
{
    public class AddHttpUnitTestRuleResponseData
    {
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

    }
}
