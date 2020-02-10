namespace Zidium.Core.ConfigDb
{
    public static class UrlHelper
    {
        public static string CommonWebSite
        {
            get
            {
                if (_commonWebSite == null)
                {
                    _commonWebSite = new UrlService().GetCommonWebsiteUrl();
                }
                return _commonWebSite;
            }
        }

        private static string _commonWebSite;
    }
}
