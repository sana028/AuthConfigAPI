namespace AuthConfigAPI.Models
{
    public class ForgotPasswordRequest
    {
        public required string Password { get; set; }   

        public required string NewPassword { get; set; }
    }
}
