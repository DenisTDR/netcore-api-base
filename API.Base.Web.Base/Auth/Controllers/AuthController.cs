using API.Base.Web.Base.Auth.Jwt;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Helpers;
using Microsoft.AspNetCore.Identity;

namespace API.Base.Web.Base.Auth.Controllers
{
    public class AuthController : AuthBasicController
    {
        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, 
            IJwtFactory jwtFactory, IEmailHelper emailHelper)
            : base(userManager, signInManager, jwtFactory, emailHelper)
        {
        }
    }
}