using API.Helpers.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Citation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public CitationType Type { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Author { get; set; }

        [Required]

        public string? Pages { get; set; } // Optional, can be null if not applicable
        public string? Volume { get; set; } // Optional, can be null if not applicable
        public string? Issue { get; set; } // Optional, can be null if not applicable
        public string? URL { get; set; } // Optional, can be null if not applicable
        public string? AccessURL { get; set; } // Optional, can be null if not applicable
        public string? AccessLocation { get; set; } // Optional, can be null if not applicable

        public string? PublicationInfo { get; set; } //  Details vary by type (e.g., ISBN, Journal Name, URL)

        public string? Publisher { get; set; } // Optional, can be null if not applicable

        public DateTime? PublicationDate { get; set; } // Use nullable DateTime

        public string? PublisherLocation { get; set; } // Optional, can be null if not applicable

        public string? AccessDate { get; set; }

        public string? DOI { get; set; }

        [Required]
        public Guid FK_DocumentId { get; set; }
        
        [ForeignKey("FK_DocumentId")]
        public Document? Document { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}