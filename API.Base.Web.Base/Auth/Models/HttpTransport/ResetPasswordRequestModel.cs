using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Attributes;

namespace API.Base.Web.Base.Auth.Models.HttpTransport
{
    public class ResetPasswordRequestModel : BasePasswordReset
    {
        [Required] [IsReadOnly] public string Token { get; set; }
        [Required] [IsReadOnly] public string UserId { get; set; }
    }
}