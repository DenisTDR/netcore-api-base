using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.Base.Web.Base.Controllers.Api;
using API.Base.Web.Base.Data;
using API.Base.Web.Base.Extensions;
using API.Base.Web.Base.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace API.Base.Web.Common.Controllers.Api
{
    public class SettingController : ApiController
    {
        private readonly ISettingsRepository _settingsRepository;

        public SettingController(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        private object MakeSettingsObject(IReadOnlyCollection<SettingViewModel> settingsList)
        {
            var settings = new Dictionary<string, object>();

            foreach (var cat in _settingsRepository.GetCategories())
            {
                Dictionary<string, object> catDict;
                if (cat == "global")
                {
                    catDict = settings;
                }
                else
                {
                    catDict = new Dictionary<string, object>();
                    settings[cat.ToCamelCase()] = catDict;
                }

                foreach (var settingViewModel in settingsList.Where(setting => setting.Category == cat))
                {
                    if (settingViewModel.Value != null)
                    {
                        catDict[settingViewModel.Slug.ToCamelCase()] = settingViewModel.Value;
                    }
                }
            }
            
            var json = JsonConvert.SerializeObject(settings, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var obj = JsonConvert.DeserializeObject(json);

            return obj;
        }
        
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, object>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetBySlugs(List<string> slugs)
        {
            var settingsList = new List<SettingViewModel>();
            var allSettings = (await _settingsRepository.GetSettings()).ToList();
            
            foreach (var setting in allSettings)
            {
                if (slugs.Contains(setting.Slug.ToCamelCase()))
                {
                    settingsList.Add(setting);
                }
            }

            var obj = MakeSettingsObject(settingsList);
            
            return Ok(obj);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, object>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var settingsList = (await _settingsRepository.GetSettings()).ToList();
            var obj = MakeSettingsObject(settingsList);

            return Ok(obj);
        }


        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(object), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetOne(string slug)
        {
            var setting = await _settingsRepository.GetOne(slug);

            return Ok(setting.Value);
        }
    }
}