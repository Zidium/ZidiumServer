using System.ComponentModel.DataAnnotations;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.SqlChecksModels
{
    public class EditSimpleModel : CheckSimpleBaseModel
    {
        [Required(ErrorMessage = "Пожалуйста, заполните название")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Пожалуйста, выберите провайдера")]
        [Display(Name = "Провайдер")]
        public SqlRuleDatabaseProviderType Provider { get; set; }

        [Required(ErrorMessage = "Пожалуйста, заполните строку соединения")]
        [Display(Name = "Строка соединения")]
        public string ConnectionString { get; set; }

        [Required(ErrorMessage = "Пожалуйста, заполните Sql-запрос")]
        [Display(Name = "Sql-запрос")]
        [DataType("TextArea")]
        public string Query { get; set; }

    }
}