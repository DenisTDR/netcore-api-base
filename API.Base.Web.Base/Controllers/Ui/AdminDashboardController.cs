using System;
using System.Threading.Tasks;
using API.Base.Web.Base.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Base.Web.Base.Controllers.Ui
{
    [Authorize(Roles = "Staff")]
    public class AdminDashboardController : Controller
    {
        private IDataSeeder _dataSeeder;


        public AdminDashboardController(
            IDataSeeder dataSeeder
        )
        {
            _dataSeeder = dataSeeder;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Seed(bool show = false)
        {
            try
            {
                if (show)
                {
                    ViewBag.Data = await _dataSeeder.SeedToString("true");
                }

                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public IActionResult ClearModelMetadataCache()
        {
            ModelMetadataController.ClearCache();
            return RedirectToAction("Index");
        }
    }
}