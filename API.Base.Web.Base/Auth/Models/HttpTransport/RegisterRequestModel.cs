using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Attributes.GenericForm;

namespace API.Base.Web.Base.Auth.Models.HttpTransport
{
    public class RegisterRequestModel : HttpTransportBaseType
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(64)]
        [FieldHint("register-email-hint")]
        [FieldAutocomplete("username email")]
        [FieldPlaceholder("register-email-placeholder")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [FieldHint("register-password-hint")]
        [FieldPlaceholder("register-password-placeholder")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [FieldHint("register-confirm-password-hint")]
        [FieldPlaceholder("register-confirm-password-placeholder")]
        public string ConfirmPassword { get; set; }

        [Required]
        [MinLength(3)]
        [FieldHint("first-name-hint")]
        [FieldAutocomplete("given-name")]
        [FieldPlaceholder("first-name-placeholder")]
        public virtual string FirstName { get; set; }

        [Required]
        [MinLength(3)]
        [FieldHint("last-name-hint")]
        [FieldAutocomplete("family-name")]
        [FieldPlaceholder("last-name-placeholder")]
        public virtual string LastName { get; set; }
    }
}