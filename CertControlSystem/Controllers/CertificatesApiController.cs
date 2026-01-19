using CertControlSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
        public async Task<ActionResult<CertificateDto>> PostCertificate([FromBody] CertificateCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!DateOnly.TryParseExact(dto.IssueDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var issueDate) ||
                !DateOnly.TryParseExact(dto.ExpirationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var expirationDate))
            {
                return BadRequest(new { error = "Daty muszą mieć format yyyy-MM-dd" });
            }

            var client = await _context.Clients.FindAsync(dto.ClientId);
            if (client == null)
                return BadRequest(new { error = $"Klient o Id = {dto.ClientId} nie istnieje" });

            int resolvedTypeId;
            CertificateType? certificateType = null;

            if (dto.TypeId.HasValue && dto.TypeId.Value > 0)
            {
                certificateType = await _context.CertificateTypes.FindAsync(dto.TypeId.Value);
                if (certificateType == null)
                    return BadRequest(new { error = $"Typ certyfikatu o Id = {dto.TypeId.Value} nie istnieje" });
                resolvedTypeId = certificateType.Id;
            }
            else if (!string.IsNullOrWhiteSpace(dto.TypeName))
            {
                certificateType = await _context.CertificateTypes
                    .FirstOrDefaultAsync(t => t.Name == dto.TypeName);
                if (certificateType == null)
                    return BadRequest(new { error = $"Typ certyfikatu o nazwie '{dto.TypeName}' nie istnieje" });
                resolvedTypeId = certificateType.Id;
            }
            else
            {
                return BadRequest(new { error = "Należy podać TypeId lub TypeName" });
            }

            var certificate = new Certificate
            {
                ClientId = client.Id,
                TypeId = resolvedTypeId,
                IssueDate = issueDate,
                ExpirationDate = expirationDate,
                IsActive = expirationDate >= DateOnly.FromDateTime(DateTime.Now)
            };

            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();

            // zwracamy DTO w body
            var resultDto = new CertificateDto
            {
                Id = certificate.Id,
                ClientName = client.FirstName + " " + client.LastName,
                TypeName = certificateType != null ? certificateType.Name : (await _context.CertificateTypes.FindAsync(certificate.TypeId))?.Name ?? string.Empty,
                IssueDate = certificate.IssueDate.ToString("yyyy-MM-dd"),
                ExpirationDate = certificate.ExpirationDate.ToString("yyyy-MM-dd"),
                IsActive = certificate.IsActive
            };

            return CreatedAtAction(nameof(GetCertificate), new { id = certificate.Id }, resultDto);
        }

        // PUT: api/certificatesapi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCertificate(int id, [FromBody] CertificateUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.Id)
                return BadRequest(new { error = "Niezgodne Id w URL i body" });

            if (!DateOnly.TryParseExact(dto.IssueDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var issueDate) ||
                !DateOnly.TryParseExact(dto.ExpirationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var expirationDate))
            {
                return BadRequest(new { error = "Daty muszą mieć format yyyy-MM-dd" });
            }

            var client = await _context.Clients.FindAsync(dto.ClientId);
            if (client == null)
                return BadRequest(new { error = $"Klient o Id = {dto.ClientId} nie istnieje" });

            int resolvedTypeId;
            if (dto.TypeId.HasValue && dto.TypeId.Value > 0)
            {
                var certType = await _context.CertificateTypes.FindAsync(dto.TypeId.Value);
                if (certType == null)
                    return BadRequest(new { error = $"Typ certyfikatu o Id = {dto.TypeId.Value} nie istnieje" });
                resolvedTypeId = certType.Id;
            }
            else if (!string.IsNullOrWhiteSpace(dto.TypeName))
            {
                var certType = await _context.CertificateTypes
                    .FirstOrDefaultAsync(t => t.Name == dto.TypeName);
                if (certType == null)
                    return BadRequest(new { error = $"Typ certyfikatu o nazwie '{dto.TypeName}' nie istnieje" });
                resolvedTypeId = certType.Id;
            }
            else
            {
                return BadRequest(new { error = "Należy podać TypeId lub TypeName" });
            }

            var certificate = new Certificate
            {
                Id = dto.Id,
                ClientId = dto.ClientId,
                TypeId = resolvedTypeId,
                IssueDate = issueDate,
                ExpirationDate = expirationDate,
                IsActive = expirationDate >= DateOnly.FromDateTime(DateTime.Now)
            };

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