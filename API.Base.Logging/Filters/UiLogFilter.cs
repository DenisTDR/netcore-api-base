using System;
using System.Collections.Generic;
using API.Base.Logging.Logger;
using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Controllers.Ui;
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
        
        private readonly ILLogger _logger;

        public AdminLogFilter(UserManager<User> userManager, ILLogger logger)
        {
            UserManager = userManager;
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                if (context.HttpContext.Request.Method == "POST")
                {
                    if (IsUiController(context))
                    {
                        LogPostRequest(context);
                    }
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
        public bool IsUiController(ActionExecutingContext context)
        {
            return context.ActionDescriptor is ControllerActionDescriptor actionDescriptor &&
                   typeof(UiController).IsAssignableFrom(actionDescriptor.ControllerTypeInfo);
        }

        private void LogPostRequest(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                return;
            }

            var request = context.HttpContext.Request;

            var actionDescriptor = (ControllerActionDescriptor) context.ActionDescriptor;

            var str = actionDescriptor.ActionName == "Delete" ? null : PatchFormData(request.Form);

            var targetId = context.ActionArguments.ContainsKey("id") ? context.ActionArguments["id"]?.ToString() : null;

            var controller = (UiController) context.Controller;

            var uiLog = new LogsUiEntity
            {
                Action = actionDescriptor.ActionName,
                Controller = actionDescriptor.ControllerTypeInfo.Name,
                NewVersion = str,
                Author = controller.CurrentUserIfLoggedIn,
                Url = context.HttpContext.Request.GetEncodedPathAndQuery(),
                TargetId = targetId,
                TraceIdentifier = context.HttpContext.TraceIdentifier
            };
            _logger.UiLog(uiLog);
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