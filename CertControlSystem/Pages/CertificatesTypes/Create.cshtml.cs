using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CertControlSystem.Models;

namespace CertControlSystem.Pages.CertificatesTypes
{
    public class CreateModel : PageModel
    {
        private readonly CertControlSystem.Models.CertDbContext _context;

        public CreateModel(CertControlSystem.Models.CertDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CertificateType CertificateType { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.CertificateTypes.Add(CertificateType);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
