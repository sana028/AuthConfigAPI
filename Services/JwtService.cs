using AuthConfigAPI.Interfaces.Services;
using AuthConfigAPI.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthConfigAPI.Services
{
    public class JwtService :IJwtService
    {
        private readonly IConfiguration Configuration;
        private readonly IHttpContextAccessor ContextAccessor;
        public JwtService(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            Configuration = configuration;
            ContextAccessor = contextAccessor;
        }

        public string GenerateAccessToken(string email, string role)
        {
            var key = Encoding.UTF8.GetBytes(Configuration["JwtSettings:SecretKey"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email,email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Issuer = Configuration["JwtSettings:Issuer"],
                Audience = Configuration["JwtSettings:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            var response = ContextAccessor.HttpContext.Response;
            response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Domain = "localhost",
                SameSite = SameSiteMode.Strict,
            });

            return accessToken;
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(Configuration["JwtSettings:SecretKey"]);
                var tokenHandler = new JwtSecurityTokenHandler();

                return tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Configuration["JwtSettings:Issuer"],
                    ValidAudience = Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GenerateRefreshToken(string email, string role)
        {
            var key = Encoding.UTF8.GetBytes(Configuration["JwtSettings:SecretKey"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email,email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Issuer = Configuration["JwtSettings:Issuer"],
                Audience = Configuration["JwtSettings : Audience"],
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = tokenHandler.WriteToken(token);
            var response = ContextAccessor.HttpContext.Response;
            response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });
            return refreshToken;
        }
    }
}
