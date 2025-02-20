using AuthConfigAPI.Data;
using AuthConfigAPI.Interfaces.Repositories;
using AuthConfigAPI.Interfaces.Services;
using AuthConfigAPI.Models;
using Azure.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthConfigAPI.Services
{
    public class UserManagementService : IUserSerice
    {
        private readonly IBaseUserManagement<string, SignUpRequest> UserManagement;
        private readonly IBaseUserManagement<string, TemporarySignUpData> TemporarySignUpData;
        private readonly IOtpService OtpService;
        private readonly IUserRegistry UserRegistry;
        private readonly IJwtService JwtService;
        private readonly AuthenticationDbContext AuthenticationContext;
        public UserManagementService(IBaseUserManagement<string, SignUpRequest> userManagement, IOtpService otpService, IUserRegistry userRegistry, IBaseUserManagement<string, TemporarySignUpData> temporarySignUpData, IJwtService jwtService, AuthenticationDbContext authenticationDbContext)
        {
            UserManagement = userManagement;
            OtpService = otpService;
            UserRegistry = userRegistry;
            TemporarySignUpData = temporarySignUpData;
            JwtService = jwtService;
            AuthenticationContext = authenticationDbContext;
        }

        public async Task AddNewSignupUserData(SignUpRequest signupData)
        {
            try
            {
                var isUserExist = await UserManagement.GetUserData(signupData.Email);

                if (isUserExist != null)
                {
                    throw new Exception("UserAlreadyExists: A user with this email or ID already exists.Please login instead.");
                }
                else
                {
                    var isUserExistButNotVerified = await TemporarySignUpData.GetUserData(signupData.Email);
                    if (isUserExistButNotVerified != null)
                    {
                        throw new Exception("You have already signed up. Please verify your OTP.");
                    }
                    else
                    {
                        OtpService.SendEmailWithOtp(signupData);
                        var data = new TemporarySignUpData
                        {
                            Email = signupData.Email,
                            UserId = signupData.UserId,
                            Password = signupData.Password,
                            Role = "User"
                        };
                        await TemporarySignUpData.AddNewUserData(data);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> VerifyOTP(OtpModel otpModel)
        {
            try
            {
                OtpService.ValidateOtp(otpModel);
                var userTempData = await TemporarySignUpData.GetUserData(otpModel.Email);
                var data = new SignUpRequest
                {
                    Email = otpModel.Email,
                    UserId = userTempData.UserId,
                    Password = userTempData.Password,
                    Role = "User"
                };
                await UserManagement.AddNewUserData(data);
                await TemporarySignUpData.DeleteUserData(userTempData.Email);
                var token = JwtService.GenerateAccessToken(data.Email, data.Role);
                return token;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SignUpRequest> GetCurrentUserData(string userId)
        {
            try
            {
                return await UserManagement.GetUserData(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateCurrentUserData(string userId, SignUpRequest request)
        {
            try
            {
                await UserManagement.UpdateUserData(userId, request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> ValidateUserCredentials(LoginRequest loginRequest)
        {
            try
            {
                var user = await UserRegistry.checkUserCredentials(loginRequest.UserIdorEmail, loginRequest.Password);
                if (user != null)
                {
                    //HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    var token = JwtService.GenerateAccessToken(user.Email, user.Role);
                    return token;
                }
                throw new Exception("Incorrect password or email. Click 'Forgot Password' to reset Or create new account if you are new to the site");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<string> ResendOTP(string email)
        {
            try
            {
                var userTempData = await TemporarySignUpData.GetUserData(email);

                var data = new SignUpRequest
                {
                    Email = email,
                    UserId = userTempData.UserId,
                    Password = userTempData.Password,
                    Role = "User"
                };
                OtpService.SendEmailWithOtp(data);
                return "Otp send to the email";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
