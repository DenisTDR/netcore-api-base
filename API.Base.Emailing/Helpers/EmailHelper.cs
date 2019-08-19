using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Base.Logging.Logger;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Data;
using API.Base.Web.Base.Exceptions;
using API.Base.Web.Base.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace API.Base.Emailing.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        protected ILLogger Logger;
        protected IEmailSender EmailSender;
        protected ILightTemplateRepository TemplateRepository;

        public EmailHelper(ILLogger logger,
            IEmailSender emailSender,
            ILightTemplateRepository templateRepository)
        {
            Logger = logger;
            EmailSender = emailSender;
            TemplateRepository = templateRepository;
        }

        public async Task SendAdminConfirmationEmail(string emailAddress, string url)
        {
            var data = new Dictionary<string, object> {["url"] = url};
            var mailTemplate = await TemplateRepository.BuildEmail("admin-confirmation-email", data);
            Logger.LogInfo("Sending admin confirmation password email");
            await EmailSender.SendEmailAsync(emailAddress, mailTemplate.Subject, mailTemplate.Body);
        }

        private static string BuildBaseUrl(string path)
        {
            if (path == null || !Regex.IsMatch(path, @"^\/([a-zA-Z-]*\/)*$"))
            {
                throw new KnownException("invalid path prefix header (must have leading and trailing slash)", 400);
            }

            return $"{EnvVarManager.GetOrThrow("EXTERNAL_URL")}{path}";
        }

        public async Task SendConfirmationEmail(User user, string token, string pathPrefix)
        {
            var url = $"{BuildBaseUrl(pathPrefix)}confirm-email?userId={user.Id}&token={token}";

            var data = new Dictionary<string, object> {["url"] = url};
            var mailTemplate = await TemplateRepository.BuildEmail("confirmation-email", data);

            Logger.LogInfo("Sending confirmation email");
            await EmailSender.SendEmailAsync(user.Email, mailTemplate.Subject, mailTemplate.Body);
        }

        public async Task SendLoginTokenEmail(User user, string token, string pathPrefix)
        {
            var url = $"{BuildBaseUrl(pathPrefix)}token-login?userEmail={user.Email}&token={token}";

            var data = new Dictionary<string, object> {{"url", url}, {"token", token}};
            var mailTemplate = await TemplateRepository.BuildEmail("login-token-email", data);

            Logger.LogInfo("Sending login token email");
            await EmailSender.SendEmailAsync(user.Email, mailTemplate.Subject, mailTemplate.Body);
        }

        public async Task SendResetPasswordEmail(User user, string token, string pathPrefix)
        {
            var url = $"{BuildBaseUrl(pathPrefix)}reset-password?userId={user.Id}&token={token}";

            var data = new Dictionary<string, object> {["url"] = url};
            var mailTemplate = await TemplateRepository.BuildEmail("reset-password-email", data);

            Logger.LogInfo("Sending reset password email");
            await EmailSender.SendEmailAsync(user.Email, mailTemplate.Subject, mailTemplate.Body);
        }

        public async Task SendEmail(string recipient, string templateName, Dictionary<string, object> dataBag)
        {
            var mailTemplate = await TemplateRepository.BuildEmail(templateName, dataBag);
            Logger.LogInfo("Sending email: " + templateName);
            await EmailSender.SendEmailAsync(recipient, mailTemplate.Subject, mailTemplate.Body);
        }
    }
}