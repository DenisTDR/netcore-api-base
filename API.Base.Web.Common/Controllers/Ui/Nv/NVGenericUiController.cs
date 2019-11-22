using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using API.Base.Web.Base.Attributes;
using API.Base.Web.Base.Controllers.Ui;
using API.Base.Web.Base.Extensions.ReflectionExtensions;
using API.Base.Web.Base.Models;
using API.Base.Web.Base.Models.Entities;
using API.Base.Web.RazorGenerator.Models;
using API.Base.Web.RazorGenerator.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Web.Common.Controllers.Ui.Nv
{
    public class NvGenericUiController<TE> : GenericUiController<TE>, IGenerableViews<TE>
        where TE : Entity
    {
        protected virtual IUiViewGeneratorService UiViewGeneratorService =>
            ServiceProvider.GetService<IUiViewGeneratorService>();

        protected bool UseInheritedProperties { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            BuildGenerableMetadata();
        }

        public void BuildGenerableMetadata()
        {
            ListItemActions = new List<string>();
            DetailsActions = new List<string>();
            SetListItemActions();

            ListColumns = new List<PropertyInfo>();
            SetListColumns();

            DisplayProperties = new List<PropertyInfo>();
            SetDisplayProperties();

            FormProperties = new List<PropertyInfo>();
            SetFormProperties();

            TopLinks = new List<AdminDashboardLink>();
            SetTopLinks();
        }

        protected virtual void SetTopLinks()
        {
        }

        protected virtual void SetListItemActions()
        {
            ListItemActions = DetailsActions = new List<string> {"Edit", "Details", "Delete"};
        }

        protected virtual void SetListColumns()
        {
            var flags = BindingFlags.Public | BindingFlags.Instance |
                        (UseInheritedProperties ? BindingFlags.Default : BindingFlags.DeclaredOnly);
            ListColumns = typeof(TE).GetProperties(flags).ToList();
            if (typeof(IOrderedEntity).IsAssignableFrom(typeof(TE)))
            {
                ListColumns.Remove(typeof(TE).GetProperty(nameof(IOrderedEntity.OrderIndex)));
            }
        }

        protected void AddListColumn<TProperty>(Expression<Func<TE, TProperty>> navigationPropertyPath)
        {
            ListColumns.Add(SymbolExtensions.GetPropertyInfo(navigationPropertyPath));
        }

        protected void AddListColumns<TProperty>(params Expression<Func<TE, TProperty>>[] navigationPropertyPaths)
        {
            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                AddListColumn(navigationPropertyPath);
            }
        }

        protected virtual void SetFormProperties()
        {
            var flags = BindingFlags.Public | BindingFlags.Instance |
                        (UseInheritedProperties ? BindingFlags.Default : BindingFlags.DeclaredOnly);

            FormProperties = typeof(TE).GetProperties(flags)
                .Where(p => p.CustomAttributes.All(a => a.AttributeType != typeof(IsReadOnlyAttribute))).ToList();
            if (typeof(IOrderedEntity).IsAssignableFrom(typeof(TE)))
            {
                FormProperties.Remove(typeof(TE).GetProperty(nameof(IOrderedEntity.OrderIndex)));
            }
        }

        protected virtual void SetDisplayProperties()
        {
            DisplayProperties = typeof(TE).GetProperties().ToList();
        }

        public IList<AdminDashboardLink> TopLinks { get; set; }
        public IList<string> ListItemActions { get; set; }
        public IList<PropertyInfo> ListColumns { get; set; }
        public IList<PropertyInfo> DisplayProperties { get; set; }
        public IList<string> DetailsActions { get; set; }
        public IList<PropertyInfo> FormProperties { get; set; }
    }
}