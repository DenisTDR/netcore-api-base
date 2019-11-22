using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.ApiBuilder;
using API.Base.Web.Base.Extensions;
using API.Base.Web.Base.Helpers;
using API.Base.Web.Base.Misc;
using API.Base.Web.RazorGenerator.Extensions;
using API.Base.Web.RazorGenerator.Models;
using API.Base.Web.RazorGenerator.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace API.Base.Web.RazorGenerator
{
    public class RazorGeneratorApiSpecifications : ApiSpecifications
    {
        public override void ConfigMvc(MvcOptions options)
        {
//            options.Filters.Add<GeneratedTemporaryViewDeleteFilter>();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddScoped<IUiViewGeneratorService, UiViewGeneratorService>();
            AddTmpViewsDirectory(services);

            if (IsGeneratorActivated)
            {
                services.AddTransient(GetControllerType());
            }
        }

        public override void ConfigureApp(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            var appLifeTime = serviceProvider.GetService<IApplicationLifetime>();

            if (!IsGeneratorActivated) return;

            Console.WriteLine("Razor View Generator activated");
            GenerateViews(serviceProvider);
            appLifeTime.StopApplication();
        }

        private bool IsGeneratorActivated => Configuration.GetValue<bool>("generate-razor-view");

        private Type GetControllerType()
        {
            var controller = Configuration.GetValue<string>("controller");

            if (string.IsNullOrEmpty(controller))
            {
                Utilis.DieWith("Missing --controller argument");
            }

            if (!controller.EndsWith("Controller")) controller += "Controller";
            var controllerType = GetTypeExtensions.GetType(controller);
            if (controllerType == null)
            {
                Utilis.DieWith("Couldn't locate controller type '" + controller + "'");
            }

            return controllerType;
        }

        private void GenerateViews(IServiceProvider serviceProvider)
        {
            var viewNames = Configuration.GetValue<string>("view-names");

            if (string.IsNullOrEmpty(viewNames))
            {
                Utilis.DieWith("Missing --view-names argument");
            }

            if (viewNames == "all")
            {
                viewNames = "_Display,_Form,Create,Delete,Details,Edit,Index,ReOrder";
            }

            foreach (var vn in viewNames.Split(','))
            {
                GenerateView(serviceProvider, vn);
            }
        }

        private void GenerateView(IServiceProvider serviceProvider, string viewName)
        {
            var controllerType = GetControllerType();
            if (!typeof(IGenerableView).IsAssignableFrom(controllerType))
            {
                Utilis.DieWith("Controller type '" + controllerType + "' doesn't implement '" + typeof(IGenerableView) +
                               "'");
            }

            var controllerInstance = (IGenerableView) serviceProvider.GetService(controllerType);
            controllerInstance.SetServiceProvider(serviceProvider);
            controllerInstance.BuildGenerableMetadata();

            if (!controllerType.ImplementsGenericInterface(typeof(IGenerableView<>)))
            {
                throw new Exception($"{controllerType.Name} is not a {typeof(IGenerableView<>).Name}");
            }

            var methodInfo = controllerType.GetMethods().FirstOrDefault(m => m.Name == viewName);
            if (!viewName.StartsWith("_") && methodInfo == null)
            {
                throw new Exception($"Invalid viewName '{viewName}' on controller '{controllerType.FullName}'");
            }

            var entityType =
                controllerType.GetGenericArgumentTypeOfImplementedGenericInterface(typeof(IGenerableView<>));

            var mi = typeof(IUiViewGeneratorService).GetMethod(nameof(IUiViewGeneratorService.GenerateFor));
            mi = mi.MakeGenericMethod(entityType);


            var viewGenerator = serviceProvider.GetService<IUiViewGeneratorService>();

            var task = (Task) mi.Invoke(viewGenerator, new object[] {viewName, controllerInstance});
            task.Wait();
        }

        protected virtual void AddTmpViewsDirectory(IServiceCollection services)
        {
            var tmpViewsPath = EnvVarManager.GetOrThrow("TEMPORARY_VIEWS_PATH");
            var sharedViewsDirectory = Path.Combine(tmpViewsPath, "Views", "Shared");
            if (!Directory.Exists(sharedViewsDirectory))
            {
                Console.WriteLine("Creating directory " + sharedViewsDirectory);
                Directory.CreateDirectory(sharedViewsDirectory);
            }

            var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), tmpViewsPath));

//            Console.WriteLine("Temporary views in " + path);
            var fileProvider = new PhysicalFileProvider(path);
            services.Configure<RazorViewEngineOptions>(options => { options.FileProviders.Add(fileProvider); });
        }
    }
}