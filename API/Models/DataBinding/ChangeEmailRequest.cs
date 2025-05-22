using System.ComponentModel.DataAnnotations;

namespace API.Models.DataBinding
{
    public class ChangeEmailRequest
    {
        [Required, StringLength(255, ErrorMessage = "Email max 255 characters"), EmailAddress]
        public string? OldEmail { get; set; }

        [Required, StringLength(255, ErrorMessage = "Email max 255 characters"), EmailAddress]
        public string? NewEmail { get; set; }
    }
}
