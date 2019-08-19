using System;
using System.Collections.Generic;
using API.Base.Files.Controllers;
using API.Base.Web.Base.ApiBuilder;
using API.Base.Web.Base.Data;
using API.Base.Web.Base.Models;
using API.Base.Web.Common.Controllers.Ui;
using API.Base.Web.Common.Data;
using API.Base.Web.Common.FAQ;
using API.Base.Web.Common.Models;
using API.Base.Web.Common.OgMetadata;
using API.Base.Web.Common.Partners.Controllers;
using API.Base.Web.Common.ReferenceTrack;
using API.Base.Web.Common.Subscriber;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Web.Common
{
    public class WebCommonApiSpecifications : ApiSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddOptions<SettingsCategories>().Configure(sc =>
            {
                sc.Add("global");
                sc.Add("home_page");
                sc.Add("menu");
            });

            services.AddScoped<ITranslationsRepository, TranslationsRepository>();
            services.AddScoped<ILightTranslationsRepository, TranslationsRepository>();

            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<ILightTemplateRepository, TemplateRepository>();
        }

        public override List<AdminDashboardSection> RegisterAdminDashboardSections()
        {
            return new List<AdminDashboardSection>
            {
                new AdminDashboardSection
                {
                    Name = "Common",
                    Links = new List<AdminDashboardLink>
                    {
                        new AdminDashboardLink("Settings", controller: nameof(SettingUiController)),
                        new AdminDashboardLink("Templates", controller: nameof(TemplateUiController)),
                        new AdminDashboardLink("Translations", controller: nameof(TranslationsUiController)),
                        new AdminDashboardLink("Files", controller: nameof(FileUiController))
                    }
                },
                new AdminDashboardSection
                {
                    Name = "Content",
                    Links = new List<AdminDashboardLink>
                    {
                        new AdminDashboardLink("Partners", controller: nameof(PartnersUiController))
                    }
                },

                new AdminDashboardSection
                {
                    Name = "Content",
                    Links = new List<AdminDashboardLink>
                    {
                        new AdminDashboardLink("Tracking", controller: nameof(ReferenceTrackUiController)),
                        new AdminDashboardLink("Subscribers", controller: nameof(SubscriberUiController)),
                    }
                },
                new AdminDashboardSection
                {
                    Name = "Content",
                    Links = new List<AdminDashboardLink>
                    {
                        new AdminDashboardLink("FAQs", controller: nameof(FaqUiController)),
                        new AdminDashboardLink("OG Metadata", controller: nameof(OgMetadataUiController)),
                    }
                },
            };
        }
    }
}