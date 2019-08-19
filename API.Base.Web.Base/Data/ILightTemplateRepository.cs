using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Base.Web.Base.Data
{
    public interface ILightTemplateRepository
    {
        Task<EmailTemplate> BuildEmail(string slug, Dictionary<string, object> bag);
    }
}