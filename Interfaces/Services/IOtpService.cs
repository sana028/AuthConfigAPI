using AuthConfigAPI.Models;

namespace AuthConfigAPI.Interfaces.Services
{
    public interface IOtpService
    {
        string GenerateOTP(string email);
        string ValidateOtp(OtpModel otpModel);

        void SendEmailWithOtp(SignUpRequest signUpRequest);
    }
}
