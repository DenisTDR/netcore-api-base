using System;
using System.Text;
using API.Base.Web.Base.ApiBuilder;
using API.Base.Web.Base.Auth.Jwt;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Database;
using API.Base.Web.Base.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Base.Web.Base
{
    public class AuthApiSpecifications : ApiSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            ConfigureIdentity(services);
            ConfigureAuthorizations(services);
            ConfigureJwtServices(services);
        }


        public override void ConfigureApp(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
        }

        public override IMvcBuilder MvcChain(IMvcBuilder source)
        {
            var final = source.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            return final;
        }

        private void ConfigureIdentity(IServiceCollection services)
        {
            services.AddDefaultIdentity<User>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;

                    options.SignIn.RequireConfirmedEmail = true;

                    options.ClaimsIdentity.UserIdClaimType = Claims.Id;
                })
                .AddRoles<Role>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<BaseDbContext>();
        }

        private void ConfigureAuthorizations(IServiceCollection services)
        {
//            services.AddAuthorization(options =>
//            {
//                foreach (Role value in Enum.GetValues(typeof(Role)))
//                {
//                    options.AddPolicy("MinimumRole=" + value.ToString(), builder =>
//                    {
//                        builder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
////                        builder.AddRequirements(new MinimumRoleRequirement(value));
//                    });
//                }
//            });

//            services.AddSingleton<IAuthorizationHandler, MinimumRoleHandler>();
        }

        private void ConfigureJwtServices(IServiceCollection services)
        {
//            Console.WriteLine("AuthApiSpecifications.ConfigureJwtServices");
            services.AddSingleton<IJwtFactory, JwtFactory>();


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EnvVarManager.GetOrThrow("JWT_SECURITY_KEY")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var audience = "http://localhost:5020";
            var issuer = "http://localhost:5020";

            services.Configure<JwtOptions>(options =>
            {
                options.Audience = audience;
                options.Issuer = issuer;
                options.SignInCredentials = creds;
            });

//            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = false;
                    options.Audience = audience;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidIssuer = issuer,
                        ValidateAudience = false,
                        ValidAudience = audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        RequireExpirationTime = false,
                        ValidateLifetime = false,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                });
//                .AddCookie(options => options.SlidingExpiration = true);
        }
    }
}