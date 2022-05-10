using System;
using Zidium.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models
{
    public class DisableDialogPostModel
    {
        public Guid Id { get; set; }

        public string Interval { get; set; }

        public DateTime? Date { get; set; }

        public string Comment { get; set; }

        public DateTime? GetDate()
        {
            if (Interval == "forever")
            {
                return null;
            }
            if (Interval != "my")
            {
                var duration = TimeSpanHelper.ParseHtml(Interval);
                return DateTime.UtcNow + duration;
            }
            if (Date == null)
            {
                throw new UserFriendlyException("Не удалось получить дату отключения");
            }
            return Date.Value;
        }
    }
}