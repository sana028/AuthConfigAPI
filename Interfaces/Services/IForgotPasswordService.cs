using AuthConfigAPI.Models;

namespace AuthConfigAPI.Interfaces.Services
{
    public interface IForgotPasswordService
    {
        Task<string> CheckTheUserExists(string email);

        string ValidateOTP(OtpModel otpModel);

        Task ResetThePassword(string userIdOrEmail, ForgotPasswordRequest forgotPasswordRequest);
    }
}
