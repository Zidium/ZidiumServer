namespace Zidium.UserAccount.Models.Home
{
    public class StartModel
    {
        public bool HintSetMobilePhone { get; set; }

        public bool HasHints
        {
            get { return HintSetMobilePhone; }
        }
    }
}