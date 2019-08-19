using System.Collections.Generic;
using System.Threading.Tasks;
using API.Base.Web.Base.Models.ViewModels;

namespace API.Base.Web.Base.Data
{
    public interface ISettingsRepository
    {
        Task<object> Get(string slug, string category = null);
        Task<SettingViewModel> GetOne(string slug, string category = null);
        Task<bool> GetBool(string slug, string category = null, bool assume = false);
        Task<IEnumerable<SettingViewModel>> GetSettings();
        IEnumerable<string> GetCategories();
    }
}