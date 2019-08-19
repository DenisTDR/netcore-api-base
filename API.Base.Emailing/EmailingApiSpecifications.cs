using API.Base.Emailing.Helpers;
using API.Base.Emailing.Models;
using API.Base.Web.Base.ApiBuilder;
using API.Base.Web.Base.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Emailing
{
    public class EmailingApiSpecifications : ApiSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddTransient<IEmailSender, EmailSender.EmailSender>();
            services.AddTransient<IEmailHelper, EmailHelper>();

            services.AddOptions<SendGridCredentials>().Configure(sgc =>
            {
                var key = EnvVarManager.Get("SENDGRID_KEY");
                if (!string.IsNullOrEmpty(key))
                {
                    sgc.Key = key;
                }
                else
                {
                    sgc.Simulate = true;
                }
            });
        }
    }
}