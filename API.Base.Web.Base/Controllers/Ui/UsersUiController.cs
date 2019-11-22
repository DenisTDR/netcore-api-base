using System;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Base.Controllers.Ui
{
    [Authorize(Roles = "Moderator")]
    public class UsersUiController : DiController
    {
        private UserManager<User> _userManager;


        public UsersUiController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.OrderBy(c => c.Created).ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmChangeRole(string id, string role, bool add)
        {
            var user = await _userManager.GetUserById(id);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string id, string role, bool add)
        {
            var user = await _userManager.GetUserById(id);
            if (CurrentUser.Id.Equals(user.Id) && (role == "Staff" || role == "Admin") && !add)
            {
                return RedirectToAction(nameof(Users));
            }

            if (add)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            return RedirectToAction(nameof(Users));
        }


        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var user = await _userManager.GetUserById(id);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Update(string id, User user, string returnUrl = null)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var existingUser = await _userManager.GetUserById(id);
            var save = false;

            var updatablePropNames = new[] {"LastName", "FirstName", "EmailConfirmed"};

            foreach (var upn in updatablePropNames)
            {
                var up = typeof(User).GetProperty(upn);
                if (up.GetValue(user)?.Equals(up.GetValue(existingUser)) != true)
                {
                    Console.WriteLine("Updating: " + up);
                    up.SetValue(existingUser, up.GetValue(user));
                    save = true;
                    await _userManager.UpdateAsync(existingUser);
                }
            }

            if (user.Email != existingUser.Email)
            {
                existingUser.Email = user.Email;

                var tmpToken = await _userManager.GenerateChangeEmailTokenAsync(existingUser, user.Email);

                await _userManager.ChangeEmailAsync(existingUser, user.Email, tmpToken);
                await _userManager.UpdateNormalizedEmailAsync(existingUser);

                await _userManager.SetUserNameAsync(existingUser, user.Email);

                save = true;
            }


            if (save)
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction(nameof(Users));
                }

                return LocalRedirect(returnUrl);
            }

            return View(existingUser);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.GetUserById(id);
          
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            var user = await _userManager.GetUserById(id);
            
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Users));
        }
    }
}