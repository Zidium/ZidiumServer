using System;

namespace Zidium.UserAccount.Models.Defects
{
    public class EditDefectModel
    {
        public Guid Id { get; set; }

        [MyRequired]
        public string Title { get; set; }

        public string DefectCode { get; set; }

        [MyRequired]
        public Guid? UserId { get; set; }

        public string Notes { get; set; }
    }
}