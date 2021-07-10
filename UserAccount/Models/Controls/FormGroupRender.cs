using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zidium.UserAccount.Models.Controls
{
    public class FormGroupRender<TModel> where TModel : class
    {
        public string Name { get; protected set; }
        public string GroupCssClass { get; protected set; }
        public TModel Model { get; protected set; }

        protected string GroupTemplate { get; set; }
        protected IHtmlHelper<TModel> Html { get; set; }

        protected IHtmlContent LabelHtml { get; set; }
        protected IHtmlContent TooltipHtml { get; set; }
        protected IHtmlContent ControlHtml { get; set; }
        protected string ControlWidth { get; set; }
        protected string DisplayName { get; set; }

        public FormGroupRender(string name, string groupTemplate, IHtmlHelper<TModel> html, TModel model, string displayName = null)
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
            LabelHtml = new HtmlString(text);
            return this;
        }

        public FormGroupRender<TModel> Label(Func<FormGroupRender<TModel>, HelperResult> control)
        {
            var result = control.Invoke(this);
            LabelHtml = result;
            return this;
        }

        public FormGroupRender<TModel> Tooltip(string text)
        {
            TooltipHtml = new HtmlString(text);
            return this;
        }

        public FormGroupRender<TModel> Tooltip(Func<FormGroupRender<TModel>, HelperResult> control)
        {
            var result = control.Invoke(this);
            TooltipHtml = result;
            return this;
        }

        public FormGroupRender<TModel> Control(Func<FormGroupRender<TModel>, HelperResult> control)
        {
            var result = control.Invoke(this);
            ControlHtml = result;
            return this;
        }

        public FormGroupRender<TModel> Control(Func<FormGroupRender<TModel>, IHtmlContent> control)
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
            ControlHtml = new HtmlString($"<p class=\"form-control-static\">{textEncoded}</p>");
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
                Label = LabelHtml ?? new HtmlString(DisplayName),
                Control = ControlHtml,
                ControlWidth = ControlWidth,
                Tooltip = TooltipHtml
            };
        }

        public IHtmlContent Partial()
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