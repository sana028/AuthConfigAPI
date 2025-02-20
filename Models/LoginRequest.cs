using System.ComponentModel.DataAnnotations;

namespace AuthConfigAPI.Models
{
    public class LoginRequest
    {
        [Key]
        [Required]
        public required string UserIdorEmail { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
