using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CertControlSystem.Models;

namespace CertControlSystem.Pages.CertificatesTypes
{
    public class IndexModel : PageModel
    {
        private readonly CertControlSystem.Models.CertDbContext _context;

        public IndexModel(CertControlSystem.Models.CertDbContext context)
        {
            _context = context;
        }

        public IList<CertificateType> CertificateType { get;set; } = default!;

        public async Task OnGetAsync()
        {
            CertificateType = await _context.CertificateTypes.ToListAsync();
        }
    }
}
