// Models/TaskItem.cs
using System.ComponentModel.DataAnnotations;

namespace TrelloClone.API.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "todo"; // todo, doing, done [cite: 34]

        // İlişkiler: Her görev bir projeye aittir.
        [Required]
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        // Görev bir kullanıcıya atanabilir (Opsiyonel / Nullable)
        public int? AssignedToUserId { get; set; }
        public User? AssignedToUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}