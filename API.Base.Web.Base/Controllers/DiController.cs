using System;
using System.Threading.Tasks;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Web.Base.Controllers
{
    public abstract class DiController : Controller
    {
        private IServiceProvider _serviceProvider;


        protected IServiceProvider ServiceProvider => _serviceProvider ??
                                                      throw new Exception(
                                                          "ApiController: Service provider can be used after OnActionExecuting hook or SetServiceProvider call.");

        [ApiExplorerSettings(IgnoreApi = true)]
        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _serviceProvider = HttpContext.RequestServices;
        }


        protected UserManager<User> UserManager => ServiceProvider.GetService<UserManager<User>>();

        private User _currentUser;
        private ClaimsToUserHelper _claimsUserHelper;

        protected User CurrentUser =>
            _currentUser ?? (_currentUser = ClaimsUserHelper.GetCurrentUser());

        public User CurrentUserIfLoggedIn =>
            _currentUser ?? (_currentUser = ClaimsUserHelper.GetCurrentUser(false));

        protected ClaimsToUserHelper ClaimsUserHelper =>
            _claimsUserHelper ?? (_claimsUserHelper = new ClaimsToUserHelper(User, UserManager));
    }
}