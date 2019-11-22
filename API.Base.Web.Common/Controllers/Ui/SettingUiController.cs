using System.Linq;
using System.Threading.Tasks;
using API.Base.Files.Models.Entities;
using API.Base.Files.Models.ViewModels;
using API.Base.Web.Base.Data;
using API.Base.Web.Common.Controllers.Ui.Nv;
using API.Base.Web.Common.Data;
using API.Base.Web.Common.Models;
using API.Base.Web.Common.Models.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Common.Controllers.Ui
{
    [Authorize(Roles = "Admin")]
    public class SettingUiController : NvGenericUiController<SettingEntity>
    {
        private readonly SettingsRepository _settingsRepository;

        public SettingUiController(ISettingsRepository settingsRepository)
        {
            _settingsRepository = (SettingsRepository) settingsRepository;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.RebuildQueryable(dbSet => dbSet.Include(e => e.File));
        }

        public override async Task<IActionResult> Index()
        {
            var all = await _settingsRepository.GetAll();
            all = all.OrderBy(entity => entity.Category).ThenBy(entity => entity.Slug);
            return View(all);
        }

        protected override void SetListColumns()
        {
            AddListColumns(e => e.Value);
            AddListColumns(e => e.File);
            AddListColumns(e => e.Slug);
            AddListColumns(e => e.Category);
            AddListColumns(e => e.Description);
        }

        protected override async Task<SettingEntity> GetOne(string id)
        {
            var setting = await base.GetOne(id);
            if (setting.File != null)
            {
                setting.File.Link = Mapper.Map<FileViewModel>(setting.File).Link;
            }

            return setting;
        }

        public override Task<IActionResult> Create(SettingEntity entity)
        {
            if (entity?.Type == SettingType.File)
            {
                if (!string.IsNullOrEmpty(entity?.Value))
                {
                    entity.File = new FileEntity {Id = entity.Value};
                    entity.Value = null;
                }
            }

            return base.Create(entity);
        }

        public override Task<IActionResult> Edit(string id, SettingEntity entity)
        {
            if (entity.Type == SettingType.File)
            {
                if (!string.IsNullOrEmpty(entity.Value))
                {
                    entity.File = new FileEntity {Id = entity.Value};
                    entity.Value = null;
                }
            }

            return base.Edit(id, entity);
        }
    }
}