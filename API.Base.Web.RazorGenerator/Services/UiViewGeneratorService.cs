using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using API.Base.Files.Models.Entities;
using API.Base.Web.Base.Helpers;
using API.Base.Web.Base.Misc;
using API.Base.Web.Base.Models.Entities;
using API.Base.Web.RazorGenerator.Models;
using API.Base.Web.RazorGenerator.Models.Create;
using API.Base.Web.RazorGenerator.Models.Delete;
using API.Base.Web.RazorGenerator.Models.Details;
using API.Base.Web.RazorGenerator.Models.Display;
using API.Base.Web.RazorGenerator.Models.Edit;
using API.Base.Web.RazorGenerator.Models.Form;
using API.Base.Web.RazorGenerator.Models.Index;
using API.Base.Web.RazorGenerator.Models.Order;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Web.RazorGenerator.Services
{
    public interface IUiViewGeneratorService
    {
        Task<string> RenderView<TE>(IGenerableIndexView<TE> generableIndexView, string viewDirectory = null,
            string viewName = null) where TE : Entity;

        Task<string> WriteAndGetViewName(string razorViewContent, string viewDirectory = null, string viewName = null);
        Task GenerateFor<TE>(string viewName, IGenerableView<TE> generableView) where TE : Entity;

        void DeleteGeneratedView();
    }

    public class UiViewGeneratorService : IUiViewGeneratorService
    {
        public readonly IServiceProvider ServiceProvider;
        private const string TplDir = "RazorViewGenerator";

        protected virtual IViewRenderService ViewRenderService =>
            ServiceProvider.GetService<IViewRenderService>();

        public UiViewGeneratorService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task GenerateFor<TE>(string viewName, IGenerableView<TE> generableView) where TE : Entity
        {
            Console.WriteLine("UiViewGeneratorService.GenerateFor: " + generableView.GetType().Name);
            var viewDirectory = "Views/" + generableView.GetType().Name.Replace("Controller", "");
            Directory.CreateDirectory(viewDirectory);
            switch (viewName)
            {
                case "Index":
                    await RenderView((IGenerableIndexView<TE>) generableView, viewDirectory, viewName);
                    break;
                case "_Display":
                    await RenderView((IGenerableDisplayView<TE>) generableView, viewDirectory, viewName);
                    break;
                case "Details":
                    await RenderView((IGenerableDetailsView<TE>) generableView, viewDirectory, viewName);
                    break;
                case "_Form":
                    await RenderView((IGenerableFormView<TE>) generableView, viewDirectory, viewName);
                    break;
                case "Create":
                    await RenderView((IGenerableCreateView<TE>) generableView, viewDirectory, viewName);
                    break;
                case "Edit":
                    await RenderView((IGenerableEditView<TE>) generableView, viewDirectory, viewName);
                    break;
                case "Delete":
                    await RenderView((IGenerableDeleteView<TE>) generableView, viewDirectory, viewName);
                    break;
                case "ReOrder":
                    await RenderView((IGenerableOrderView<TE>) generableView, viewDirectory, viewName);
                    break;
                default:
                    throw new NotImplementedException(viewName);
            }

            Console.WriteLine($"Generated: {viewDirectory}/{viewName}.cshtml");
        }

        private string _generatedViewPath;

        public void DeleteGeneratedView()
        {
            if (File.Exists(_generatedViewPath))
            {
                File.Delete(_generatedViewPath);
            }
        }

        public async Task<string> RenderView<TE>(IGenerableDeleteView<TE> generableDeleteView,
            string viewDirectory = null, string viewName = null) where TE : Entity
        {
            var fullTypeName = typeof(TE).FullName;
            var entityName = typeof(TE).Name.Replace("Entity", "");
            var vm = new DeleteTemplateModel(fullTypeName, entityName, entityName);

            return await RenderGenericView(vm, viewName, viewDirectory);
        }


        public async Task<string> RenderView<TE>(IGenerableEditView<TE> generableEditView,
            string viewDirectory = null, string viewName = null) where TE : Entity
        {
            var fullTypeName = typeof(TE).FullName;
            var entityName = typeof(TE).Name.Replace("Entity", "");
            var vm = new EditTemplateModel(fullTypeName, entityName, entityName);

            return await RenderGenericView(vm, viewName, viewDirectory);
        }

        public async Task<string> RenderView<TE>(IGenerableCreateView<TE> generableCreateView,
            string viewDirectory = null, string viewName = null) where TE : Entity
        {
            var fullTypeName = typeof(TE).FullName;
            var entityName = typeof(TE).Name.Replace("Entity", "");
            var vm = new CreateTemplateModel(fullTypeName, entityName, entityName);

            return await RenderGenericView(vm, viewName, viewDirectory);
        }

        public async Task<string> RenderView<TE>(IGenerableFormView<TE> generableFormView,
            string viewDirectory = null, string viewName = null) where TE : Entity
        {
            var fullTypeName = typeof(TE).FullName;
            var entityName = typeof(TE).Name.Replace("Entity", "");
            var vm = new FormTemplateModel(fullTypeName, entityName, entityName)
                {Properties = generableFormView.FormProperties};

            vm.ImportHtmlHelpers =
                typeof(TE).GetProperties().Any(prop =>
                    prop.PropertyType.IsSubclassOf(typeof(Entity)) || prop.GetCustomAttributes<DataTypeAttribute>().Any(
                        attr =>
                            attr != null && attr.DataType == DataType.Html));
            vm.InjectDataLayer = typeof(TE).GetProperties().Any(prop =>
                prop.PropertyType.IsSubclassOf(typeof(Entity)) && !prop.PropertyType.IsSubclassOf(typeof(FileEntity)));

            return await RenderGenericView(vm, viewName, viewDirectory);
        }

        public async Task<string> RenderView<TE>(IGenerableDetailsView<TE> generableDetailsView,
            string viewDirectory = null, string viewName = null) where TE : Entity
        {
            var fullTypeName = typeof(TE).FullName;
            var entityName = typeof(TE).Name.Replace("Entity", "");
            var vm = new DetailsTemplateModel(fullTypeName, entityName, entityName)
            {
                Actions = generableDetailsView.DetailsActions
            };

            return await RenderGenericView(vm, viewName, viewDirectory);
        }

        public async Task<string> RenderView<TE>(IGenerableDisplayView<TE> generableDisplayView,
            string viewDirectory = null, string viewName = null) where TE : Entity
        {
            var fullTypeName = typeof(TE).FullName;
            var entityName = typeof(TE).Name.Replace("Entity", "");
            var vm = new DisplayTemplateModel(fullTypeName, entityName, entityName)
                {Properties = generableDisplayView.DisplayProperties};

            return await RenderGenericView(vm, viewName, viewDirectory);
        }

        public async Task<string> RenderView<TE>(IGenerableOrderView<TE> generableOrderView,
            string viewDirectory = null,
            string viewName = null) where TE : Entity
        {
            var fullTypeName = typeof(TE).FullName;
            var entityName = typeof(TE).Name.Replace("Entity", "");
            if (!typeof(IOrderedEntity).IsAssignableFrom(typeof(TE)))
            {
                Console.WriteLine("Skipping ReOrder view generation. '" + fullTypeName + "' does not implement IOrderedEntity");
                return null;
            }

            var vm = new OrderTemplateModel(fullTypeName, entityName, entityName);

            return await RenderGenericView(vm, viewName, viewDirectory);
        }
        
        public async Task<string> RenderView<TE>(IGenerableIndexView<TE> generableIndexView,
            string viewDirectory = null,
            string viewName = null) where TE : Entity
        {
            var fullTypeName = typeof(TE).FullName;
            var entityName = typeof(TE).Name.Replace("Entity", "");

            var vm = new IndexTemplateModel(fullTypeName, entityName, entityName)
            {
                Actions = generableIndexView.ListItemActions,
                Columns = generableIndexView.ListColumns,
                TopLinks = generableIndexView.TopLinks,
                IsOrdered = typeof(IOrderedEntity).IsAssignableFrom(typeof(TE))
            };

            return await RenderGenericView(vm, viewName, viewDirectory);
        }

        public async Task<string> RenderGenericView(BaseTemplateModel model, string viewName, string viewDirectory)
        {
            var tplName = viewName + "ViewTemplate";
            var content = await ViewRenderService.RenderToStringAsync(Path.Combine(TplDir, tplName), model);
            return await WriteAndGetViewName(content, viewDirectory, viewName);
        }

        public async Task<string> WriteAndGetViewName(string razorViewContent, string viewDirectory = null,
            string viewName = null)
        {
            viewName = viewName ?? "generated_" + DateTime.Now.Ticks + "_" + Utilis.GenerateRandomHexString(25);
            viewDirectory = viewDirectory ??
                            Path.Combine(EnvVarManager.GetOrThrow("TEMPORARY_VIEWS_PATH"), "Views/Shared");
            var viewPath =
                Path.Combine(viewDirectory, viewName + ".cshtml")
                    .Replace("\\", "/");

            _generatedViewPath = viewPath;
            using (var fs = new FileStream(viewPath, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                await sw.WriteAsync(razorViewContent);
            }

            return viewName;
        }
    }
}