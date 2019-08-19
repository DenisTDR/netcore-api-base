using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Attributes.GenericForm;

namespace API.Base.Web.Base.Auth.Models.HttpTransport
{
    public class LoginRequestModel: HttpTransportBaseType
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(64)]
        [FieldHint("login-email-hint")]
        [FieldAutocomplete("username email")]
        [FieldPlaceholder("login-email-placeholder")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [FieldHint("login-password-hint")]
        [FieldAutocomplete("current-password")]
        [FieldPlaceholder("login-password-placeholder")]
        public string Password { get; set; }
    }
}