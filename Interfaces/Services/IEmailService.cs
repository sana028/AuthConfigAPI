using AuthConfigAPI.Models;

namespace AuthConfigAPI.Interfaces.Services
{
    public interface IEmailService
    {
        void SendEmail(EmailNotification notification);
        string GetOtpEmailBody(string userName, string otp);
    }
}
