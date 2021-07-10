using System;
using System.Collections.Generic;

namespace Zidium.UserAccount.Models.Controls
{
    public class StatusByTypeTableModel
    {
        public StatusByTypeTableModel()
        {
            Rows = new List<Row>();
        }

        public class Row
        {
            public string GroupName { get; set; }
            
            public string TypeName { get; set; }

            public int RedCount { get; set; }

            public int YellowCount { get; set; }

            public int GreenCount { get; set; }

            public int GrayCount { get; set; }

            public string GetByTypeUrl { get; set; }

            public string GetByGroupUrl { get; set; }

            public string GetByAllCountUrl { get; set; }

            public int Count()
            {
                return RedCount + YellowCount + GreenCount + GrayCount;
            }
        }

        public string TypeColumnName { get; set; }

        public List<Row> Rows { get; set; }
    }
}