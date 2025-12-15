using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CertControlSystem.Models;

namespace CertControlSystem.Pages.NotificationLogs
{
    public class EditModel : PageModel
    {
        private readonly CertControlSystem.Models.CertDbContext _context;

        public EditModel(CertControlSystem.Models.CertDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public NotificationLog NotificationLog { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificationlog =  await _context.NotificationLogs.FirstOrDefaultAsync(m => m.Id == id);
            if (notificationlog == null)
            {
                return NotFound();
            }
            NotificationLog = notificationlog;
           ViewData["CertificateId"] = new SelectList(_context.Certificates, "Id", "Id");
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

            _context.Attach(NotificationLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationLogExists(NotificationLog.Id))
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

        private bool NotificationLogExists(int id)
        {
            return _context.NotificationLogs.Any(e => e.Id == id);
        }
    }
}
