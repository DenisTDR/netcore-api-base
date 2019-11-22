using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Base.Web.Base.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Base.Web.Base.Auth.Models.Entities
{
    public class User : IdentityUser, IEntity
    {
        [Display(Name = "First Name")] public string FirstName { get; set; }


        [Display(Name = "Last Name")] public string LastName { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
                {
                    return Email;
                }
                return $"{FirstName} {LastName}".Trim();
            }
        }

        public static string ParseShortGuid(string shortGuid)
        {
            string base64 = shortGuid.Replace("_", "/").Replace("-", "+") + "==";
            return new Guid(Convert.FromBase64String(base64)).ToString();
        }

        public static string MakeShortGuid(string guid)
        {
            return Convert.ToBase64String(new Guid(guid).ToByteArray())
                .Replace("==", "")
                .Replace("/", "_")
                .Replace("+", "-");
        }

        [NotMapped]
        public string Code => Id != null ? MakeShortGuid(Id) : null;

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        
        public string LoginToken { get; set; }

        [DataType(DataType.EmailAddress)] public override string Email { get; set; }


        [Display(Name = "Email Confirmed")] public override bool EmailConfirmed { get; set; }


        public override string ToString()
        {
            return FullName;
        }
    }
}