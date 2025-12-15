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
    public class DetailsModel : PageModel
    {
        private readonly CertControlSystem.Models.CertDbContext _context;

        public DetailsModel(CertControlSystem.Models.CertDbContext context)
        {
            _context = context;
        }

        public CertificateType CertificateType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificatetype = await _context.CertificateTypes.FirstOrDefaultAsync(m => m.Id == id);

            if (certificatetype is not null)
            {
                CertificateType = certificatetype;

                return Page();
            }

            return NotFound();
        }
    }
}
