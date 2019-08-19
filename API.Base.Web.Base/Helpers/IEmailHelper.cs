using System.Collections.Generic;
using System.Threading.Tasks;
using API.Base.Web.Base.Auth.Models.Entities;

namespace API.Base.Web.Base.Helpers
{
    public interface IEmailHelper
    {
        Task SendAdminConfirmationEmail(string emailAddress, string url);
        Task SendConfirmationEmail(User user, string token, string pathPrefix);
        Task SendLoginTokenEmail(User user, string token, string pathPrefix);
        Task SendResetPasswordEmail(User user, string token, string pathPrefix);
        Task SendEmail(string recipient, string templateName, Dictionary<string, object> dataBag);
    }
}