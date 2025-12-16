using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CertControlSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace CertControlSystem.Controllers
{
    [Authorize]
    public class CertificatesController : Controller
    {
        private readonly CertDbContext _context;

        public CertificatesController(CertDbContext context)
        {
            _context = context;
        }

        // GET: Certificates
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var certDbContext = _context.Certificates.Include(c => c.Client).Include(c => c.Type);
            return View(await certDbContext.ToListAsync());
        }

        // GET: Certificates/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _context.Certificates
                .Include(c => c.Client)
                .Include(c => c.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }

        // GET: Certificates/Create
        public IActionResult Create()
        {
            // 1. Tworzymy listę z pełnym imieniem i nazwiskiem
            var clients = _context.Clients
                .Select(c => new { Id = c.Id, FullName = c.FirstName + " " + c.LastName })
                .ToList();

            ViewData["ClientId"] = new SelectList(clients, "Id", "FullName");

            // 2. Lista typów
            ViewData["TypeId"] = new SelectList(_context.CertificateTypes, "Id", "Name");

            // 3. Przekazujemy do widoku słownik: ID Typu -> Ile miesięcy ważny
            // To pozwoli JavaScriptowi automatycznie ustawić datę
            ViewBag.TypeValidities = _context.CertificateTypes
                .ToDictionary(t => t.Id, t => t.DefaultValidityMonths);

            return View();
        }

        // POST: Certificates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientId,TypeId,IssueDate,ExpirationDate")] Certificate certificate)
        {
            ModelState.Remove("Client");
            ModelState.Remove("Type");
            ModelState.Remove("NotificationLogs");

            if (ModelState.IsValid)
            {
                certificate.IsActive = certificate.ExpirationDate >= DateOnly.FromDateTime(DateTime.Now);
                _context.Add(certificate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var clients = _context.Clients
                .Select(c => new { Id = c.Id, FullName = c.FirstName + " " + c.LastName })
                .ToList();
            ViewData["ClientId"] = new SelectList(clients, "Id", "FullName", certificate.ClientId);
            ViewData["TypeId"] = new SelectList(_context.CertificateTypes, "Id", "Name", certificate.TypeId);

            ViewBag.TypeValidities = _context.CertificateTypes
                .ToDictionary(t => t.Id, t => t.DefaultValidityMonths);

            return View(certificate);
        }

        // GET: Certificates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null) return NotFound();

            // 1. To samo dla edycji - pełne nazwy
            var clients = _context.Clients
                .Select(c => new { Id = c.Id, FullName = c.FirstName + " " + c.LastName })
                .ToList();

            ViewData["ClientId"] = new SelectList(clients, "Id", "FullName", certificate.ClientId);
            ViewData["TypeId"] = new SelectList(_context.CertificateTypes, "Id", "Name", certificate.TypeId);

            // 2. Przekazujemy ważność typów (gdyby ktoś zmieniał typ przy edycji)
            ViewBag.TypeValidities = _context.CertificateTypes
                .ToDictionary(t => t.Id, t => t.DefaultValidityMonths);

            return View(certificate);
        }

        // POST: Certificates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,TypeId,IssueDate,ExpirationDate,IsActive")] Certificate certificate)
        {
            if (id != certificate.Id) return NotFound();

            ModelState.Remove("Client");
            ModelState.Remove("Type");
            ModelState.Remove("NotificationLogs");

            if (ModelState.IsValid)
            {
                try
                {
                    certificate.IsActive = certificate.ExpirationDate >= DateOnly.FromDateTime(DateTime.Now);
                    _context.Update(certificate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CertificateExists(certificate.Id))
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
            var clients = _context.Clients
                    .Select(c => new { Id = c.Id, FullName = c.FirstName + " " + c.LastName })
                    .ToList();
            ViewData["ClientId"] = new SelectList(clients, "Id", "FullName", certificate.ClientId);
            ViewData["TypeId"] = new SelectList(_context.CertificateTypes, "Id", "Name", certificate.TypeId);

            ViewBag.TypeValidities = _context.CertificateTypes
                .ToDictionary(t => t.Id, t => t.DefaultValidityMonths);

            return View(certificate);
        }

        // GET: Certificates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _context.Certificates
                .Include(c => c.Client)
                .Include(c => c.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }

        // POST: Certificates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate != null)
            {
                _context.Certificates.Remove(certificate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CertificateExists(int id)
        {
            return _context.Certificates.Any(e => e.Id == id);
        }
    }
}
