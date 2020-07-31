using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class LogPartialModel
    {
        public LogFiltersModel Filters { get; set; }

        public ItemModel[] Items { get; set; }

        public DateAndOrderModel Previous { get; set; }

        public DateAndOrderModel Next { get; set; }

        public bool MarkAsNew { get; set; }

        public Guid? ScrollToId { get; set; }

        public bool NoPreviousRecords { get; set; }

        public bool NoNextRecords { get; set; }

        public LogRowPropertiesModel ExpandedProperties { get; set; }
        
        public int OutputRecordCount { get; set; }

        public class ItemModel
        {
            public Guid Id { get; set; }

            public DateTime Date { get; set; }

            public int Order { get; set; }

            public LogLevel Level { get; set; }

            public string Message { get; set; }
        }

        public class DateAndOrderModel
        {
            public DateTime Date { get; set; }

            public int Order { get; set; }
        }
    }
}