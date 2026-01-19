using System.ComponentModel.DataAnnotations;

namespace CertControlSystem.Models
{
    public class CertificateUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        public int? TypeId { get; set; }

        public string? TypeName { get; set; }

        [Required]
        public string IssueDate { get; set; } = string.Empty; // "yyyy-MM-dd"

        [Required]
        public string ExpirationDate { get; set; } = string.Empty; // "yyyy-MM-dd"
    }
}