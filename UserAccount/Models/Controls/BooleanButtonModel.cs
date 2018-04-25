namespace Zidium.UserAccount.Models
{
    public class BooleanButtonModel
    {
        public string Name { get; set; }

        public bool AutoRefreshPage { get; set; }

        public string CaptionTrue { get; set; }

        public string CaptionFalse { get; set; }

        public bool FalseIsNull { get; set; }

        public bool Value { get; set; }
    }
}