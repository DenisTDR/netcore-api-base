using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API.Base.Logging.Logger;
using API.Base.Web.Base.Data;
using API.Base.Web.Base.Database;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Base.Extensions.ReflectionExtensions;
using API.Base.Web.Base.Models.Entities;
using API.Base.Web.Common.FAQ;
using API.Base.Web.Common.Models.Entities;
using API.Base.Web.Common.OgMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Web.Common.Database
{
    public class CommonDataSeeder<TDst> : BaseDataSeeder<TDst> where TDst : CommonDataSeedType, new()
    {
        protected ILLogger _logger;
        private ISettingsRepository _settingsRepository;

        public CommonDataSeeder(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = serviceProvider.GetService<ILLogger>();
            _settingsRepository = serviceProvider.GetService<ISettingsRepository>();
        }

        public override async Task LoadSeed()
        {
            await base.LoadSeed();
            await SeedSettings();
            await SeedTranslations();
            await SeedTemplates();
            await SeedOgMetaDatas();
            await SeedFaqs();
        }


        protected virtual async Task SeedSettings()
        {
            if (SeedingData.Settings == null)
            {
                return;
            }

            var settingsRepo = (IRepository<SettingEntity>) _settingsRepository;
            var existingSettings = (await settingsRepo.GetAll()).ToList();
            settingsRepo.SkipSaving = true;
            foreach (var settingEntity in SeedingData.Settings)
            {
                if (!existingSettings.Any(s => s.Slug == settingEntity.Slug && s.Category == settingEntity.Category))
                {
                    _logger.LogInfo("Added seed setting (" + settingEntity.Category + ":" + settingEntity.Slug + ")");
                    await settingsRepo.Add(settingEntity);
                }
            }

            await DataLayer.SaveChangesAsync();
        }

        protected virtual async Task SeedTranslations()
        {
            if (SeedingData.Translations == null)
            {
                return;
            }

            var translationsRepo = DataLayer.Repo<TranslationEntity>();
            var existingTranslations = (await translationsRepo.GetAll()).ToList();
            translationsRepo.SkipSaving = true;
            foreach (var translationEntity in SeedingData.Translations)
            {
                if (!existingTranslations.Any(s =>
                    s.Slug == translationEntity.Slug && s.Language == translationEntity.Language))
                {
                    _logger.LogInfo(
                        "Added seed translation (" + translationEntity.Language + ":" + translationEntity.Slug + ")");
                    await translationsRepo.Add(translationEntity);
                }
            }

            await DataLayer.SaveChangesAsync();
        }

        protected virtual async Task SeedTemplates()
        {
            if (SeedingData.Templates == null)
            {
                return;
            }

            var templatesRepo = DataLayer.Repo<TemplateEntity>();
            var existingTemplates = (await templatesRepo.GetAll()).ToList();
            templatesRepo.SkipSaving = true;
            foreach (var templateEntity in SeedingData.Templates)
            {
                if (existingTemplates.All(s => s.Slug != templateEntity.Slug))
                {
                    _logger.LogInfo(
                        "Added seed template (" + templateEntity.Slug + ")");
                    await templatesRepo.Add(templateEntity);
                }
            }

            await DataLayer.SaveChangesAsync();
        }


        protected virtual async Task SeedOgMetaDatas()
        {
            if (SeedingData.OgMetaDatas == null)
            {
                return;
            }

            var ogMetaDataRepo = DataLayer.Repo<OgMetadataEntity>();
            var existingOgMetaDatas = (await ogMetaDataRepo.GetAll()).ToList();
            ogMetaDataRepo.SkipSaving = true;
            foreach (var ogMetaDataEntity in SeedingData.OgMetaDatas)
            {
                if (existingOgMetaDatas.All(s => s.Slug != ogMetaDataEntity.Slug))
                {
                    _logger.LogInfo(
                        "Added seed ogmetadata (" + ogMetaDataEntity.Slug + ")");
                    await ogMetaDataRepo.Add(ogMetaDataEntity);
                }
            }

            await DataLayer.SaveChangesAsync();
        }

        protected virtual async Task SeedFaqs()
        {
            if (SeedingData.Faqs == null)
            {
                return;
            }

            var faqRepo = DataLayer.Repo<FaqEntity>();
            var existingOgMetaDatas = (await faqRepo.GetAll()).ToList();
            faqRepo.SkipSaving = true;
            foreach (var faqEntity in SeedingData.Faqs)
            {
                if (existingOgMetaDatas.All(s => s.Question != faqEntity.Question))
                {
                    _logger.LogInfo(
                        "Added seed faq (" + faqEntity.Question + ")");
                    await faqRepo.Add(faqEntity);
                }
            }

            await DataLayer.SaveChangesAsync();
        }

        protected override async Task SetSeedingData(List<string> entities = null)
        {
            await base.SetSeedingData(entities);
            await TryGenerateSeedFor(e => e.Settings, entities);
            await TryGenerateSeedFor(e => e.Translations, entities);
            await TryGenerateSeedFor(e => e.Templates, entities);
            SeedingData.OgMetaDatas = await GetEntitiesFromRepo(DataLayer.Repo<OgMetadataEntity>());
            SeedingData.Faqs = await GetEntitiesFromRepo(DataLayer.Repo<FaqEntity>());
        }

        private async Task TryGenerateSeedFor<TE>(Expression<Func<TDst, IEnumerable<TE>>> expr,
            ICollection<string> entities = null) where TE : Entity
        {
            var propInfo = SymbolExtensions.GetPropertyInfo(expr);
            var collectionName = propInfo.Name;

            if (entities != null && !entities.Contains(collectionName))
            {
                return;
            }

            try
            {
                dynamic repo = DataLayer.Repo<TE>();
                var allEntities = await GetEntitiesFromRepo(repo);

                propInfo.SetValue(SeedingData, allEntities);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task<IEnumerable<SettingEntity>> GetEntitiesFromRepo(IRepository<SettingEntity> repo)
        {
            return await repo.Queryable.OrderBy(t => t.Slug).ThenBy(t => t.Category).Select(entity => new SettingEntity
                {
                    Slug = entity.Slug, Type = entity.Type, Value = entity.Value, Category = entity.Category,
                    Description = entity.Description
                })
                .ToListAsync();
        }

        private async Task<IEnumerable<TranslationEntity>> GetEntitiesFromRepo(IRepository<TranslationEntity> repo)
        {
            return await repo.Queryable.OrderBy(t => t.Slug).Select(
                entity => new TranslationEntity
                    {Slug = entity.Slug, Value = entity.Value, Language = entity.Language}).ToListAsync();
        }

        private async Task<IEnumerable<TemplateEntity>> GetEntitiesFromRepo(IRepository<TemplateEntity> repo)
        {
            return await repo.Queryable.OrderBy(t => t.Slug).Select(
                entity => new TemplateEntity
                {
                    Slug = entity.Slug,
                    Title = entity.Title,
                    Content = entity.Content,
                    Description = entity.Description
                }).ToListAsync();
        }

        private async Task<IEnumerable<OgMetadataEntity>> GetEntitiesFromRepo(IRepository<OgMetadataEntity> repo)
        {
            return await repo.Queryable.OrderBy(t => t.Slug).Select(
                entity => new OgMetadataEntity()
                {
                    Slug = entity.Slug,
                    Title = entity.Title,
                    Description = entity.Description,
                    Type = entity.Type
                }).ToListAsync();
        }

        private async Task<IEnumerable<FaqEntity>> GetEntitiesFromRepo(IRepository<FaqEntity> repo)
        {
            return await repo.Queryable.OrderBy(t => t.Question).Select(
                entity => new FaqEntity()
                {
                    Question = entity.Question,
                    Answer = entity.Answer,
                    OrderIndex = entity.OrderIndex,
                    Category = entity.Category,
                    Published = entity.Published
                }).ToListAsync();
        }
    }

    public class CommonDataSeedType : BaseDataSeedType
    {
        public IEnumerable<SettingEntity> Settings { get; set; }
        public IEnumerable<TranslationEntity> Translations { get; set; }
        public IEnumerable<TemplateEntity> Templates { get; set; }
        public IEnumerable<OgMetadataEntity> OgMetaDatas { get; set; }
        public IEnumerable<FaqEntity> Faqs { get; set; }
    }
}