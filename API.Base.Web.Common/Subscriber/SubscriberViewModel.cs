using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Models.ViewModels;

namespace API.Base.Web.Common.Subscriber
{
    public class SubscriberViewModel : ViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}