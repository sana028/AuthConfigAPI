using AuthConfigAPI.Interfaces.Repositories;
using AuthConfigAPI.Interfaces.Services;
using AuthConfigAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthConfigAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IForgotPasswordService ForgotPasswordService;


        public ForgotPasswordController(IForgotPasswordService passwordService)
        {
           ForgotPasswordService = passwordService;
        }

        [HttpPost("{email}")]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            try
            {
                await ForgotPasswordService.CheckTheUserExists(email);
                return Ok("Successfully sent the otp to your email");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("/verifyOTP")]
        public IActionResult ValidateOTP(OtpModel otpModel)
        {
            try
            {
                var email = ForgotPasswordService.ValidateOTP(otpModel);
                return Ok(email);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch("resetPassword/{userIdOrEmail}")]
        public async Task<IActionResult> ResetPassword(string userIdOrEmail,[FromBody] ForgotPasswordRequest passwordRequest)
        {
            try
            {
                await ForgotPasswordService.ResetThePassword(userIdOrEmail, passwordRequest);
                return Ok("Now try to login with your new password");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
       
    }
}
