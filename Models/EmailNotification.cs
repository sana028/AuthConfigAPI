namespace AuthConfigAPI.Models
{
    public class EmailNotification
    {
        public required string Email { get; set; }
        public required string Subject { get; set; }

        public required string Body { get; set; }
    }
}
