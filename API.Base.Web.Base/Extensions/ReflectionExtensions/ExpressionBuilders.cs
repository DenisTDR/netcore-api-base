using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace API.Base.Web.Base.Extensions.ReflectionExtensions
{
    public static class ExpressionBuilders
    {
        // create a lambda expression of the form (o => o.propertyName)
        // TIn - object type
        // TOut - property type
        public static Expression<Func<TIn, TOut>> CreatePropertySelectorByName<TIn, TOut>(string propertyName)
        {
            var param = Expression.Parameter(typeof(TIn));
            var body = Expression.PropertyOrField(param, propertyName);
            return Expression.Lambda<Func<TIn, TOut>>(body, param);
        }

        // create a lambda expression of the form (o => o.propertyName)
        // TIn - object type
        // TOut - property type
        public static Expression<Func<TIn, TOut>> CreatePropertySelector<TIn, TOut>(PropertyInfo propertyInfo)
        {
            var param = Expression.Parameter(typeof(TIn));
            var body = Expression.Property(param, propertyInfo);
            return Expression.Lambda<Func<TIn, TOut>>(body, param);
        }

        // create a lambda expression of the form (modelItem => item.propertyName)
        // modelItem - some non-used IEnumerable<TModelItem>, it is there only for it's static type
        // getItem - a 'getter' for the 'item' object
        // TModelItem - item type
        // TResult - property type
        public static Expression<Func<IEnumerable<TModelItem>, TResult>>
            BuildDisplayLForPropExpression<TModelItem, TResult>(PropertyInfo propertyInfo,
                Expression<Func<TModelItem>> getItem)
        {
            var param = Expression.Parameter(typeof(IEnumerable<TModelItem>));
            var itemExpr = getItem.Body;
            var body = Expression.Property(itemExpr, propertyInfo);
            return Expression.Lambda<Func<IEnumerable<TModelItem>, TResult>>(body, param);
        }
    }
}