// Models/Project.cs
using System.ComponentModel.DataAnnotations;

namespace TrelloClone.API.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        // İlişkiler: Her projenin bir sahibi (User) vardır.
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Bir projede birden fazla görev olabilir.
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}