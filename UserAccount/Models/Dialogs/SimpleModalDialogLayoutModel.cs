using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Razor;
using Zidium.Common;

namespace Zidium.UserAccount.Models.Dialogs
{
    /// <summary>
    /// Данные для Layout простого диалога, чтобы не передавать все через ViewBag
    /// </summary>
    public class SimpleModalDialogLayoutModel
    {
        public string Title { get; set; }

        public string SubmitBtnName { get; set; }

        public string SubmitUrl { get; set; }

        public bool Large { get; set; }

        public bool EnableUnobtrusiveValidation { get; set; }

        private SimpleModalDialogLayoutModel(RazorPage page)
        {
            SubmitUrl = page.Context.Request.GetDisplayUrl();

            // запомним модель во ViewBag
            dynamic viewBag = page.ViewBag;
            viewBag.SimpleModalDialogLayoutModel = this;
        }

        public static SimpleModalDialogLayoutModel Create(RazorPage page, string title)
        {
            var model = new SimpleModalDialogLayoutModel(page)
            {
                Title = title,
                SubmitBtnName = "Применить"
            };
            return model;
        }

        public static SimpleModalDialogLayoutModel Get(RazorPage page)
        {
            dynamic viewBag = page.ViewBag;
            var model = viewBag.SimpleModalDialogLayoutModel as SimpleModalDialogLayoutModel;
            if (model == null)
            {
                throw new UserFriendlyException("Не удалось найти SimpleModalDialogLayoutModel");
            }
            return model;
        }
    }
}