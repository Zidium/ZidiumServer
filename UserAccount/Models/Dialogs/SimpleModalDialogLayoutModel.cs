using System.Web.Mvc;
using Zidium.Core;

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

        private SimpleModalDialogLayoutModel(WebViewPage page)
        {
            SubmitUrl = page.Context.Request.RawUrl;

            // запомним модель во ViewBag
            dynamic viewBag = page.ViewBag;
            viewBag.SimpleModalDialogLayoutModel = this;
        }

        public static SimpleModalDialogLayoutModel Create(WebViewPage page, string title)
        {
            var model = new SimpleModalDialogLayoutModel(page)
            {
                Title = title,
                SubmitBtnName = "Применить"
            };
            return model;
        }

        public static SimpleModalDialogLayoutModel Get(WebViewPage page)
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