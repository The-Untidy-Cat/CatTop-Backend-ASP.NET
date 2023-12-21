using asp.net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace asp.net.Services
{
    public class AuthSetting
    {
        public JwtSetting? Jwt { get; internal set; }
        public CookieSetting? Cookie { get; internal set; }
    }

    public class JwtSetting
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public double ExpireDays { get; set; }
        public string? Subject { get; set; }
    }

    public class CookieSetting
    {
        public string? Name { get; set; }
        public string? Path { get; set; }
        public string? Domain { get; set; }
        public bool HttpOnly { get; set; }
        public bool Secure { get; set; }
        public int MaxAge { get; set; }
        public SameSiteMode SameSite { get; set; }
    }
}
