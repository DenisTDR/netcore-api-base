using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API.Base.Web.Base.Controllers.Api;
using API.Base.Web.Base.Extensions;
using API.Base.Web.Base.Models;
using API.Base.Web.Base.Models.EntityMaps;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Web.Base.ApiBuilder
{
    public abstract class ApiSpecifications
    {
        public IConfiguration Configuration { get; private set; }

        public virtual void Init()
        {
        }

        public void SetConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual void ConfigMvc(MvcOptions options)
        {
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void ConfigureApp(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            this.ConfigureApp(app, serviceProvider.GetService<IHostingEnvironment>());
        }

        public virtual void ConfigureApp(IApplicationBuilder app, IHostingEnvironment env)
        {
        }

        public virtual IMvcBuilder MvcChain(IMvcBuilder source)
        {
            return source;
        }

        public virtual AdminDashboardSection RegisterAdminDashboardSection()
        {
            return null;
        }

        public virtual List<AdminDashboardSection> RegisterAdminDashboardSections()
        {
            return new List<AdminDashboardSection>();
        }

        public IEnumerable<Type> GetEntityTypesConfigurationsFromAssembly(Assembly assembly)
        {
            return assembly.GetSubTypes(typeof(EntityTypeConfiguration<>));
        }

        public IEnumerable<Type> GetAutoMapperTypesFromAssembly(Assembly assembly)
        {
            return assembly.GetSubTypes(typeof(EntityViewModelMap<,>));
        }

        public Assembly Assembly => this.GetType().Assembly;
    }
}