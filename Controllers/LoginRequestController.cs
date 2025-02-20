using AuthConfigAPI.Interfaces.Repositories;
using AuthConfigAPI.Interfaces.Services;
using AuthConfigAPI.Models;
using Azure.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthConfigAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginRequestController : ControllerBase
    {
        private readonly IUserSerice UserSerice;
        private readonly IHttpContextAccessor HttpContextAccessor;
        public LoginRequestController(IUserSerice userService, IHttpContextAccessor httpContext)
        {
            UserSerice = userService;
            HttpContextAccessor = httpContext;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var token = await UserSerice.ValidateUserCredentials(loginRequest);
                if(token == null) { 
                    return Unauthorized("Invalid credentials");
                }
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, loginRequest.Password) };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("protected-route")]
        public IActionResult ProtectedRoute()
        {
            var accessToken = Request.Cookies["access_token"];


            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized(new { message = "Access denied" });

            return Ok(new { message = "You are authorized", token = accessToken });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = "Logout successful" });
        }

    }
}
