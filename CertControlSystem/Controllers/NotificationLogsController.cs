using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CertControlSystem.Models;

namespace CertControlSystem.Controllers
{
    public class NotificationLogsController : Controller
    {
        private readonly CertDbContext _context;

        public NotificationLogsController(CertDbContext context)
        {
            _context = context;
        }

        // GET: NotificationLogs
        public async Task<IActionResult> Index()
        {
            var certDbContext = _context.NotificationLogs.Include(n => n.Certificate);
            return View(await certDbContext.ToListAsync());
        }

        // GET: NotificationLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificationLog = await _context.NotificationLogs
                .Include(n => n.Certificate)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notificationLog == null)
            {
                return NotFound();
            }

            return View(notificationLog);
        }

        // GET: NotificationLogs/Create
        public IActionResult Create()
        {
            ViewData["CertificateId"] = new SelectList(_context.Certificates, "Id", "Id");
            return View();
        }

        // POST: NotificationLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CertificateId,Channel,SentDate,MessageContent,Status")] NotificationLog notificationLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notificationLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CertificateId"] = new SelectList(_context.Certificates, "Id", "Id", notificationLog.CertificateId);
            return View(notificationLog);
        }

        // GET: NotificationLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificationLog = await _context.NotificationLogs.FindAsync(id);
            if (notificationLog == null)
            {
                return NotFound();
            }
            ViewData["CertificateId"] = new SelectList(_context.Certificates, "Id", "Id", notificationLog.CertificateId);
            return View(notificationLog);
        }

        // POST: NotificationLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CertificateId,Channel,SentDate,MessageContent,Status")] NotificationLog notificationLog)
        {
            if (id != notificationLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notificationLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationLogExists(notificationLog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CertificateId"] = new SelectList(_context.Certificates, "Id", "Id", notificationLog.CertificateId);
            return View(notificationLog);
        }

        // GET: NotificationLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificationLog = await _context.NotificationLogs
                .Include(n => n.Certificate)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notificationLog == null)
            {
                return NotFound();
            }

            return View(notificationLog);
        }

        // POST: NotificationLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notificationLog = await _context.NotificationLogs.FindAsync(id);
            if (notificationLog != null)
            {
                _context.NotificationLogs.Remove(notificationLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationLogExists(int id)
        {
            return _context.NotificationLogs.Any(e => e.Id == id);
        }
    }
}
