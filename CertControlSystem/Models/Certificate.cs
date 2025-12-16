using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Ważne!
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CertControlSystem.Models;

public partial class Certificate
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Klient")]
    [Required(ErrorMessage = "Wybór klienta jest wymagany")]
    public int ClientId { get; set; }

    [Display(Name = "Typ Certyfikatu")]
    [Required(ErrorMessage = "Typ certyfikatu jest wymagany")]
    public int TypeId { get; set; }

    [Display(Name = "Data Wystawienia")]
    [Required(ErrorMessage = "Data wystawienia jest wymagana")]
    [DataType(DataType.Date)]
    public DateOnly IssueDate { get; set; }

    [Display(Name = "Data Ważności")]
    [Required(ErrorMessage = "Data ważności jest wymagana")]
    [DataType(DataType.Date)]
    // Opcjonalnie: własna walidacja daty, ale na razie Required wystarczy
    public DateOnly ExpirationDate { get; set; }

    [Display(Name = "Czy Aktywny?")]
    public bool IsActive { get; set; }

    [ForeignKey("ClientId")]
    [InverseProperty("Certificates")]
    public virtual Client Client { get; set; } = null!;

    [InverseProperty("Certificate")]
    public virtual ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();

    [ForeignKey("TypeId")]
    [InverseProperty("Certificates")]
    public virtual CertificateType Type { get; set; } = null!;
}