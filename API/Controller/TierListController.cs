using System.Security.Claims;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TierListController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TierListController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TierList (Tout le monde peut voir)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TierList>>> GetTierLists()
        {
            return await _context.TierLists
                .Include(t => t.Utilisateur)
                .Include(t => t.Contenus)
                    .ThenInclude(c => c.Element)
                .ToListAsync();
        }

        // POST: api/TierList (Créer une liste)
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TierList>> CreateTierList([FromBody] TierListDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var tierList = new TierList
            {
                Nom = dto.Nom,
                UtilisateurId = userId
            };

            _context.TierLists.Add(tierList);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTierLists), new { id = tierList.Id }, tierList);
        }

        
        [Authorize]
        [HttpPost("{id}/contenu")]
        public async Task<IActionResult> AddContenu(int id, [FromBody] ContenuDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tierList = await _context.TierLists.FindAsync(id);

            if (tierList == null) return NotFound();

            // Vérif propriétaire
            if (tierList.UtilisateurId != userId)
            {
                return StatusCode(403, "Seul le propriétaire peut modifier cette liste.");
            }

            // Vérifier si l'association existe 
            var existing = await _context.ContenusTierList
                .FirstOrDefaultAsync(c => c.TierListId == id && c.ElementId == dto.ElementId);

            if (existing != null)
            {
                
                existing.Notation = dto.Notation;
            }
            else
            {
             
                var contenu = new ContenuTierList
                {
                    TierListId = id,
                    ElementId = dto.ElementId,
                    Notation = dto.Notation
                };
                _context.ContenusTierList.Add(contenu);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTierList(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tierList = await _context.TierLists.FindAsync(id);

            if (tierList == null) return NotFound();

            if (tierList.UtilisateurId != userId)
            {
                return StatusCode(403, "Interdit : Vous n'êtes pas le propriétaire.");
            }

            _context.TierLists.Remove(tierList);
            
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    
    public class TierListDto
    {
        public string Nom { get; set; } = string.Empty;
    }

    public class ContenuDto
    {
        public int ElementId { get; set; }
        public string Notation { get; set; } = string.Empty;
    }
}