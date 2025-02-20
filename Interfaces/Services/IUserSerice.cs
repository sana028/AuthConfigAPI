using AuthConfigAPI.Models;

namespace AuthConfigAPI.Interfaces.Services
{
    public interface IUserSerice
    {
        Task AddNewSignupUserData(SignUpRequest signUpData);
        Task<SignUpRequest> GetCurrentUserData(string userId);
        Task UpdateCurrentUserData(string userId,SignUpRequest request);

        Task<string> VerifyOTP(OtpModel otpModel);

        Task<string> ValidateUserCredentials(LoginRequest loginRequest);

        Task<string> ResendOTP(string email);
    }
}
