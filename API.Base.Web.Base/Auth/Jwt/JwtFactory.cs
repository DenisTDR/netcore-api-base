using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using API.Base.Web.Base.Auth.Models.HttpTransport;
using Microsoft.Extensions.Options;

namespace API.Base.Web.Base.Auth.Jwt
{
    internal class JwtFactory : IJwtFactory
    {
        private readonly JwtOptions _jwtOptions;

        public JwtFactory(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateToken(string username, IEnumerable<string> roles, string id)
        {
            var claims = new List<Claim>
            {
                new Claim(Claims.Username, username),
                new Claim(Claims.Id, id),
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SignInCredentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Session GenerateSession(string username, IEnumerable<string> roles, string id)
        {
            var rolesList = roles.ToList();
            return new Session
            {
                TokenType = "Bearer",
                Token = this.GenerateToken(username, rolesList, id),
                Username = username,
                Roles = string.Join(",", rolesList),
                Expiration = DateTime.Now.AddSeconds(_jwtOptions.ValidFor.TotalSeconds)
            };
        }
    }
}