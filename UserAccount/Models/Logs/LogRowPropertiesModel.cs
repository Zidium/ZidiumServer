using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class LogRowPropertiesModel
    {
        public Guid LogId { get; set; }

        public Guid ComponentId { get; set; }

        public string Context { get; set; }

        public string Message { get; set; }

        public string Text { get; set; }

        public LogRowPropertyItem[] Items { get; set; }

        public class LogRowPropertyItem
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public DataType DataType { get; set; }

            public string Value { get; set; }
        }
    }
}