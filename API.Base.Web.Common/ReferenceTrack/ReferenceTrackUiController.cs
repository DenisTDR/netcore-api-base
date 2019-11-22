using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.Controllers.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Common.ReferenceTrack
{
    [Authorize(Roles = "Staff")]
    public class ReferenceTrackUiController : GenericUiController<ReferenceTrackEntity>
    {
        public override async Task<IActionResult> Index()
        {
            var all = await Repo.DbSet.OrderBy(s => s.Created).ToListAsync();
            foreach (var rt in all)
            {
                rt.Code = GetFullName(rt.Code);
            }

            return View(all);
        }

        private static string GetFullName(string code)
        {
            if (string.IsNullOrEmpty(code)) return "";
            switch (code)
            {
                case "tc1":
                    return "Afis camine";
                case "tc2":
                    return "Afis camine";
                case "tml":
                    return "Mail participanti itec link";
                case "tmb":
                    return "Mail participanti itec buton";
                case "tsa":
                    return "Site pagina fb";
                case "tf":
                    return "Afis facultate";
                default: return code;
            }
        }
    }
}