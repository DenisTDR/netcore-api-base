using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using API.Base.Logging.Logger;
using API.Base.Logging.Models;
using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Auth.Jwt;
using API.Base.Web.Base.Extensions;
using API.Base.Web.Base.Misc.PatchBag;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace API.Base.Logging.Filters
{
    internal class ActionLogFilter : IResourceFilter
    {
        private readonly ILLogger _logger;

        private static readonly ConcurrentDictionary<string, DateTime> TraceIdentifiers =
            new ConcurrentDictionary<string, DateTime>();

        public ActionLogFilter(ILLogger logger)
        {
//            Console.WriteLine("ActionLogFilter ctor");
            _logger = logger;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            try
            {
                var controllerName = GetControllerName(context);
                if (controllerName == null)
                {
                    return;
                }

                TraceIdentifiers.TryAdd(context.HttpContext.TraceIdentifier, DateTime.Now);

                var actionName = GetActionName(context);
                _logger.Log(LogLevel.Information,
                    $"[{controllerName}.{actionName}][{context.HttpContext.TraceIdentifier}] Begin");

                LogAudit(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            var controllerName = GetControllerName(context);
            if (controllerName == null)
            {
                return;
            }

            var actionName = GetActionName(context);
            var durationStr = "-";
            if (TraceIdentifiers.TryRemove(context.HttpContext.TraceIdentifier, out DateTime startDate))
            {
                var duration = DateTime.Now - startDate;

                durationStr = duration.Seconds >= 1 ? duration.Seconds + "." : "";
                durationStr += duration.Milliseconds;

                FinalizeRequestAudit(context, duration);
            }

            _logger.Log(LogLevel.Information,
                $"[{controllerName}.{actionName}][{context.HttpContext.TraceIdentifier}] End (" + durationStr + ")");
        }

        private void FinalizeRequestAudit(ResourceExecutedContext context, TimeSpan duration)
        {
            if (!ShouldAuditRequest(context))
            {
                return;
            }

            var ms = (int) duration.TotalMilliseconds;
            var result = "none";
            if (context.Result is ObjectResult objectResult)
            {
                result = JsonConvert.SerializeObject(objectResult.Value);
            }

            var epb = new EntityPatchBag<AuditEntity>
            {
                Id = context.HttpContext.TraceIdentifier,
                Model = new AuditEntity
                {
                    Result = result,
                    ResponseDuration = ms
                },
                PropertiesToUpdate =
                    new Dictionary<string, bool> {{"result", true}, {"responseDuration", true}}
            };

            _logger.UpdateAuditLog(epb);
        }

        private string GetControllerName(ActionContext context)
        {
            return GetActionMethodInfo(context)?.DeclaringType?.CSharpName();
        }

        private string GetActionName(ActionContext context)
        {
            return GetActionMethodInfo(context)?.Name;
        }

        private MethodInfo GetActionMethodInfo(ActionContext context)
        {
            if (context.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)
            {
                var ad = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor) context.ActionDescriptor;
                var methodInfo = ad?.MethodInfo;
                return methodInfo;
            }

            return null;
        }

        private void LogAudit(ResourceExecutingContext context)
        {
            if (!ShouldAuditRequest(context))
            {
                return;
            }

            var str = GetRequestBody(context.HttpContext.Request);
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            var actionDisplayName = context.ActionDescriptor.DisplayName;

            str = ClearPasswords(str);


            if (!string.IsNullOrEmpty(str))
            {
                _logger.Log(LogLevel.Information,
                    new AuditEntity
                    {
                        RequestBody = str,
                        Ip = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                        AuthInfo = GetUser(context.HttpContext.Request.Headers["Authorization"]),
//                            responseHeaders.ContainsKey("Auth")
//                                ? JsonConvert.DeserializeObject<AuthEntity>(responseHeaders["Auth"])
//                                : null,
                        Info = actionDisplayName,
                        RequestUri = context.HttpContext.Request.GetEncodedUrl(),
                        TraceIdentifier = context.HttpContext.TraceIdentifier
                    });
            }
        }

        private bool ShouldAuditRequest(ActionContext context)
        {
            var requestHeaders = context.HttpContext.Request.Headers;
            var isJsonRequest = requestHeaders.ContainsKey("Content-Type") &&
                                requestHeaders["Content-Type"][0].Contains("application/json");
            if (!isJsonRequest)
            {
                return false;
            }


            var method = context.HttpContext.Request.Method;
            if (method == "GET" || method == "OPTIONS")
            {
                return false;
            }

            return true;
        }

        private string GetRequestBody(HttpRequest request)
        {
            var inputStream = request.Body;
            var ms = new MemoryStream();
            inputStream.CopyTo(ms);
            ms.Position = 0;
            var sr = new StreamReader(ms);
            var str = sr.ReadToEnd();
            ms.Position = 0;
            request.Body = ms;

            return str;
        }

        private string ClearPasswords(string json)
        {
            try
            {
                if (json.Trim().StartsWith("{"))
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(json);
                    ClearPasswords(obj);
                    return JsonConvert.SerializeObject(obj);
                }

                if (json.Trim().StartsWith("["))
                {
                    var objs = JsonConvert.DeserializeObject<JArray>(json);
                    foreach (var obj in objs)
                    {
                        ClearPasswords(obj);
                    }

                    return JsonConvert.SerializeObject(objs);
                }

                return json;
            }
            catch (Exception exc)
            {
                _logger.Log(LogLevel.Information,
                    $"Couldn't filter sensitive data in request body for audit because:{exc.Message} \n {json}");
                return json;
            }
        }

        private void ClearPasswords(JToken jObject)
        {
            foreach (var property in jObject.Children().Where(ch => ch is JProperty).Cast<JProperty>())
            {
                if (property?.Name?.ToLower()?.Contains("password") == true ||
                    property?.Name?.ToLower() == "token"
                )
                {
                    property.Value = null;
                }
            }
        }

        private string GetUser(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                return null;
            }

            if (jwtToken.StartsWith("Bearer "))
            {
                jwtToken = jwtToken.Substring(7);
            }

            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(jwtToken) as JwtSecurityToken;

            return JsonConvert.SerializeObject(new UserBasicLogModel
            {
                Id = tokenS?.Claims.FirstOrDefault(claim => claim.Type == Claims.Id)?.Value,
                Username = tokenS?.Claims.FirstOrDefault(claim => claim.Type == Claims.Username)?.Value,
                Roles = tokenS?.Claims.FirstOrDefault(claim => claim.Type == Claims.Roles)?.Value,
            });
        }
    }
}