using asp.net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace asp.net.Services
{
    public class AuthSetting
    {
        [JsonProperty("Jwt")]
        public JwtSetting? Jwt { get; set; }

        [JsonProperty("Cookie")]
        public CookieSetting? Cookie { get; set; }
    }

    public class JwtSetting
    {
        [JsonProperty("Key")]
        public string? Key { get; set; }
        [JsonProperty("Issuer")]
        public string? Issuer { get; set; }
        [JsonProperty("Audience")]
        public string? Audience { get; set; }
        [JsonProperty("ExpireDays")]
        public double ExpireDays { get; set; }
        [JsonProperty("Subject")]
        public string? Subject { get; set; }
    }

    public class CookieSetting
    {
        [JsonProperty("Name")]
        public string? Name { get; set; }
        [JsonProperty("Path")]
        public string? Path { get; set; }
        [JsonProperty("Domain")]
        public string? Domain { get; set; }
        [JsonProperty("HttpOnly")]
        public bool HttpOnly { get; set; }
        [JsonProperty("Secure")]
        public bool Secure { get; set; }
        [JsonProperty("MaxAge")]
        public int MaxAge { get; set; }
        [JsonProperty("Samesite")]
        public SameSiteMode SameSite { get; set; }
    }
}
