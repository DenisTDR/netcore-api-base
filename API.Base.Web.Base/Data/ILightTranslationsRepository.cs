using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Base.Web.Base.Data
{
    public interface ILightTranslationsRepository
    {
        Task<IEnumerable<string>> GetAvailableLanguages();
        Task<string> Translate(string translationSlug, params object[] args);
    }
}