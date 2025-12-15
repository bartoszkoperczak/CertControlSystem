using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CertControlSystem.Models;

public partial class NotificationLog
{
    [Key]
    public int Id { get; set; }

    public int CertificateId { get; set; }

    [StringLength(20)]
    public string Channel { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime SentDate { get; set; }

    public string? MessageContent { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [ForeignKey("CertificateId")]
    [InverseProperty("NotificationLogs")]
    public virtual Certificate Certificate { get; set; } = null!;
}
