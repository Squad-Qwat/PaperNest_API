using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Review
    {
        [Key, Required]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
