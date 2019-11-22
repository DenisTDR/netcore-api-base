using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Files.Models.ViewModels;
using API.Base.Web.Base.Data;
using API.Base.Web.Base.Database;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Base.Misc.PatchBag;
using API.Base.Web.Base.Models.ViewModels;
using API.Base.Web.Common.Models;
using API.Base.Web.Common.Models.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace API.Base.Web.Common.Data
{
    public class SettingsRepository : GenericRepository<SettingEntity>, ISettingsRepository
    {
        private static IEnumerable<SettingEntity> _entitiesCache = null;
        private static readonly object CacheLock = new object();
        private IEnumerable<SettingViewModel> _cache = null;

        private IEnumerable<string> _categories;

        public SettingsRepository(IDataLayer dataLayer, IOptions<SettingsCategories> categories,
            BaseDbContext dbContext) : base(dbContext.Set<SettingEntity>(), dataLayer)
        {
            dataLayer.SetRepo(this);
            _categories = categories.Value.List;
        }

        public async Task<object> Get(string slug, string category = null)
        {
            return (await GetOne(slug, category))?.Value;
        }

        public async Task<SettingViewModel> GetOne(string slug, string category = null)
        {
            var query = (await GetSettings());
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(s => s.Category == category);
            }

            return query.FirstOrDefault(setting => setting.Slug == slug);
        }

        public async Task<bool> GetBool(string slug, string category = null, bool assume = false)
        {
            return (bool) (await Get(slug, category) ?? assume);
        }

        private void InvalidateCache()
        {
            lock (CacheLock)
            {
                _entitiesCache = null;
            }
        }


        public override Task<SettingEntity> Patch(EntityPatchBag<SettingEntity> eub)
        {
            InvalidateCache();
            return base.Patch(eub);
        }

        public override Task<SettingEntity> Add(SettingEntity e)
        {
            InvalidateCache();
            return base.Add(e);
        }

        public override Task<SettingEntity> Update(SettingEntity e)
        {
            InvalidateCache();
            return base.Update(e);
        }

        public async Task<IEnumerable<SettingViewModel>> GetSettings()
        {
            if (_cache != null) return _cache;

            var settingE = await GetAll();
            _cache = settingE.Select(MapToViewModel).ToList();

            return _cache;
        }

        public IEnumerable<string> GetCategories()
        {
            return _categories;
        }

#pragma warning disable 1998
        public override async Task<IEnumerable<SettingEntity>> GetAll(bool dontFetch = false)
#pragma warning restore 1998
        {
            lock (CacheLock)
            {
                if (_entitiesCache == null)
                {
                    _entitiesCache = Queryable.Include(setting => setting.File).ToListAsync().Result;
                }

                return _entitiesCache;
            }
        }

        private SettingViewModel MapToViewModel(SettingEntity settingEntity)
        {
            var viewModel = new SettingViewModel {Slug = settingEntity.Slug, Category = settingEntity.Category};

            switch (settingEntity.Type)
            {
                case SettingType.String:
                    viewModel.Value = settingEntity.Value;
                    break;
                case SettingType.Number:
                    if (int.TryParse(settingEntity.Value, out var intValue))
                    {
                        viewModel.Value = intValue;
                    }
                    else if (double.TryParse(settingEntity.Value, out var doubleValue))
                    {
                        viewModel.Value = doubleValue;
                    }
                    else
                    {
                        viewModel.Value = 0;
                    }

                    break;
                case SettingType.Boolean:
                    if (bool.TryParse(settingEntity.Value, out var boolValue))
                    {
                        viewModel.Value = boolValue;
                    }
                    else
                    {
                        viewModel.Value = false;
                    }

                    break;
                case SettingType.File:
                    if (settingEntity.File == null)
                    {
                        viewModel.Value = null;
                    }
                    else
                    {
                        viewModel.Value = Mapper.Map<FileViewModel>(settingEntity.File).Link;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return viewModel;
        }
    }
}