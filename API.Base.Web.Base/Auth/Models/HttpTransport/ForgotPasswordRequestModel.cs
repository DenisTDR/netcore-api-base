using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Attributes.GenericForm;

namespace API.Base.Web.Base.Auth.Models.HttpTransport
{
    public class ForgotPasswordRequestModel: HttpTransportBaseType
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(128)]
        [FieldHint("forgot-password-hint")]
        [FieldPlaceholder("forgot-password-placeholder")]
        public string Email { get; set; }
    }
}