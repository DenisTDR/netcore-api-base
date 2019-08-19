using System.Threading.Tasks;
using API.Base.Web.Base.Auth.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Base.Web.Base.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<User> GetUserById(this UserManager<User> userManager, string id)
        {
            return await userManager.FindByIdAsync(id);
        }
    }
}