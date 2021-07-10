namespace Zidium.UserAccount.Models.Controls
{
    /// <summary>
    /// Используется для построения раскрашенных ячеек с количеством статусов
    /// </summary>
    public class ColorCountGroup
    {
        public ColorCountGroupItem RedItem { get; set; }

        public ColorCountGroupItem YellowItem { get; set; }

        public ColorCountGroupItem GreenItem { get; set; }

        public ColorCountGroupItem GrayItem { get; set; }

        public int All
        {
            get { return RedItem.Count + YellowItem.Count + GreenItem.Count + GrayItem.Count; }
        }

        /// <summary>
        /// Самый опасный цвет в группе
        /// </summary>
        public ImportanceColor HighImportanceColor
        {
            get
            {
                if (RedItem.Count> 0)
                {
                    return ImportanceColor.Red;
                }
                if (YellowItem.Count > 0)
                {
                    return ImportanceColor.Yellow;
                }
                if (GreenItem.Count > 0)
                {
                    return ImportanceColor.Green;
                }
                return ImportanceColor.Gray;
            }
        }

        public string AllUrl { get; set; }
    }
}