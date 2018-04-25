using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;

namespace Zidium.UserAccount.Models.Controls
{
    public class FormGroupRender<TModel> where TModel : class
    {
        public string Name { get; protected set; }
        public string GroupCssClass { get; protected set; }
        public TModel Model { get; protected set; }

        protected string GroupTemplate { get; set; }
        protected HtmlHelper<TModel> Html { get; set; }

        protected MvcHtmlString LabelHtml { get; set; }
        protected MvcHtmlString TooltipHtml { get; set; }
        protected MvcHtmlString ControlHtml { get; set; }
        protected string ControlWidth { get; set; }
        protected string DisplayName { get; set; }

        public FormGroupRender(string name, string groupTemplate, HtmlHelper<TModel> html, TModel model, string displayName = null)
        {
            Name = name;
            GroupTemplate = groupTemplate;
            Html = html;
            Model = model;
            DisplayName = displayName;
        }

        public FormGroupRender<TModel> AddGroupCss(string css)
        {
            GroupCssClass = css;
            return this;
        }

        public FormGroupRender<TModel> Label(string text)
        {
            LabelHtml = new MvcHtmlString(text);
            return this;
        }

        public FormGroupRender<TModel> Label(Func<FormGroupRender<TModel>, HelperResult> control)
        {
            var result = control.Invoke(this);
            LabelHtml = MvcHtmlString.Create(result.ToHtmlString());
            return this;
        }

        public FormGroupRender<TModel> Tooltip(string text)
        {
            TooltipHtml = new MvcHtmlString(text);
            return this;
        }

        public FormGroupRender<TModel> Tooltip(Func<FormGroupRender<TModel>, HelperResult> control)
        {
            var result = control.Invoke(this);
            TooltipHtml = MvcHtmlString.Create(result.ToHtmlString());
            return this;
        }

        public FormGroupRender<TModel> Control(Func<FormGroupRender<TModel>, HelperResult> control)
        {
            var result = control.Invoke(this);
            ControlHtml = MvcHtmlString.Create(result.ToHtmlString());
            return this;
        }

        public FormGroupRender<TModel> Control(Func<FormGroupRender<TModel>, MvcHtmlString> control)
        {
            ControlHtml = control.Invoke(this);
            return this;
        }

        public FormGroupRender<TModel> SetControlWidth(string width)
        {
            ControlWidth = width;
            return this;
        }

        public FormGroupRender<TModel> Paragraph(string text)
        {
            var textEncoded = Html.Encode(text);
            ControlHtml = MvcHtmlString.Create($"<p class=\"form-control-static\">{textEncoded}</p>");
            return this;
        }

        public FormGroupRender<TModel> DropDownList(
            IEnumerable<SelectListItem> selectList,
            object htmlAttributes = null)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new { @class = "form-control" };
            }
            ControlHtml = Html.DropDownList(Name, selectList, htmlAttributes);
            return this;
        }

        public FormGroupRender<TModel> TextBox()
        {
            ControlHtml = Html.TextBox(Name, null, new { @class = "form-control" });
            return this;
        }

        public FormGroupRender<TModel> CheckBox()
        {
            ControlHtml = Html.CheckBox(Name, new { @class = "checkbox" });
            return this;
        }

        protected FormGroupModel GetFormGroupModel()
        {
            return new FormGroupModel()
            {
                Name = Name,
                CssClass = GroupCssClass,
                Label = LabelHtml ?? new MvcHtmlString(DisplayName),
                Control = ControlHtml,
                ControlWidth = ControlWidth,
                Tooltip = TooltipHtml
            };
        }

        public MvcHtmlString Partial()
        {
            var groupModel = GetFormGroupModel();
            var viewPath = $"~/Views/Controls/{GroupTemplate}.cshtml";
            var viewHtml = Html.Partial(viewPath, groupModel);
            return viewHtml;
        }

        public void Render()
        {
            var groupModel = GetFormGroupModel();
            var viewPath = $"~/Views/Controls/{GroupTemplate}.cshtml";
            Html.RenderPartial(viewPath, groupModel);
        }
    }
}