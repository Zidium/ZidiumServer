using System;
using System.IO;
using System.Net;

namespace Zidium.Core.Common.Helpers
{
    public static class BannerHelper
    {
        public static readonly string BannerHtmlCode = "@<a href='http://zidium.net'><img src='http://zidium.net/banners/app-mon-120-60.png' style='width:120px; height:60px; border: 0' alt='ZIDIUM - мониторинг приложений' title='ZIDIUM - мониторинг приложений'></a>";

        private static bool HasAttribute(string html, string attrName, string attrValue)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }
            if (attrName == null)
            {
                throw new ArgumentNullException("attrName");
            }
            if (attrValue == null)
            {
                throw new ArgumentNullException("attrValue");
            }

            // двойные кавычки
            var searchText = attrName + "=\"" + attrValue + "\"";
            if (html.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return true;
            }

            // одинарные кавычки
            searchText = attrName + "='" + attrValue + "'";
            if (html.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }

        public static bool FindBanner(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.UserAgent = "Zidium-Banner-Checker";
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null)
                        return false;

                    using (var reader = new StreamReader(stream))
                    {
                        var html = reader.ReadToEnd();

                        // ищем фрагмент
                        // <a href='http://zidium.net'><img src='http://zidium.net/banners/app-mon-120-60.png' width='120' height='60' border='0' alt='ZIDIUM - мониторинг приложений' title='ZIDIUM - мониторинг приложений'></a>

                        // чтобы не иметь проблем с разным форматированием, сделаем упрощенную проверку:

                        // 1. проверим что есть ссылка на сайт zidium.net
                        if (HasAttribute(html, "href", "http://zidium.net") == false &&
                            HasAttribute(html, "href", "https://zidium.net") == false)
                        {
                            return false;
                        }

                        // 2. проверим что есть ссылка на баннер http://zidium.net/banners/app-mon-120-60.png
                        if (HasAttribute(html, "src", "http://zidium.net/banners/app-mon-120-60.png") == false &&
                            HasAttribute(html, "src", "https://zidium.net/banners/app-mon-120-60.png") == false)
                        {
                            return false;
                        }

                        // 3. проверим что есть alt='ZIDIUM - мониторинг приложений'
                        if (HasAttribute(html, "alt", "ZIDIUM - мониторинг приложений")==false)
                        {
                            return false;
                        }

                        // 4. проверим что есть title='ZIDIUM - мониторинг приложений'
                        if (HasAttribute(html, "title", "ZIDIUM - мониторинг приложений")==false)
                        {
                            return false;
                        }

                        // есть баннер
                        return true;
                    }
                }
            }
        }
    }
}
