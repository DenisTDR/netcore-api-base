using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API.Base.Web.Base.ApiBuilder.AppFeatureProvider;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Database;
using API.Base.Web.Base.Extensions;
using API.Base.Web.Base.Helpers;
using API.Base.Web.Base.Models;
using API.Base.Web.Base.Models.EntityMaps;
using API.Base.Web.Base.Swagger;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace API.Base.Web.Base.ApiBuilder
{
    public class ApiBuilder
    {
        internal static ApiBuilder Instance { get; private set; }

        private readonly Assembly _mainAssembly;

        private readonly List<EntityTypeStackConfiguration> _typeStackConfigurations =
            new List<EntityTypeStackConfiguration>();

        private readonly List<ApiSpecifications> _specifications = new List<ApiSpecifications>();
        private readonly IConfiguration _configuration;

        public ApiBuilder(Assembly assembly, IConfiguration configuration)
        {
            _mainAssembly = assembly;
            _configuration = configuration;
            Instance = this;
        }

        public ApiBuilder BuildServices(IServiceCollection services)
        {
            if (_specifications.FirstOrDefault(spec => spec is WebBaseApiSpecifications) == null)
            {
                this.AddSpecifications<WebBaseApiSpecifications>();
            }

            _processEntities();
            var mvcBuilder = services.AddMvc(mvcOptions =>
            {
                foreach (var apiSpecifications in _specifications)
                {
                    apiSpecifications.ConfigMvc(mvcOptions);
                }
            }).AddJsonOptions(mvcJsonOptions =>
            {
                mvcJsonOptions.SerializerSettings.Converters.Add(
                    new Newtonsoft.Json.Converters.StringEnumConverter());
                mvcJsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                mvcJsonOptions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                mvcJsonOptions.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            });
            foreach (var apiSpecifications in _specifications)
            {
                mvcBuilder = apiSpecifications.MvcChain(mvcBuilder);
            }

            mvcBuilder.ConfigureApplicationPartManager(apm =>
                apm.FeatureProviders.Add(new DisabledControllersFeatureProvider(_typeStackConfigurations)));

            var seedingApiSpec = _specifications.LastOrDefault(spec => spec.AddDbSeederAction != null);
            if (seedingApiSpec != null)
            {
                _addDbSeeder = seedingApiSpec.AddDbSeederAction;
            }

            _registerAutoMapperPairs();
            _registerDbContext(services);
            if (_useSwagger)
            {
                new ApiBuilderSwaggerHelper(_swaggerSpecs).Bind(services);
            }

            services.AddOptions<SwaggerSpecs>().Configure(sc =>
            {
                foreach (var propertyInfo in sc.GetType().GetProperties())
                {
                    propertyInfo.SetValue(sc, propertyInfo.GetValue(_swaggerSpecs));
                }

                sc.IndexStreamAction = null;
            });

            foreach (var apiSpecifications in _specifications)
            {
                apiSpecifications.ConfigureServices(services);
            }

            services.AddOptions<AdminDashboardConfig>().Configure(_processAdminDashboardSections);

            services.AddOptions<List<EntityTypeStackConfiguration>>().Configure(c =>
            {
                c.AddRange(_typeStackConfigurations);
            });

            return this;
        }


        public ApiBuilder BuildApp(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime applicationLifetime, IDataSeeder seeder, IServiceProvider serviceProvider)
        {
            if (_useSwagger)
            {
                new ApiBuilderSwaggerHelper(_swaggerSpecs).Bind(app, env);
            }

            foreach (var apiSpecifications in _specifications)
            {
                apiSpecifications.ConfigureApp(app, serviceProvider);
            }

            var shouldMigrate = _configuration.GetValue<bool>("migrate");
            var shouldLoadSeed = _configuration.GetValue<bool>("seed");

            if (shouldMigrate)
            {
                Console.WriteLine("Migrating...");
                seeder.MigrateDatabase().Wait();
                Console.WriteLine("Migrating done.");
                if (!shouldLoadSeed)
                {
                    applicationLifetime.StopApplication();
                }
            }
            else
            {
                seeder.EnsureMigrated().Wait();
            }

            if (shouldLoadSeed)
            {
                Console.WriteLine("Seeding data...");
                seeder.LoadSeed().Wait();
                Console.WriteLine("Seeding done.");
                applicationLifetime.StopApplication();
            }


            var generateSeed = _configuration.GetValue<string>("generate-seed");
            if (generateSeed != null)
            {
                Console.WriteLine("Generating seed...");
                seeder.SeedToFile(generateSeed).Wait();
                Console.WriteLine("Generating seed done.");
                applicationLifetime.StopApplication();
            }

            var adminEmail = _configuration.GetValue<string>("give-admin");
            if (adminEmail != null)
            {
                Console.WriteLine("Giving admin to " + adminEmail + "...");
                var userManager = serviceProvider.GetService<UserManager<User>>();
                var user = userManager.FindByEmailAsync(adminEmail).Result;
                userManager.AddToRoleAsync(user, "Admin").Wait();
                userManager.AddToRoleAsync(user, "Staff").Wait();
                userManager.AddToRoleAsync(user, "Moderator").Wait();
                userManager.AddToRoleAsync(user, "User").Wait();
                Console.WriteLine("Done.");

                applicationLifetime.StopApplication();
            }

            // assert that variable is set correctly
            if (EnvVarManager.GetOrThrow("EXTERNAL_URL").EndsWith('/')
                || !EnvVarManager.GetOrThrow("EXTERNAL_URL").Contains("http"))
            {
                throw new Exception("EXTERNAL_URL must include protocol and must not end with /");
            }

            return this;
        }

        public ApiBuilder AddSpecifications(ApiSpecifications specifications)
        {
            _specifications.Add(specifications);

            return this;
        }

        public ApiBuilder AddSpecifications<T>() where T : ApiSpecifications, new()
        {
            var apiConf = new T();
            apiConf.SetConfiguration(_configuration);
            return this.AddSpecifications(apiConf);
        }

        private bool _useSwagger;
        private SwaggerSpecs _swaggerSpecs;

        public ApiBuilder UseSwagger(SwaggerSpecs swaggerSpecs)
        {
            _useSwagger = true;
            _swaggerSpecs = swaggerSpecs;
            return this;
        }

        private bool _useMySql;

        private string _connectionString;

//string connectionString = null
        public ApiBuilder UseMySql<T>() where T : DbContext
        {
            if (_useMySql)
            {
                throw new InvalidOperationException("UseMySql<T> already called on this ApiBuilder");
            }

            _useMySql = true;

            var connectionString =
                $"server={EnvVarManager.GetOrThrow("DB_SERVER")};" +
                $"port={EnvVarManager.GetOrThrow("DB_PORT")};" +
                $"database={EnvVarManager.GetOrThrow("DB_DATABASE")};" +
                $"uid={EnvVarManager.GetOrThrow("DB_USER")};" +
                $"password={EnvVarManager.Get("DB_PASSWORD")}";


            _connectionString = connectionString + (connectionString.EndsWith(";") ? "" : ";") +
                                "Persist Security Info=True;Convert Zero Datetime=True;charset=utf8";

            _addDbContextAction = services =>
            {
                services.AddDbContext<T>(optionsBuilder =>
                {
                    if (_useMySql)
                    {
                        PutMysql(optionsBuilder);
                    }

                    BaseDbContext.ConfigureBuilder = _registerEntityTypes;
                });
            };
            return this;
        }

        internal void PutMysql(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString, x => x.MigrationsAssembly(_mainAssembly.FullName));
        }

        private Action<IServiceCollection> _addDbSeeder = services => { services.AddTransient<IDataSeeder>(); };

        public ApiBuilder AddSeeder<T>() where T : class, IDataSeeder
        {
            _addDbSeeder = services => { services.AddTransient<IDataSeeder, T>(); };
            return this;
        }

        private void _processEntities()
        {
            _typeStackConfigurations.AddRange(new EntityViewModelApiBuilderHelper().ProcessEntities(_specifications));
        }

        private void _processAdminDashboardSections(AdminDashboardConfig config)
        {
            foreach (var apiSpec in _specifications)
            {
                void AddAdSection(AdminDashboardSection section)
                {
                    var existingSection = config.Sections.FirstOrDefault(s => s.Name == section.Name);
                    if (existingSection != null)
                    {
                        existingSection.Links.AddRange(section.Links);
                    }
                    else
                    {
                        config.Sections.Add(section);
                    }
                }

                apiSpec.RegisterAdminDashboardSections().ForEach(AddAdSection);
                var tmp1 = apiSpec.RegisterAdminDashboardSection();
                if (tmp1 != null)
                {
                    AddAdSection(tmp1);
                }
            }

            foreach (var section in config.Sections)
            {
                section.Links.RemoveAll(link =>
                    link.IsAspUrl &&
                    _typeStackConfigurations.Any(tsc => tsc.Disabled && tsc.UiControllerType == link.Controller));
            }

            config.Sections.RemoveAll(section => section.Links.Count == 0);
        }

        private void _registerAutoMapperPairs()
        {
            Mapper.Initialize(config =>
            {
                foreach (var typeConfig in _typeStackConfigurations)
                {
                    foreach (var viewModelMapPair in typeConfig.ViewModelPairTypes.Where(vmp =>
                        vmp.EntityViewModelMapType != null))
                    {
                        var instance =
                            (IEntityViewModelMap) Activator.CreateInstance(viewModelMapPair.EntityViewModelMapType);
                        instance.ConfigureEntityToViewModelMapper(config);
                        instance.ConfigureViewModelToEntityMapper(config);
                    }
                }
            });
        }

        private Action<IServiceCollection> _addDbContextAction;

        private void _registerDbContext(IServiceCollection services)
        {
            _addDbContextAction(services);
            _addDbSeeder(services);
        }


        private void _registerEntityTypes(ModelBuilder modelBuilder)
        {
            foreach (var entityTypePairConfiguration in _typeStackConfigurations.Where(config => config.IsStored))
            {
                modelBuilder.AddConfigurationType(entityTypePairConfiguration.EntityTypeConfigurationType);
            }
        }
    }
}