using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Base.Web.Common.Subscriber
{
    [Authorize(Roles = "Moderator")]
    public class SubscriberUiController : GenericUiController<SubscriberEntity>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.RebuildQueryable(dbSet => dbSet.Where(s => !s.Deleted).OrderBy(s => s.Created));
        }

        public override async Task<IActionResult> Index()
        {
            return View(await Repo.GetAll());
        }

        public async Task<IActionResult> ToCsv()
        {
            Response.Headers.Add("Content-Disposition", "attachment;filename=subscribers-export.csv");
            return View(await Repo.GetAll());
        }
    }
}