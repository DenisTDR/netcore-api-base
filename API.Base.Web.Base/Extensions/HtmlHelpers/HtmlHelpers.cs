using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace API.Base.Web.Base.Extensions.HtmlHelpers
{
    public static class HtmlHelpers
    {
        public static IHtmlContent DescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var modelExplorer =
                ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);
            if (modelExplorer == null)
                throw new InvalidOperationException(
                    $"Failed to get model explorer for {ExpressionHelper.GetExpressionText(expression)}");

            return new HtmlString($"<small class='form-text text-muted'>{modelExplorer.Metadata.Description}</small>");
        }
//        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper)
//        {
//            return HtmlHelperFor(htmlHelper, default(TModel));
//        }
//
//        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper, TModel model)
//        {
//            return HtmlHelperFor(htmlHelper, model, null);
//        }

//        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper)
//        {
//            var helper = holder
//        }

//        public HtmlHelper(IHtmlGenerator htmlGenerator, ICompositeViewEngine viewEngine,
// IModelMetadataProvider metadataProvider, IViewBufferScope bufferScope, HtmlEncoder htmlEncoder,
// UrlEncoder urlEncoder, ExpressionTextCache expressionTextCache)
//            : base(htmlGenerator, viewEngine, metadataProvider, bufferScope, htmlEncoder, urlEncoder)
//        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper, TModel model, string none)
//        {
//            var helper = new HtmlHelper<TModel>(null, new CompositeViewEngine(), htmlHelper.MetadataProvider, null,
//                null, htmlHelper.UrlEncoder, null);
//
//            helper.Contextualize(htmlHelper.ViewContext);
////            helper.
//
//            return null;
//        }

//        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper, TModel model, string htmlFieldPrefix) {
//
//            var viewDataContainer = CreateViewDataContainer(htmlHelper.ViewData, model);
//
//            TemplateInfo templateInfo = viewDataContainer.ViewData.TemplateInfo;
//
//            if (!String.IsNullOrEmpty(htmlFieldPrefix))
//                templateInfo.HtmlFieldPrefix = templateInfo.GetFullHtmlFieldName(htmlFieldPrefix);
//
//            ViewContext viewContext = htmlHelper.ViewContext;
//            ViewContext newViewContext = new ViewContext(viewContext.Controller.ControllerContext, viewContext.View, viewDataContainer.ViewData, viewContext.TempData, viewContext.Writer);
//
//            return new HtmlHelper<TModel>(newViewContext, viewDataContainer, htmlHelper.RouteCollection);
//        }
//
//        static IViewDataContainer CreateViewDataContainer(ViewDataDictionary viewData, object model) {
//
//            var newViewData = new ViewDataDictionary(viewData) {
//                Model = model
//            };
//
//            newViewData.TemplateInfo = new TemplateInfo { 
//                HtmlFieldPrefix = newViewData.TemplateInfo.HtmlFieldPrefix 
//            };
//
//            return new ViewDataContainer {
//                ViewData = newViewData
//            };
//        }
//
//        class ViewDataContainer : IViewDataContainer {
//
//            public ViewDataDictionary ViewData { get; set; }
//        }
    }
}