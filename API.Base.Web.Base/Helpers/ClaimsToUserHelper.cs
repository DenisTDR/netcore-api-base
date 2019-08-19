using System.Security.Claims;
using API.Base.Web.Base.Auth.Jwt;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace API.Base.Web.Base.Helpers
{
    public class ClaimsToUserHelper
    {
        private readonly ClaimsPrincipal _claims;
        private readonly UserManager<User> _userManager;

        public ClaimsToUserHelper(ClaimsPrincipal claims, UserManager<User> userManager)
        {
            _claims = claims;
            _userManager = userManager;
        }

        public User GetUserById()
        {
            var userId = _claims.FindFirstValue(Claims.Id);
            return _userManager.FindByIdAsync(userId).Result;
        }

        public User GetCurrentUser(bool shouldThrow = true)
        {
            var user = GetUserById();

            if (user == null && shouldThrow)
            {
                throw new KnownException("User does not exist.", 401);
            }

            return user;
        }
    }
}