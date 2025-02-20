using System.Net.Mail;
using System.Net;
using AuthConfigAPI.Models;
using AuthConfigAPI.Interfaces.Services;

namespace AuthConfigAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration Configuration;

        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void SendEmail(EmailNotification data)
        {
            try
            {
                var emailSettings = Configuration.GetSection("EmailSettings");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings["fromEmail"]),
                    Subject = data.Subject,
                    Body = data.Body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(data.Email);

                using (var smtpClient = new SmtpClient(emailSettings["smtpServer"], int.Parse(emailSettings["smtpPort"])))
                {
                    smtpClient.Credentials = new NetworkCredential(emailSettings["fromEmail"], emailSettings["fromPassword"]);
                    smtpClient.EnableSsl = true; // Required for Gmail and most SMTP servers
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send the Email {ex.Message}");
            }
        }

        public string GetOtpEmailBody(string userName, string otp)
        {
            // HTML body (from your previous design)
        return $@"
          <!DOCTYPE html>
         <html lang='en'>
         <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>OTP Email</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f9;
                    margin: 0;
                    padding: 0;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    background-color: #ffffff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 0 15px rgba(0, 0, 0, 0.1);
                }}
                .header {{
                    text-align: center;
                }}
                .header img {{
                    width: 150px;
                    margin-bottom: 20px;
                }}
                .greeting {{
                    font-size: 20px;
                    color: #333333;
                    margin-bottom: 10px;
                }}
                .otp {{
                    font-size: 24px;
                    font-weight: bold;
                    color: #007bff;
                    margin: 20px 0;
                }}
                .additional-info {{
                    font-size: 14px;
                    color: #555555;
                    line-height: 1.6;
                }}
                .footer {{
                    text-align: center;
                    font-size: 12px;
                    color: #888888;
                    margin-top: 30px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <img src='https://your-logo-url.com/logo.png' alt='Company Logo'>
                </div>
                <div class='greeting'>
                    <p>Hi {userName},</p>
                    <p>Thank you for your request! Here is your One-Time Password (OTP) to proceed.</p>
                </div>
                <div class='otp'>
                    <p>{otp}</p>
                </div>
                <div class='additional-info'>
                    <p>Your OTP is valid for the next 10 minutes. Please do not share it with anyone.</p>
                    <p>If you did not request this, please ignore this message or contact support.</p>
                </div>
                <div class='footer'>
                    <p>&copy; 2025 Your Company. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";
        }

    }

}
