using AuthConfigAPI.Interfaces.Services;
using AuthConfigAPI.Models;
using Azure.Core;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;

namespace AuthConfigAPI.Services
{
    public class OtpService : IOtpService
    {
        private readonly IHttpContextAccessor ContextAccessor;
        private readonly IEmailService EmailService;
        private static readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> otpStorage = new();

        public OtpService(IHttpContextAccessor contextAccessor, IEmailService emailService)
        {
            ContextAccessor = contextAccessor;
            EmailService = emailService;
        }

        // Generate and Store OTP in a Secure Cookie
        public string GenerateOTP(string email)
        {
            var randomOtp = GenerateRandomOtp();
            DateTime expiry = DateTime.UtcNow.AddMinutes(2); // OTP valid for 2 minutes

            otpStorage[email] = (randomOtp, expiry);


            return randomOtp;
        }

        public string ValidateOtp(OtpModel request)
        {
            if (otpStorage.TryGetValue(request.Email, out var storedOtp))
            {
                if (DateTime.UtcNow > storedOtp.Expiry)
                {
                    otpStorage.TryRemove(request.Email, out _);
                    throw new Exception ("OTP expired. Please request a new one.");
                }

                if (storedOtp.Otp == request.Otp)
                {
                    otpStorage.TryRemove(request.Email, out _);
                    return "OTP verified successfully!";
                }

                throw new Exception ("Invalid OTP.");
            }
            throw new Exception("OTP not send to email, Try again");
        }

        // Generate a random 6-digit OTP
        private string GenerateRandomOtp()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[4];
                rng.GetBytes(data);
                var otp = BitConverter.ToUInt32(data, 0) % 1000000;
                return otp.ToString("D6");
            }
        }

        public void SendEmailWithOtp(SignUpRequest signUpRequest)
        {
            var otp = GenerateOTP(signUpRequest.Email);
            var body = EmailService.GetOtpEmailBody(signUpRequest.UserId, otp);
            try
            {
                var emailNotificationPayload = new EmailNotification
                {
                    Body = body,
                    Subject = $"{signUpRequest.UserId} here is your PIN {otp}",
                    Email = signUpRequest.Email,
                };
                EmailService.SendEmail(emailNotificationPayload);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
