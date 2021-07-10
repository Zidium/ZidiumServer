using System.Collections.Generic;

namespace Zidium.UserAccount.Models.Controls
{
    public class KeyValueTableModel
    {
        public string AddRowButtonTitle { get; set; }

        public string CollectionName { get; set; }

        public List<KeyValueRowModel> Rows { get; set; }
    }
}