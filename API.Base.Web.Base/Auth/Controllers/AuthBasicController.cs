using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Base.Web.Base.Auth.Jwt;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Auth.Models.HttpTransport;
using API.Base.Web.Base.Controllers.Api;
using API.Base.Web.Base.Exceptions;
using API.Base.Web.Base.Helpers;
using API.Base.Web.Base.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable 1998

namespace API.Base.Web.Base.Auth.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public abstract class AuthBasicController<TLogin> : ApiController where TLogin : LoginRequestModel
    {
        protected IEmailHelper EmailHelper;
        protected SignInManager<User> SignInManager;
        protected IJwtFactory _jwtFactory;

        public AuthBasicController(UserManager<User> userManager, SignInManager<User> signInManager,
            IJwtFactory jwtFactory, IEmailHelper emailHelper)
        {
            EmailHelper = emailHelper;
            SignInManager = signInManager;
            _jwtFactory = jwtFactory;
        }


        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<IActionResult> Login([FromBody] TLogin model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new[]
                    {new ErrorResponseModel {Code = "InvalidCredentials", Description = "Invalid credentials."}});
            }

            var result = await SignInManager.CheckPasswordSignInAsync(user, model.Password, false);


            if (!result.Succeeded)
            {
                return BadRequest(new[]
                    {new ErrorResponseModel {Code = "InvalidCredentials", Description = "Invalid credentials."}});
            }

            var roles = await UserManager.GetRolesAsync(user);

            if (roles.Contains("Admin") || roles.Contains("Staff"))
            {
                return BadRequest(new[]
                {
                    new ErrorResponseModel
                    {
                        Code = "InvalidCredentials",
                        Description = "You can't use this account here. You can use it only in admin panel."
                    }
                });
            }

            return Ok(_jwtFactory.GenerateSession(user.UserName, roles, user.Id));
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest();
            }

            if (!model.NewPassword.Equals(model.RepeatNewPassword))
            {
                return BadRequest(new[]
                {
                    new ErrorResponseModel
                        {Code = "InvalidCredentials", Description = "The new introduced passwords don't match."}
                });
            }


            var tokenIsValid = await UserManager.VerifyUserTokenAsync(user,
                UserManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", model.Token);

            if (!tokenIsValid)
            {
                return BadRequest(new[]
                {
                    new ErrorResponseModel
                        {Code = "OperationFailed", Description = "The reset token is invalid or expired. Try again."}
                });
            }

            if (!await UserManager.IsEmailConfirmedAsync(user))
            {
                user.EmailConfirmed = true;
                await UserManager.UpdateAsync(user);
            }

            var result = await UserManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new[]
                {
                    new ErrorResponseModel {Code = "InvalidCredentials", Description = "Could not reset your password."}
                });
            }

            var roles = await UserManager.GetRolesAsync(user);

            return Ok(new
                {
                    message = "Password has been changed successfully.",
                    session = _jwtFactory.GenerateSession(user.UserName, roles, user.Id)
                }
            );
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordRequestModel model,
            [FromHeader(Name = "X-Auth-Path-Prefix")] [Required]
            string pathPrefix)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest(new[]
                {
                    new ErrorResponseModel
                        {Code = "OperationFailed", Description = "This email is not associated with an account."}
                });
            }

            try
            {
                var resetToken = await UserManager.GeneratePasswordResetTokenAsync(user);

                resetToken = HttpUtility.UrlEncode(resetToken);

                await EmailHelper.SendResetPasswordEmail(user, resetToken, pathPrefix);

                return Ok("Email has been sent successfully. Please check your email.");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                throw new KnownException("We couldn't send the forgot password email. Please contact us.", 500, exc);
            }
        }

        [HttpPost]
        [Authorize]
        public virtual async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var validCurrentPsw = await UserManager.CheckPasswordAsync(CurrentUser, model.CurrentPassword);

            if (!validCurrentPsw)
            {
                return BadRequest(new[]
                    {new ErrorResponseModel {Code = "InvalidCredentials", Description = "Invalid current password."}});
            }

            if (!model.NewPassword.Equals(model.RepeatNewPassword))
            {
                return BadRequest(new[]
                {
                    new ErrorResponseModel
                        {Code = "InvalidCredentials", Description = "The new introduced passwords don't match."}
                });
            }

            var result = await UserManager.ChangePasswordAsync(CurrentUser, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new[]
                    {new ErrorResponseModel {Code = "OperationFailed", Description = $"{result.ToString()}"}});
            }

            return Ok("Password has been changed successfully.");
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<IActionResult> Register(
            [FromBody] RegisterRequestModel model,
            [FromHeader(Name = "X-Auth-Path-Prefix")] [Required]
            string pathPrefix)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                return BadRequest(new[]
                {
                    new ErrorResponseModel
                        {Code = "InvalidCredentials", Description = "The new introduced passwords don't match."}
                });
            }

            if (IsLigaAcEmail(model.Email))
            {
                return BadRequest(new[]
                {
                    new ErrorResponseModel {Code = "LigaAcEmail", Description = "Don't use @ligaac.ro email address!"}
                });
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Created = DateTime.Now,
                Updated = DateTime.Now
            };


            var createResult = await UserManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors);
            }

            var resultUserRole = await UserManager.AddToRoleAsync(user, "User");
            var resultApiRole = await UserManager.AddToRoleAsync(user, "Api");
            if (!resultUserRole.Succeeded || !resultApiRole.Succeeded)
            {
                await UserManager.DeleteAsync(user);
                return BadRequest(!resultUserRole.Succeeded ? resultUserRole.Errors : resultApiRole.Errors);
            }

            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            token = HttpUtility.UrlEncode(token);
            try
            {
                await EmailHelper.SendConfirmationEmail(user, token, pathPrefix);

                return Ok("Confirmation email sent. Please check your email.");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                user.EmailConfirmed = true;
                await UserManager.UpdateAsync(user);
                throw new KnownException("Successfully registered. You can login now.", 500, exc);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest();
            }

            var token = model.Token;
            var isTokenValid = await UserManager.VerifyUserTokenAsync(user,
                UserManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", token);
            if (!isTokenValid)
            {
                return BadRequest(new[]
                {
                    new ErrorResponseModel
                        {Code = "OperationFailed", Description = "The confirmation token is invalid or expired."}
                });
            }

            if (await UserManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(new[]
                {
                    new ErrorResponseModel
                        {Code = "OperationFailed", Description = "Your email address is already confirmed."}
                });
            }

            var result = await UserManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(new[]
                {
                    new ErrorResponseModel
                        {Code = "OperationFailed", Description = "Could not confirm your email address. "}
                });
            }

            var roles = await UserManager.GetRolesAsync(user);

            return Ok(new
                {
                    message = "Your email is now confirmed.",
                    session = _jwtFactory.GenerateSession(user.UserName, roles, user.Id)
                }
            );
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(UserViewModel), 200)]
        public virtual async Task<IActionResult> GetOwn()
        {
            var vm = new UserViewModel
            {
                FirstName = CurrentUser.FirstName,
                LastName = CurrentUser.LastName,
                Email = CurrentUser.UserName,
                Id = CurrentUser.Id,
                Code = CurrentUser.Code,
            };
            return Ok(vm);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(UserViewModel), 200)]
        public virtual async Task<IActionResult> UpdateOwn([FromBody] UserViewModel vm)
        {
            CurrentUser.FirstName = vm.FirstName;
            CurrentUser.LastName = vm.LastName;

            await UserManager.UpdateAsync(CurrentUser);
            return Ok(vm);
        }

        [HttpGet]
        [Authorize()]
        public virtual IActionResult IsAuthorized()
        {
            var claims = User.Claims.Select(c =>
                new
                {
                    Type = c.Type,
                    Value = c.Value
                });
            return Ok(claims);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public virtual IActionResult IsAuthorizedUser()
        {
            return this.IsAuthorized();
        }

        protected bool IsLigaAcEmail(string email)
        {
            return email.EndsWith("ligaac.ro");
        }
    }
}