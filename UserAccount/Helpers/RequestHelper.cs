using System;
using Microsoft.AspNetCore.Http;

namespace Zidium.UserAccount
{
    public static class RequestHelper
    {
        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// 
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";

            return false;
        }

        /// <summary>
        /// Определение, что запрос отправлен через smart-blocks и требует соответствующего формата ответа
        /// </summary>
        public static bool IsSmartBlocksRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Headers != null)
                return request.Headers["SmartBlocksRequest"] == "true";

            return false;
        }
    }
}
