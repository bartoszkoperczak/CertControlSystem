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
    public class CertificateTypesController : Controller
    {
        private readonly CertDbContext _context;

        public CertificateTypesController(CertDbContext context)
        {
            _context = context;
        }

        // GET: CertificateTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.CertificateTypes.ToListAsync());
        }

        // GET: CertificateTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificateType = await _context.CertificateTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certificateType == null)
            {
                return NotFound();
            }

            return View(certificateType);
        }

        // GET: CertificateTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CertificateTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DefaultValidityMonths")] CertificateType certificateType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(certificateType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(certificateType);
        }

        // GET: CertificateTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificateType = await _context.CertificateTypes.FindAsync(id);
            if (certificateType == null)
            {
                return NotFound();
            }
            return View(certificateType);
        }

        // POST: CertificateTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DefaultValidityMonths")] CertificateType certificateType)
        {
            if (id != certificateType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(certificateType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CertificateTypeExists(certificateType.Id))
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
            return View(certificateType);
        }

        // GET: CertificateTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificateType = await _context.CertificateTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certificateType == null)
            {
                return NotFound();
            }

            return View(certificateType);
        }

        // POST: CertificateTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var certificateType = await _context.CertificateTypes.FindAsync(id);
            if (certificateType != null)
            {
                _context.CertificateTypes.Remove(certificateType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CertificateTypeExists(int id)
        {
            return _context.CertificateTypes.Any(e => e.Id == id);
        }
    }
}
