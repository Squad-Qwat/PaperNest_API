using System.ComponentModel.DataAnnotations;

namespace API.Models.DataBinding
{
    public class CreateDocumentBody
    {
        [Required, StringLength(255, ErrorMessage = "Comment max 255 characters")]
        public string Comment { get; set; }

        [Required]
        public string Content { get; set; }

    }
}
