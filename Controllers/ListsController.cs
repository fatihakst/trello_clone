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
    }
}