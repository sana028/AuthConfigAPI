using AuthConfigAPI.Models;
using System.Security.Claims;

namespace AuthConfigAPI.Interfaces.Services
{
    public interface IJwtService
    {
        // generates the access_token
        string GenerateAccessToken(string email, string role);

        //represent the identity of user
        ClaimsPrincipal ValidateToken(string token);
        string GenerateRefreshToken(string email,string role);
    }
}
