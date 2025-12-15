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
    public class DetailsModel : PageModel
    {
        private readonly CertControlSystem.Models.CertDbContext _context;

        public DetailsModel(CertControlSystem.Models.CertDbContext context)
        {
            _context = context;
        }

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
    }
}
