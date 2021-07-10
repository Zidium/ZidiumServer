using Zidium.Storage;

namespace Zidium.Core.Common.EnumNames
{
    public class RussianEnumName : IEnumName
    {
        public string GetName(DefectStatus value)
        {
            if (value == DefectStatus.Unknown)
            {
                return "?";
            }
            if (value == DefectStatus.Open)
            {
                return "Открыт";
            }
            if (value == DefectStatus.InProgress)
            {
                return "В работе";
            }
            if (value == DefectStatus.Testing)
            {
                return "Тестируется";
            }
            if (value == DefectStatus.Closed)
            {
                return "Закрыт";
            }
            if (value == DefectStatus.Reopened)
            {
                return "Открыт повторно";
            }
            return "???";
        }
    }
}
