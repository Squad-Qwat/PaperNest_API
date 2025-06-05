using System.ComponentModel.DataAnnotations;

namespace API.Models.DataBinding
{
    public class LoginRequest
    {
        [Required, StringLength(255, ErrorMessage = "Email max 255 characters"), EmailAddress]
        public string? Email { get; set; }

        [Required, StringLength(15, ErrorMessage = "Username max 15 characters")]
        public string? Username { get; set; }

        [Required, StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        public string? Password { get; set; }
    }
}