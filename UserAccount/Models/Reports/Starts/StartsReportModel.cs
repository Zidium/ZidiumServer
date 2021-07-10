using System;
using System.Linq;
using System.Collections.Generic;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount.Models
{
    public class StartsReportModel
    {
        public Guid? ComponentTypeId { get; set; }

        public Guid? ComponentId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public List<StartsReportItemModel> Items { get; set; }

        public List<StartsReportGrapthItemModel> Graph { get; set; }

        public string Error { get; set; }

        public Guid? StartEventTypeId { get; set; }

        public int Total { get; set; }

        public string JsGraphData
        {
            get
            {
                return string.Join(", ", Graph.Select(t => "[" + GuiHelper.DateTimeToHighChartsFormat(t.Date) + ", " + t.Count + "]"));
            }
        }
    }
}