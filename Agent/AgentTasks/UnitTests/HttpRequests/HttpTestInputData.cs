using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Zidium.Core.AccountsDb;

namespace Zidium.Agent.AgentTasks.UnitTests.HttpRequests
{
    public class HttpTestInputData
    {
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
        public byte[] Body { get; set; }

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
        /// Параметры формы
        /// </summary>
        public Dictionary<string, string> FormParams { get; set; }

        /// <summary>
        /// Заголовки запроса
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        public CookieCollection Cookies { get; set; }
    }
}
