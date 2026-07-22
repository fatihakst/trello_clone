using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrelloClone.API.Data;
using TrelloClone.API.Models;

namespace TrelloClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListsController : ControllerBase
    {
        // ApplicationDbContext yerine AppDbContext yazıldı
        private readonly AppDbContext _context;

        public ListsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLists([FromQuery] int projectId)
        {
            var lists = await _context.Lists
                .Where(l => l.ProjectId == projectId)
                .ToListAsync();

            return Ok(lists);
        }

        [HttpPost]
        public async Task<IActionResult> CreateList([FromBody] TodoList list)
        {
            if (list == null)
            {
                return BadRequest();
            }

            _context.Lists.Add(list);
            await _context.SaveChangesAsync();

            return Ok(list);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteList(int id)
        {
            var list = await _context.Lists.FindAsync(id);
            if (list == null) return NotFound("Liste bulunamadı.");

            // Listeye ait olan tüm görevleri bul ve tamamen temizle
            var tasksToDelete = await _context.Tasks.Where(t => t.Status == id.ToString()).ToListAsync();
            _context.Tasks.RemoveRange(tasksToDelete);

            // Listeyi sil
            _context.Lists.Remove(list);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}