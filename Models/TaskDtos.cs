namespace TrelloClone.API.Models
{
    public class TaskCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Todo"; // Todo, InProgress, Done
        public int ProjectId { get; set; }
        public int? AssignedToUserId { get; set; }
        public string? LabelText { get; set; }  // Örn: "Acil", "Frontend", "Bug"
        public string? LabelColor { get; set; } // Örn: "#ff4d4f" (Kırmızı), "#108ee9" (Mavi)
    }

    public class TaskUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? AssignedToUserId { get; set; }
        public string? LabelText { get; set; }  // Örn: "Acil", "Frontend", "Bug"
        public string? LabelColor { get; set; } // Örn: "#ff4d4f" (Kırmızı), "#108ee9" (Mavi)  
    }
}