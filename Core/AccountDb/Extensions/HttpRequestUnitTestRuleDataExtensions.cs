using System.Linq;
using Zidium.Storage;

namespace Zidium.Core.AccountDb
{
    public static class HttpRequestUnitTestRuleDataExtensions
    {
        public static HttpRequestUnitTestRuleDataForRead[] GetWebFormsDatas(this HttpRequestUnitTestRuleDataForRead[] datas)
        {
            return datas.Where(t => t.Type == HttpRequestUnitTestRuleDataType.WebFormData).ToArray();
        }

        public static HttpRequestUnitTestRuleDataForRead[] GetRequestHeaders(this HttpRequestUnitTestRuleDataForRead[] datas)
        {
            return datas.Where(t => t.Type == HttpRequestUnitTestRuleDataType.RequestHeader).ToArray();
        }

        public static HttpRequestUnitTestRuleDataForRead[] GetRequestCookies(this HttpRequestUnitTestRuleDataForRead[] datas)
        {
            return datas.Where(t => t.Type == HttpRequestUnitTestRuleDataType.RequestCookie).ToArray();
        }
    }
}
