using System.Collections.Generic;
using API.Base.Web.Base.Auth.Models.HttpTransport;

namespace API.Base.Web.Base.Auth.Jwt
{
    public interface IJwtFactory
    {
        string GenerateToken(string username, IEnumerable<string> roles, string id);
        Session GenerateSession(string username, IEnumerable<string> roles, string id);
    }
}