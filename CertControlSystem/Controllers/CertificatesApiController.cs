using CertControlSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CertControlSystem.Controllers.Api
{
    [Route("api/[controller]")] // Adres będzie: /api/certificatesapi
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
            // Pobieramy dane z bazy i "mapujemy" je na bezpieczny format DTO
            var certificates = await _context.Certificates
                .Include(c => c.Client)
                .Include(c => c.Type)
                .Select(c => new CertificateDto
                {
                    Id = c.Id,
                    // Łączymy Imię i Nazwisko w jeden napis
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
    }
}