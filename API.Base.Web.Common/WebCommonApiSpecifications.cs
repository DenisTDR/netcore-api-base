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
                        new AdminDashboardLink("Settings", typeof(SettingUiController)),
                        new AdminDashboardLink("Templates", typeof(TemplateUiController)),
                        new AdminDashboardLink("Translations", typeof(TranslationsUiController)),
                        new AdminDashboardLink("Files", typeof(FileUiController))
                    }
                },
                new AdminDashboardSection
                {
                    Name = "Content",
                    Links = new List<AdminDashboardLink>
                    {
                        new AdminDashboardLink("Partners", typeof(PartnersUiController))
                    }
                },

                new AdminDashboardSection
                {
                    Name = "Content",
                    Links = new List<AdminDashboardLink>
                    {
                        new AdminDashboardLink("Tracking", typeof(ReferenceTrackUiController)),
                        new AdminDashboardLink("Subscribers", typeof(SubscriberUiController)),
                    }
                },
                new AdminDashboardSection
                {
                    Name = "Content",
                    Links = new List<AdminDashboardLink>
                    {
                        new AdminDashboardLink("FAQs", typeof(FaqUiController)),
                        new AdminDashboardLink("OG Metadata", typeof(OgMetadataUiController)),
                    }
                },
            };
        }
    }
}