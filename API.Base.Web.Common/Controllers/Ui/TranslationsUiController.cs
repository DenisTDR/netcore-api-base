using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Common.Controllers.Ui.Nv;
using API.Base.Web.Common.Data;
using API.Base.Web.Common.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Web.Common.Controllers.Ui
{
    [Authorize(Roles = "Moderator")]
    public class TranslationsUiController : NvGenericUiController<TranslationEntity>
    {
        protected override IRepository<TranslationEntity> Repo => ServiceProvider.GetService<ITranslationsRepository>();

        public override async Task<IEnumerable<TranslationEntity>> GetAllEntities()
        {
            var all = await Repo.GetAll(true);
            all = all.OrderBy(entity => entity.Slug).ThenBy(entity => entity.Language);
            return all;
        }

        protected override void SetListColumns()
        {
            base.SetListColumns();
            AddListColumn(x => x.Updated);
        }
    }
}