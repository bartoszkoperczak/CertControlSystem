using CertControlSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CertControlSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesApiController : ControllerBase
    {
        private readonly CertDbContext _context;

        public CertificatesApiController(CertDbContext context)
        {
            _context = context;
        }

        // GET: api/certificatesapi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CertificateDto>>> GetCertificates()
        {
            var certificates = await _context.Certificates
                .Include(c => c.Client)
                .Include(c => c.Type)
                .Select(c => new CertificateDto
                {
                    Id = c.Id,
                    ClientName = c.Client.FirstName + " " + c.Client.LastName,
                    TypeName = c.Type.Name,
                    IssueDate = c.IssueDate.ToString("yyyy-MM-dd"),
                    ExpirationDate = c.ExpirationDate.ToString("yyyy-MM-dd"),
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return Ok(certificates);
        }

        // GET: api/certificatesapi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CertificateDto>> GetCertificate(int id)
        {
            var c = await _context.Certificates
               .Include(client => client.Client)
               .Include(type => type.Type)
               .FirstOrDefaultAsync(x => x.Id == id);

            if (c == null)
            {
                return NotFound();
            }

            var dto = new CertificateDto
            {
                Id = c.Id,
                ClientName = c.Client.FirstName + " " + c.Client.LastName,
                TypeName = c.Type.Name,
                IssueDate = c.IssueDate.ToString("yyyy-MM-dd"),
                ExpirationDate = c.ExpirationDate.ToString("yyyy-MM-dd"),
                IsActive = c.IsActive
            };

            return Ok(dto);
        }

        // POST: api/certificatesapi
        [HttpPost]
        public async Task<ActionResult<Certificate>> PostCertificate(Certificate certificate)
        {
            certificate.IsActive = certificate.ExpirationDate >= DateOnly.FromDateTime(DateTime.Now);
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCertificate), new { id = certificate.Id }, certificate);
        }

        // PUT: api/certificatesapi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCertificate(int id, Certificate certificate)
        {
            if (id != certificate.Id)
                return BadRequest();

            certificate.IsActive = certificate.ExpirationDate >= DateOnly.FromDateTime(DateTime.Now);
            _context.Entry(certificate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Certificates.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/certificatesapi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
                return NotFound();

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}