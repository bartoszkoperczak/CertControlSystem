namespace CertControlSystem.Models
{
    // Ta klasa służy tylko do wysyłania czystych danych przez API
    public class CertificateDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string IssueDate { get; set; } = string.Empty;
        public string ExpirationDate { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}