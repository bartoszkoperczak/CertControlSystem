using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CertControlSystem.Models;

public partial class CertificateType
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public int DefaultValidityMonths { get; set; }

    [InverseProperty("Type")]
    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}
