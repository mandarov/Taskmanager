using System.ComponentModel.DataAnnotations;

namespace TaskPlanner.Api.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? CompletedDate { get; set; }
    }
}