using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CertControlSystem.Models;

namespace CertControlSystem.Pages.NotificationLogs
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
        ViewData["CertificateId"] = new SelectList(_context.Certificates, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public NotificationLog NotificationLog { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.NotificationLogs.Add(NotificationLog);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
