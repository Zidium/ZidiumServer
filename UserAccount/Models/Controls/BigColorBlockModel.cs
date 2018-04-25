using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount.Models.Controls
{
    public class BigColorBlockModel
    {
        public string Url { get; set; }

        public int Value { get; set; }

        public string Text { get; set; }

        public ImportanceColor Color { get; set; }

        public BigColorBlockModel(ImportanceColor color, int value, string url)
        {
            Value = value;
            Url = url;
            Color = color;
        }

        public string GetText()
        {
            if (Text != null)
            {
                return Text;
            }
            if (Color == ImportanceColor.Gray)
            {
                return "неактивны или отключены";
            }
            if (Color == ImportanceColor.Green)
            {
                return "все хорошо";
            }
            if (Color == ImportanceColor.Yellow)
            {
                return "необходимо внимание";
            }
            if (Color == ImportanceColor.Red)
            {
                return "требуют действий";
            }
            return "неизвестно";
        }

        public string GetBgColor()
        {
            if (Color == ImportanceColor.Gray)
            {
                return GuiHelper.StrongGrayBgColor;
            }
            if (Color == ImportanceColor.Green)
            {
                return GuiHelper.StrongGreenBgColor;
            }
            if (Color == ImportanceColor.Yellow)
            {
                return GuiHelper.StrongYellowBgColor;
            }
            if (Color == ImportanceColor.Red)
            {
                return GuiHelper.StrongRedBgColor;
            }
            return "black";
        }
    }
}