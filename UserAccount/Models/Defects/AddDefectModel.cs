using System;
using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models.Defects
{
    /// <summary>
    /// Создает дефект БЕЗ привязки к типу события
    /// </summary>
    public class AddDefectModel
    {
        [MyRequired]
        [Display(Name = "Название")]
        public string Title { get; set; }

        [MyRequired]
        [DataType("User")]
        [Display(Name = "Ответственный")]
        public Guid? ResponsibleUserId { get; set; }

        [DataType("TextArea")]
        [Display(Name = "Заметки")]
        public string Notes { get; set; }
    }
}