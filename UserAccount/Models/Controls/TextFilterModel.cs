namespace Zidium.UserAccount.Models
{
    public class TextFilterModel
    {
        public string Name { get; set; }

        public bool AutoRefreshPage { get; set; }

        public string Placeholder { get; set; }

        public string Value { get; set; }

        public bool HideWhenFilter { get; set; }
    }
}