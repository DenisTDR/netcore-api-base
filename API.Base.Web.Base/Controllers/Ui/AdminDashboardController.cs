using System;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Database;
using API.Base.Web.Base.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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