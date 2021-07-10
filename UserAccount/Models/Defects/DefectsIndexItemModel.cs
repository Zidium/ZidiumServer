using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.Defects
{
    public class DefectsIndexItemModel
    {
        public Guid Id { get; set; }

        public Guid? EventTypeId { get; set; }

        public DateTime LastChangeDate { get; set; }

        public DefectStatus Status { get; set; }

        public Guid? ResponsibleUserId { get; set; }

        public GetGuiDefectsInfo.ResponsibleUserInfo ResponsibleUser { get; set; }

        public string Code { get; set; }

        public string Comment { get; set; }

        public string Title { get; set; }

        public int Count { get; set; }

        public string OldVersion { get; set; }
    }
}