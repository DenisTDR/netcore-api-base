using System;
using System.Collections.Generic;
using System.IO;
using API.Base.Web.Base.ApiBuilder;
using API.Base.Web.Base.Controllers.Ui;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Filters;
using API.Base.Web.Base.Helpers;
using API.Base.Web.Base.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace API.Base.Web.Base
{
    internal class WebBaseApiSpecifications : ApiSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataLayer, DatabaseLayer>();

            AddViewsFromEnvVar(services);

            services.AddCors();
        }

        public override void ConfigMvc(MvcOptions options)
        {
            options.Filters.Add<CustomExceptionFilter>();
            options.Filters.Add<ResultWrapperFilter>();
        }

        public override void ConfigureApp(IApplicationBuilder app, IHostingEnvironment env)
        {
            AddCorsFromEnv(app);

            AddWwwRootsFromEnvVar(app);

            RegisterContentDirectory(app, env);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/admin/{controller=AdminDashboard}/{action=Index}/{id?}");
            });

            var option = new RewriteOptions();
            option.AddRedirect("^$", "/api/docs");
            option.AddRedirect("^api/$", "/api/docs");
            app.UseRewriter(option);
        }

        private void RegisterContentDirectory(IApplicationBuilder app, IHostingEnvironment env)
        {
            var contentPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
                EnvVarManager.GetOrThrow("CONTENT_DIRECTORY")));
            if (!Directory.Exists(contentPath))
            {
                Console.WriteLine("Creating CONTENT_DIRECTORY: " + contentPath);
                Directory.CreateDirectory(contentPath);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(contentPath),
                RequestPath = "/content",
                ServeUnknownFileTypes = true
            });
            if (env.IsDevelopment())
            {
                app.UseDirectoryBrowser(new DirectoryBrowserOptions
                {
                    FileProvider = new PhysicalFileProvider(contentPath),
                    RequestPath = "/content"
                });
            }
        }

        private void AddViewsFromEnvVar(IServiceCollection services)
        {
            var viewsAndWwwPaths = EnvVarManager.Get("VIEWS_AND_WWW_PATHS");
            if (!string.IsNullOrEmpty(viewsAndWwwPaths))
            {
                foreach (var projectDirectory in viewsAndWwwPaths.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), projectDirectory));
                    if (!Directory.Exists(path))
                    {
                        continue;
                    }

//                    Console.WriteLine("Views in " + path);
                    var fileProvider = new PhysicalFileProvider(path);
                    services.Configure<RazorViewEngineOptions>(options => { options.FileProviders.Add(fileProvider); });
                }
            }
        }

        private void AddWwwRootsFromEnvVar(IApplicationBuilder app)
        {
            var viewsAndWwwPaths = EnvVarManager.Get("VIEWS_AND_WWW_PATHS");
            if (!string.IsNullOrEmpty(viewsAndWwwPaths))
            {
                foreach (var projectDirectory in viewsAndWwwPaths.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), projectDirectory,
                        "wwwroot"));
                    if (!Directory.Exists(path))
                    {
                        continue;
                    }

//                    Console.WriteLine("using: " + path);
                    app.UseStaticFiles(new StaticFileOptions()
                        {FileProvider = new PhysicalFileProvider(path), RequestPath = ""});
                }
            }
        }

        private void AddCorsFromEnv(IApplicationBuilder app)
        {
            var corsHostsStr = EnvVarManager.Get("ALLOWED_CORS_HOSTS");
            if (string.IsNullOrEmpty(corsHostsStr))
            {
                return;
            }

            var corsHosts = corsHostsStr.Split(';');
            app.UseCors(builder =>
            {
                foreach (var corsHost in corsHosts)
                {
                    if (corsHost == "*")
                    {
                        builder = builder.AllowAnyOrigin().AllowAnyHeader();
                    }
                    else
                    {
                        builder = builder.WithOrigins(corsHost.Trim()).AllowAnyHeader();
                    }
                }
            });
        }

        public override AdminDashboardSection RegisterAdminDashboardSection()
        {
            return new AdminDashboardSection
            {
                Name = "Base",
                Links = new List<AdminDashboardLink>
                {
                    new AdminDashboardLink("Users", nameof(UsersUiController),
                        nameof(UsersUiController.Users)),
                    new AdminDashboardLink("Generate seed", nameof(AdminDashboardController),
                        nameof(AdminDashboardController.Seed)),
                }
            };
        }
    }
}