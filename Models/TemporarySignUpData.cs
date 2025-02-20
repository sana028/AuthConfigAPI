using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthConfigAPI.Models
{
    public class TemporarySignUpData 
    {
        
        [Required]
        public required string UserId { get; set; }
        [Key]
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [BindNever]
        [JsonIgnore]
        public string Role { get; set; } = string.Empty;

        [BindNever]
        [JsonIgnore]
        public DateOnly CreatedAt { get; set; } = new DateOnly();

    }
}
