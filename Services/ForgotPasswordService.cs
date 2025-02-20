using AuthConfigAPI.Interfaces.Repositories;
using AuthConfigAPI.Interfaces.Services;
using AuthConfigAPI.Models;

namespace AuthConfigAPI.Services
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IOtpService OtpService;
        private readonly IUserRegistry UserRegistry;
        private readonly IBaseUserManagement<string, SignUpRequest> SignUpManangement;

        public ForgotPasswordService(IOtpService otpService, IUserRegistry userRegistry, IBaseUserManagement<string,SignUpRequest>baseUserManagement)
        {
            OtpService = otpService;
            UserRegistry = userRegistry;
            SignUpManangement = baseUserManagement;
        }

        public async Task<string> CheckTheUserExists(string email)
        {
            try
            {
                var ExistedUserData = await SignUpManangement.GetUserData(email);
                if (ExistedUserData != null)
                {
                    OtpService.SendEmailWithOtp(ExistedUserData);
                    return ExistedUserData.UserId;
                }
                throw new Exception("The Email is not exist, create new account");
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ResetThePassword(string userIdOrEmail,ForgotPasswordRequest forgotPasswordRequest)
        {
            try
            {
                var userData = await SignUpManangement.GetUserData(userIdOrEmail);
                userData.Password = forgotPasswordRequest.Password;
                await SignUpManangement.UpdateUserData(userData.UserId,userData);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string ValidateOTP(OtpModel otpModel)
        {
            try
            {
               return OtpService.ValidateOtp(otpModel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
