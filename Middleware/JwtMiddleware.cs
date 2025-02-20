using AuthConfigAPI.Interfaces.Services;
using AuthConfigAPI.Models;
using AuthConfigAPI.Services;
using System.Security.Claims;

namespace AuthConfigAPI.Middleware
{
    public class JwtMiddleware
    {
        private RequestDelegate RequestDelegate;
        private IJwtService JwtService;

        public JwtMiddleware(RequestDelegate requestDelegate, IJwtService jwtService)
        {
            RequestDelegate = requestDelegate;
            JwtService = jwtService;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies["access_token"];
            if(!string.IsNullOrEmpty(token) )
            {
                var principal = JwtService.ValidateToken(token);
                if(principal != null)
                {
                    context.User = principal;
                }
                else
                {
                    var refreshToken = context.Request.Cookies["RefreshToken"];

                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        var validatefreshtoken = JwtService.ValidateToken(refreshToken);
                        if (validatefreshtoken != null)
                        {

                            var email = validatefreshtoken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                            var role = validatefreshtoken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                            

                            var newToken = JwtService.GenerateAccessToken(email,role); // Replace with actual refresh logic
                            //context.Response.Cookies.Append("AuthToken", newToken, new CookieOptions { HttpOnly = true, Secure = true });
                            context.User = JwtService.ValidateToken(newToken);
                        }
                        else
                        {
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsync("Session expired. Please log in again.");
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized. Please log in again.");
                        return;
                    }
                }
            }
            await RequestDelegate(context);
        }
    }
}
