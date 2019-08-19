using System;

namespace API.Base.Web.Base.Auth.Models.HttpTransport
{
    public class Session
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string TokenType { get; set; }
        public DateTime Expiration { get; set; }
        public string Roles { get; set; }
    }
}