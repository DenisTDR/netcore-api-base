using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using API.Base.Web.Base.Extensions.ReflectionExtensions;
using API.Base.Web.Base.Models.Entities;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace API.Base.Web.Base.Extensions.HtmlHelpers
{
    public static class HtmlHelpersExtensions
    {
        public static string DisplayLNameForProp(
            this IHtmlHelper<dynamic> htmlHelper,
            PropertyInfo propertyInfo)
        {
            var propertySelectorCreationMethod = typeof(ExpressionBuilders)
                .GetMethod(nameof(ExpressionBuilders.CreatePropertySelector))
                .MakeGenericMethod(propertyInfo.DeclaringType, propertyInfo.PropertyType);

            var propertySelector = propertySelectorCreationMethod.Invoke(null, new object[] {propertyInfo});

            var expression = (dynamic) propertySelector;


            var result = htmlHelper.DisplayNameFor(expression);

            return result;
//            return DisplayLNameFor(htmlHelper, expression);
        }

//        public static string DisplayLNameFor<TModelItem, TResult>(
//            this IHtmlHelper<IEnumerable<TModelItem>> htmlHelper,
//            Expression<Func<TModelItem, TResult>> expression)
//        {
//            return htmlHelper.DisplayNameFor(expression);
//        }

        public static IHtmlContent DisplayLForProp<TModelItem>(
            this IHtmlHelper<IEnumerable<TModelItem>> htmlHelper,
            PropertyInfo propertyInfo,
            TModelItem item) where TModelItem : Entity
        {
            var expressionMethod = typeof(ExpressionBuilders)
                .GetMethod(nameof(ExpressionBuilders.BuildDisplayLForPropExpression))
                .MakeGenericMethod(typeof(TModelItem), propertyInfo.PropertyType);

            Expression<Func<TModelItem>> getItemExpr = () => item;

            var propertySelector = expressionMethod.Invoke(null, new object[] {propertyInfo, getItemExpr});

            var expression = (dynamic) propertySelector;

            return DisplayLFor(htmlHelper, expression);
        }


        public static IHtmlContent DisplayLFor<TModelItem, TResult>(
            this IHtmlHelper<IEnumerable<TModelItem>> htmlHelper,
            Expression<Func<IEnumerable<TModelItem>, TResult>> expression)
        {
            return htmlHelper.DisplayFor(expression);
        }

        public static string DisplayShort<TModelItem>(
            this IHtmlHelper<IEnumerable<TModelItem>> htmlHelper, IHtmlContent content)
        {
            var writer = new System.IO.StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            var str = writer.ToString();
            if (str.Length < 10)
            {
                return str;
            }

            str = str.Substring(0, 4) + "..." + str.Substring(str.Length - 5, 4);
            return str;
        }

        private static Random _random = new Random();

        public static int RandomId(
            this IHtmlHelper htmlHelper,
            int min = 10 * 1000, int max = 1000 * 1000)
        {
            lock (_random)
            {
                return _random.Next(min, max);
            }
        }

        public static async Task RenderBlindPartialAsync(this IHtmlHelper htmlHelper, string partialViewName,
            object model, ViewDataDictionary viewData = null)
        {
            await htmlHelper.RenderPartialAsync(partialViewName, model, viewData);
        }
        public static string DisplayForBool(this IHtmlHelper htmlHelper, bool? property)
        {
            return property != null && property == true ? "Yes" : "No";
        }
    }
}