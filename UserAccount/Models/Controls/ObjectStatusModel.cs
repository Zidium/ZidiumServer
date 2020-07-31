using Zidium.Storage;

namespace Zidium.UserAccount.Models.Controls
{
    public class ObjectStatusModel
    {
        public string Text { get; set; }

        public string Url { get; set; }

        public int FontSize { get; set; }

        public string CssClass { get; set; }

        public ObjectStatusModel()
        {
            FontSize = 28;
            SetGray();
        }

        public void SetRed()
        {
            CssClass = "label text-strongbgred";
        }

        public void SetYellow()
        {
            CssClass = "label text-strongbgyellow";
        }

        public void SetGreen()
        {
            CssClass = "label text-strongbggreen";
        }

        public void SetGray()
        {
            CssClass = "label text-strongbggray";
        }

        public void SetColor(DefectStatus status)
        {
            if (status == DefectStatus.Closed)
            {
                SetGreen();
            }
            else
            {
                SetRed();
            }
        }
    }
}