using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrelloClone.API.Data;
using TrelloClone.API.Models;

namespace TrelloClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(int projectId)
        {
            var userId = GetUserId();
            // Güvenlik Kontrolü: Kullanıcı bu projenin sahibi mi?
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId && p.UserId == userId);
            if (!projectExists) return Forbid();

            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskCreateDto dto)
        {
            var userId = GetUserId();
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId && p.UserId == userId);
            if (!projectExists) return BadRequest("Geçersiz proje veya yetkisiz erişim.");

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                ProjectId = dto.ProjectId,
                AssignedToUserId = dto.AssignedToUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskUpdateDto dto)
        {
            var userId = GetUserId();
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == userId);

            if (task == null) return NotFound("Görev bulunamadı veya yetkiniz yok.");

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.AssignedToUserId = dto.AssignedToUserId;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(task);
        }
    }
}