using System;
using System.ComponentModel.DataAnnotations;

namespace TrelloClone.API.Models
{
    public class TodoList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}