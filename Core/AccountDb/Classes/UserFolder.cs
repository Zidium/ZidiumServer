namespace Zidium.Core
{
    public class UserFolder
    {
        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        protected UserFolder(string systemName, string displayName)
        {
            SystemName = systemName;
            DisplayName = displayName;
        }

        public static readonly UserFolder WebSites = new UserFolder("WebSites", "Веб-сайты");

        public static readonly UserFolder DataBases = new UserFolder("DataBases", "Базы данных");

        public static readonly UserFolder Domains = new UserFolder("Domains", "Домены");

        public static readonly UserFolder OtherComponents = new UserFolder("OtherComponents", "Разные компоненты");
    }
}
