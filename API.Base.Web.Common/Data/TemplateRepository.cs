using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Logging.Logger;
using API.Base.Web.Base.Data;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Base.Exceptions;
using API.Base.Web.Common.Models.Entities;
using Newtonsoft.Json;

namespace API.Base.Web.Common.Data
{
    public class TemplateRepository : ITemplateRepository
    {
        protected readonly IRepository<TemplateEntity> Repo;
        protected ILLogger Logger;
        private readonly ISettingsRepository _settingsRepository;

        public TemplateRepository(IDataLayer dataLayer, ILLogger logger, ISettingsRepository settingsRepository)
        {
            Repo = dataLayer.Repo<TemplateEntity>();
            Logger = logger;
            _settingsRepository = settingsRepository;
        }

        public async Task<EmailTemplate> BuildEmail(string slug, Dictionary<string, object> bag)
        {
            var templateEntity = await Repo.FindOne(t => t.Slug == slug);
            if (templateEntity == null)
            {
                var msg = "Template '" + slug + "' not found.";
                Logger.LogError(msg);
                throw new KnownException(msg);
            }

            var template = await ResolveVariables(templateEntity, bag);

            var eTemplate = new EmailTemplate()
            {
                Body = template,
                Subject = templateEntity.Title
            };

            return eTemplate;
        }

        private async Task<string> ResolveVariables(TemplateEntity source, Dictionary<string, object> bag)
        {
            var template = source.Content;

            template = ReplaceBag(template, bag);

            template = await ResolveSettingsVariables(template);
            if (template.Contains("{{"))
            {
                Logger.LogError("Template '" + source.Slug + "' not fulfilled.");
            }

            return template;
        }

        private async Task<string> ResolveSettingsVariables(string source)
        {
            var settings = await _settingsRepository.GetSettings();
            var settingsBag = new Dictionary<string, object>();
            foreach (var svm in settings.Where(setting => setting.Category == "global"))
            {
                settingsBag["settings:" + svm.Slug] = svm.Value;
            }

            return ReplaceBag(source, settingsBag);
        }

        private string ReplaceBag(string source, Dictionary<string, object> bag)
        {
            if (bag != null)
            {
                foreach (var keyValuePair in bag)
                {
                    var replaceStr = $"{{{{{keyValuePair.Key}}}}}";
                    if (source.Contains(replaceStr))
                    {
                        source = source.Replace(replaceStr, keyValuePair.Value?.ToString());
                    }
                }
            }

            return source;
        }
    }
}