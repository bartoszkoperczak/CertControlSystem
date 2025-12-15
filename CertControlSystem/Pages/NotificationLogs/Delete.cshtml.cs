using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CertControlSystem.Models;

namespace CertControlSystem.Pages.NotificationLogs
{
    public class DeleteModel : PageModel
    {
        private readonly CertControlSystem.Models.CertDbContext _context;

        public DeleteModel(CertControlSystem.Models.CertDbContext context)
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

            var notificationlog = await _context.NotificationLogs.FirstOrDefaultAsync(m => m.Id == id);

            if (notificationlog is not null)
            {
                NotificationLog = notificationlog;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificationlog = await _context.NotificationLogs.FindAsync(id);
            if (notificationlog != null)
            {
                NotificationLog = notificationlog;
                _context.NotificationLogs.Remove(NotificationLog);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
