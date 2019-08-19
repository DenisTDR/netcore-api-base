using System.ComponentModel.DataAnnotations;

namespace API.Base.Web.Base.Auth.Models.HttpTransport
{
    public class ConfirmEmailRequestModel : HttpTransportBaseType
    {
        [Required] public string UserId { get; set; }
        [Required] public string Token { get; set; }
    }
}