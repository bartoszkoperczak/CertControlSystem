using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CertControlSystem.Models;

public partial class Certificate
{
    [Key]
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int TypeId { get; set; }

    public DateOnly IssueDate { get; set; }

    public DateOnly ExpirationDate { get; set; }

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
