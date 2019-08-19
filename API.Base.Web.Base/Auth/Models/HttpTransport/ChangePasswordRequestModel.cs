using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Attributes.GenericForm;

namespace API.Base.Web.Base.Auth.Models.HttpTransport
{
    public class ChangePasswordRequestModel: BasePasswordReset
    {
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [FieldHint("current-password-hint")]
        [FieldPlaceholder("current-password-placeholder")]
        public string CurrentPassword { get; set; }
    }
}