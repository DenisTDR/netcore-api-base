using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Attributes;
using API.Base.Web.Base.Attributes.GenericForm;

namespace API.Base.Web.Base.Models.ViewModels
{
    public class UserViewModel : ViewModel
    {
        [FieldDefaultTexts]
        [DataType(DataType.EmailAddress)]
        [FieldBasic("disabled", true)]
        public string Email { get; set; }

        [Required]
        [MinLength(3)]
        [FieldDefaultTexts]
        [DataType(DataType.Text)]
        [FieldAutocomplete("given-name")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(3)]
        [FieldDefaultTexts]
        [DataType(DataType.Text)]
        [FieldAutocomplete("family-name")]
        public string LastName { get; set; }

        [IsReadOnly]
        public string Code { get; set; }
    }
}
