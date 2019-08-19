using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Attributes.GenericForm;

namespace API.Base.Web.Base.Auth.Models.HttpTransport
{
    public class BasePasswordReset: HttpTransportBaseType
    {
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [FieldHint("new-password-hint")]
        [FieldPlaceholder("new-password-placeholder")]
        public string NewPassword { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [FieldHint("repeat-new-password-hint")]
        [FieldPlaceholder("repeat-new-password-placeholder")]
        public string RepeatNewPassword { get; set; }
    }
}