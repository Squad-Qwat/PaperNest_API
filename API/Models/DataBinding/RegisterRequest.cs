using System.ComponentModel.DataAnnotations;
using API.Helpers.Enums;

namespace API.Models.DataBinding
{
    public class RegisterRequest
    {
        [Key, Required]
        public Guid Id { get; protected set; } = Guid.NewGuid();

        [Required, StringLength(100, ErrorMessage = "Name max 100 characters")]
        public string? Name { get; set; }

        [Required, StringLength(255, ErrorMessage = "Email max 255 characters"), EmailAddress]
        public string? Email { get; set; }

        [Required, StringLength(15, ErrorMessage = "Username max 15 characters")]
        public string? Username { get; set; }

        [Required, StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        public string? Password { get; set; }

        [Required, EnumDataType(typeof(UserRole))]
        public string? Role { get; set; } = UserRole.Student.ToString();
    }
}