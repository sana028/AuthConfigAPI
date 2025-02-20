using AuthConfigAPI.Interfaces.Repositories;
using AuthConfigAPI.Interfaces.Services;
using AuthConfigAPI.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthConfigAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly IUserSerice UserSerice;

        public SignUpController(IUserSerice userSerice)
        {
            UserSerice = userSerice;
        }


        // GET api/<SignUpController>/5
        [HttpGet("{userId}")]
        public async Task<ActionResult<SignUpRequest>> GetUserData(string userId)
        {
            try
            {
                var result = await UserSerice.GetCurrentUserData(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<SignUpController>
        [HttpPost]
        public async Task<ActionResult> AddNewSignupUserData([FromBody] SignUpRequest signUpRequest)
        {
            try
            {
                await UserSerice.AddNewSignupUserData(signUpRequest);
                return Ok("Sent the OTP to the email, verify your email");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserSettings(string userId, [FromBody] SignUpRequest signUpRequest)
        {
            try
            {
                await UserSerice.UpdateCurrentUserData(userId, signUpRequest);
                return Ok("OTP Verified");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("verifyNewUser")]
        public async Task<IActionResult> VerifyUserOTP(OtpModel otpModel)
        {
            try
            {
                var token = await UserSerice.VerifyOTP(otpModel);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("resendOtp")]
        public async Task<IActionResult> ResendOTPToEmail(string email)
        {
            try
            {
                var response = await UserSerice.ResendOTP(email);
                return Ok(response);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
