using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Base.Web.Base.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailHelper _emailHelper;

        public StatusMessageWithType StatusMessage { get; set; }

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailHelper emailHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailHelper = emailHelper;
        }

        [BindProperty] public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = Input.Email, Email = Input.Email,
                    Created = DateTime.Now, Updated = DateTime.Now
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    result = await _userManager.AddToRoleAsync(user, "User");
                    result = await _userManager.AddToRoleAsync(user, "Staff");
                    if (await _userManager.Users.CountAsync() == 1)
                    {
                        result = await _userManager.AddToRoleAsync(user, "Admin");
                        result = await _userManager.AddToRoleAsync(user, "Moderator");
                    }

                    if (result.Succeeded)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new {userId = user.Id, code = code},
                            protocol: Request.Scheme);

                        await _emailHelper.SendAdminConfirmationEmail(Input.Email, callbackUrl);

//                    await _signInManager.SignInAsync(user, isPersistent: false);

                        StatusMessage = new StatusMessageWithType("Confirmation email sent. Please check your email.",
                            "info");
                        return Page();
//                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}