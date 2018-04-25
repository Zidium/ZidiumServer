using System;

namespace Zidium.Core.Common.Helpers
{
    public static class UserAgentHelper
    {
        public static bool IsBot(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return false;
            }
            var botTexts = new[]
            {
                "AhrefsBot",
                "ia_archiver",
                "BLEXBot",
                "bingbot",
                "crawler",
                "Googlebot",
                "SMTBot",
                "SafeDNSBot",
                "spider",
                "YandexMetrika",
                "YandexBot",
                "YandexDirect",
                "Yahoo!",
                "YaDirectFetcher",
                "Mail.RU_Bot",
                "vkShare",
                "searchbot",
                "MegaIndex",
                "bot"
            };
            foreach (var text in botTexts)
            {
                if (userAgent.IndexOf(text, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
