using System.Collections.Generic;
using System.Threading.Tasks;
using API.Base.Web.Base.Data;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Common.Models.Entities;
using API.Base.Web.Common.Models.ViewModels;

namespace API.Base.Web.Common.Data
{
    public interface ITranslationsRepository : IRepository<TranslationEntity>, ILightTranslationsRepository
    {
        Task<IEnumerable<TranslationViewModel>> GetAllForLanguage(string lang);
    }
}