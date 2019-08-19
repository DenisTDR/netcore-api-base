using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Controllers.Ui;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Base.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace API.Base.Logging.Filters
{
    public class AdminLogFilter : IActionFilter
    {
        public UserManager<User> UserManager { get; }

        public IRepository<AdminLogEntity> LogsRepo;

        public AdminLogFilter(UserManager<User> userManager, IDataLayer dataLayer)
        {
            UserManager = userManager;
            LogsRepo = dataLayer.Repo<AdminLogEntity>();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                if (context.HttpContext.Request.Method == "POST")
                {
                    LogPostRequest(context);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        private void LogPostRequest(ActionExecutingContext context)
        {
            if (!(context.Controller is UiController))
            {
                return;
            }

            if (!context.ModelState.IsValid)
            {
                return;
            }

            if (!(context.ActionDescriptor is ControllerActionDescriptor))
            {
                return;
            }

            var request = context.HttpContext.Request;


            var actionDescriptor = (ControllerActionDescriptor) context.ActionDescriptor;

            var str = actionDescriptor.ActionName == "Delete" ? null : PatchFormData(request.Form);

            var targetId = context.ActionArguments.ContainsKey("id") ? context.ActionArguments["id"]?.ToString() : null;

            var controller = (UiController) context.Controller;

            var adminLog = new AdminLogEntity
            {
                Action = actionDescriptor.ActionName,
                Controller = actionDescriptor.ControllerTypeInfo.Name,
                NewVersion = str,
                Author = controller.CurrentUserIfLoggedIn,
                Url = context.HttpContext.Request.GetEncodedPathAndQuery(),
                TargetId = targetId,
                TraceIdentifier = context.HttpContext.TraceIdentifier
            };
            LogsRepo.Add(adminLog).Wait();
        }

        private string PatchFormData(IFormCollection formCollection)
        {
            var list = new Dictionary<string, object>();
            foreach (var (key, value) in formCollection)
            {
                if (key == "__RequestVerificationToken")
                {
                    continue;
                }

                list[key] = value;
            }

            return JsonConvert.SerializeObject(list);
        }
    }
}