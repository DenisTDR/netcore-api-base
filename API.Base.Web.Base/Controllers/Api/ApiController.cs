using System.Security.Claims;
using API.Base.Web.Base.Auth.Jwt;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Exceptions;
using API.Base.Web.Base.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Web.Base.Controllers.Api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class ApiController : DiController
    {
      
    }
}