using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.ComponentTreeDetails
{
    public class UnittestDetailsResultsModel
    {
        public Guid Id { get; set; }

        public ResultModel[] Results { get; set; }

        public class ResultModel
        {
            public Guid EventId { get; set; }

            public DateTime Date { get; set; }

            public string Message { get; set; }

            public EventImportance Importance { get; set; }
        }
    }
}