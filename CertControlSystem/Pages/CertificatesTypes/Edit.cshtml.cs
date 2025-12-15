using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CertControlSystem.Models;

namespace CertControlSystem.Pages.CertificatesTypes
{
    public class EditModel : PageModel
    {
        private readonly CertControlSystem.Models.CertDbContext _context;

        public EditModel(CertControlSystem.Models.CertDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CertificateType CertificateType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificatetype =  await _context.CertificateTypes.FirstOrDefaultAsync(m => m.Id == id);
            if (certificatetype == null)
            {
                return NotFound();
            }
            CertificateType = certificatetype;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CertificateType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CertificateTypeExists(CertificateType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CertificateTypeExists(int id)
        {
            return _context.CertificateTypes.Any(e => e.Id == id);
        }
    }
}
