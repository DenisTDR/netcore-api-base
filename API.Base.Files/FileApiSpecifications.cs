using API.Base.Web.Base.ApiBuilder;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Files
{
    public class FileApiSpecifications : ApiSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<FileUploadManager, FileUploadManager>();
        }
    }
}