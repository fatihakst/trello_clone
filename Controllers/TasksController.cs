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
                .OrderBy(t => t.Order) // Görevler artık veritabanındaki Order değerine göre sıralı gelecek
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
                LabelText = dto.LabelText,   // YENİ EKLENDİ
                LabelColor = dto.LabelColor, // YENİ EKLENDİ
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            // Güvenlik kontrolü eklemek istersen burada projeyi ve kullanıcıyı doğrulayabilirsin
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound("Görev bulunamadı.");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent(); // Başarıyla silindiğinde 204 döner
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
            task.LabelText = dto.LabelText;   // YENİ EKLENDİ
            task.LabelColor = dto.LabelColor; // YENİ EKLENDİ
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Sürükle-bırak sonrası yeni sırayı ve listeyi toplu olarak kaydeden metot
        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderTasks([FromBody] List<ReorderDto> tasksData)
        {
            foreach (var item in tasksData)
            {
                var task = await _context.Tasks.FindAsync(item.Id);
                if (task != null)
                {
                    task.Order = item.Order;
                    task.Status = item.Status;
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    // Frontend'den gelen sıralama verisini karşılayacak DTO
    public class ReorderDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Status { get; set; }
    }
}