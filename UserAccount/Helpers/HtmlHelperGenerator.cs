using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.Extensions.Options;

namespace Zidium.UserAccount.Helpers
{
    public class HtmlHelperGenerator
    {
        private readonly IHtmlGenerator _htmlGenerator;
        private readonly ICompositeViewEngine _compositeViewEngine;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IViewBufferScope _viewBufferScope;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly HtmlHelperOptions _htmlHelperOptions;

        public HtmlHelperGenerator(IHtmlGenerator htmlGenerator, ICompositeViewEngine compositeViewEngine, IModelMetadataProvider modelMetadataProvider, IViewBufferScope viewBufferScope, IActionContextAccessor actionContextAccessor, IOptions<MvcViewOptions> options)
        {
            _htmlGenerator = htmlGenerator;
            _compositeViewEngine = compositeViewEngine;
            _modelMetadataProvider = modelMetadataProvider;
            _viewBufferScope = viewBufferScope;
            _actionContextAccessor = actionContextAccessor;
            _htmlHelperOptions = options.Value.HtmlHelperOptions;
        }
        public IHtmlHelper HtmlHelper(ViewDataDictionary ViewData, ITempDataDictionary TempData)
        {
            var helper = new HtmlHelper(_htmlGenerator, _compositeViewEngine, _modelMetadataProvider, _viewBufferScope, HtmlEncoder.Default, UrlEncoder.Default);
            var viewContext = new ViewContext(_actionContextAccessor.ActionContext,
               new FakeView(),
               ViewData,
               TempData,
               TextWriter.Null,
               _htmlHelperOptions);
            helper.Contextualize(viewContext);
            return helper;
        }
        private class FakeView : IView
        {
            public string Path => "View";

            public Task RenderAsync(ViewContext context)
            {
                return Task.FromResult(0);
            }
        }
    }
}
