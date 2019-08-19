using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Logging.Logger;
using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Data;
using API.Base.Web.Base.Database;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Base.Exceptions;
using API.Base.Web.Common.Models.Entities;
using API.Base.Web.Common.Models.ViewModels;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace API.Base.Web.Common.Data
{
    public class TranslationsRepository : GenericRepository<TranslationEntity>, ITranslationsRepository
    {
        private readonly ISettingsRepository _settingsRepository;
        private ILLogger _logger;

        public TranslationsRepository(IDataLayer dataLayer, ISettingsRepository settingsRepository,
            BaseDbContext dbContext, ILLogger logger) : base(dbContext.Set<TranslationEntity>(), dataLayer)
        {
            dataLayer.SetRepo(this);
            _settingsRepository = settingsRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TranslationViewModel>> GetAllForSetLanguage()
        {
            var language = (string) await _settingsRepository.Get("language");
            return await GetAllForLanguage(language);
        }

        public async Task<IEnumerable<TranslationViewModel>> GetAllForLanguage(string lang)
        {
            lang = await CheckAndFixLanguage(lang);

            var transE = await FindAll(te => te.Language == lang);

            var transVm = Mapper.Map<IEnumerable<TranslationViewModel>>(transE);

            return transVm;
        }

        public async Task<IEnumerable<string>> GetAvailableLanguages()
        {
            var availableLanguages = (string) await _settingsRepository.Get("languages");
            if (string.IsNullOrEmpty(availableLanguages))
            {
                return new string[] { };
            }

            return availableLanguages.Split(new[] {",", ";"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(lang => lang.Trim().ToLower());
        }

        private IEnumerable<TranslationViewModel> _setLanguageTranslations;

        public async Task<string> Translate(string translationSlug, params object[] args)
        {
            if (_setLanguageTranslations == null)
            {
                _setLanguageTranslations = await GetAllForSetLanguage();
            }

            var trans = _setLanguageTranslations.FirstOrDefault(t => t.Slug == translationSlug);

            if (trans?.Value == null)
            {
                var msg = "No translation for '" + translationSlug + "'.";
                _logger.Log(new AuditEntity {Level = LogLevel.Critical, Data = msg});
                _logger.LogError(msg);
                return null;
            }

            var transStr = string.Format(trans.Value, args);
            return transStr;
        }

        private async Task<string> CheckAndFixLanguage(string lang)
        {
            if (string.IsNullOrEmpty(lang))
            {
                throw new KnownException("Empty language given.");
            }

            lang = lang.Trim().ToLower();
            if (!(await GetAvailableLanguages()).Contains(lang))
            {
                throw new KnownException("Invalid language");
            }

            return lang;
        }
    }
}