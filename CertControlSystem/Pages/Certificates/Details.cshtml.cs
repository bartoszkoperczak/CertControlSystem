using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CertControlSystem.Models;

namespace CertControlSystem.Pages.Certificates
{
    public class DetailsModel : PageModel
    {
        private readonly CertControlSystem.Models.CertDbContext _context;

        public DetailsModel(CertControlSystem.Models.CertDbContext context)
        {
            _context = context;
        }

        public Certificate Certificate { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _context.Certificates.FirstOrDefaultAsync(m => m.Id == id);

            if (certificate is not null)
            {
                Certificate = certificate;

                return Page();
            }

            return NotFound();
        }
    }
}
