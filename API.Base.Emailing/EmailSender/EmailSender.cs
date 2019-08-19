using System;
using System.Threading.Tasks;
using API.Base.Emailing.Models;
using API.Base.Logging.Logger;
using API.Base.Web.Base.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace API.Base.Emailing.EmailSender
{
    public class EmailSender : IEmailSender
    {
        protected ILLogger Logger;
        protected ISettingsRepository SettingsRepository;

        public EmailSender(IOptions<SendGridCredentials> optionsAccessor,
            ILLogger logger, ISettingsRepository settingsRepository)
        {
            Credentials = optionsAccessor.Value;
            Logger = logger;
            SettingsRepository = settingsRepository;
        }

        public SendGridCredentials Credentials { get; }

        public Task SendEmailAsync(string address, string subject, string message)
        {
            if (!Credentials.Simulate)
            {
                return Execute(Credentials.Key, subject, message, address);
            }
            return SimulateEmail(subject, message, address);
        }

        private async Task Execute(string apiKey, string subject, string message, string address)
        {
            Logger.LogInfo("Sending email '" + subject + "'");
            var client = new SendGridClient(apiKey);
            var settings = await GetSettings();
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(settings.Email, settings.Name),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(address));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
        }

        private async Task SimulateEmail(string subject, string message, string address)
        {
            await Task.Run(() =>
            {
                Console.WriteLine(subject + " to " + address + "\n");
                Console.WriteLine(message);

                Logger.LogInfo("Simulate email sending'" + subject);
            });
        }

        private async Task<EmailSettings> GetSettings()
        {
            var nameSetting = await SettingsRepository.GetOne("email_sender_name");
            var emailSetting = await SettingsRepository.GetOne("email_sender_address");
            return new EmailSettings
            {
                Name = nameSetting.Value.ToString(),
                Email = emailSetting.Value.ToString()
            };
        }
        private class EmailSettings
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }
    }
}