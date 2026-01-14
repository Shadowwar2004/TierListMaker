using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ElementController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Element>>> GetElements()
        {
            return await _context.Elements.ToListAsync();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Element>> PostElement(Element element)
        {
            var categoriesValides = new[] { "Film", "Série", "Jeu vidéo" };
            if (!categoriesValides.Contains(element.Categorie))
            {
                return BadRequest("La catégorie doit être 'Film', 'Série' ou 'Jeu vidéo'.");
            }

            _context.Elements.Add(element);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetElements", new { id = element.Id }, element);
        }
    }
}