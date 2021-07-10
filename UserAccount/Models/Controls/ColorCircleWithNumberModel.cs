namespace Zidium.UserAccount.Models.Controls
{
    /// <summary>
    /// Модель данных для цветного кружка с числом внутри
    /// </summary>
    public class ColorCircleWithNumberModel
    {
        public ImportanceColor Color { get; set; }

        public int Value { get; set; }

        public string Url { get; set; }

        public int? FontSizePx { get; set; }

        public string CssClass { get; set; }
    }
}