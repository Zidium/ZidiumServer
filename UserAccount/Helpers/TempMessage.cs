namespace Zidium.UserAccount.Helpers
{
    public class TempMessage
    {
        public TempMessageType Type;

        public string Message;

        public string CssClass
        {
            get
            {
                if (Type == TempMessageType.Success)
                    return "alert-success";
                if (Type == TempMessageType.Info)
                    return "alert-info";
                if (Type == TempMessageType.Warning)
                    return "alert-warning";
                if (Type == TempMessageType.Error)
                    return "alert-danger";
                return string.Empty;
            }
        }
    }
}